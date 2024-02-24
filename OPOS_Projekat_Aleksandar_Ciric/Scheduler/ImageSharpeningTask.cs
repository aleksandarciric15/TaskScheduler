using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Scheduler
{
    public class ImageSharpeningTask : Task
    {
        [JsonProperty]
        private string outputFolder { get; set; }
        [JsonProperty]
        private int processedRows = 0;
        [JsonProperty]
        private int maxRows = 0;

        public ImageSharpeningTask(List<Resource> resources, string outputFolder, int degree)
        {
            this.resources = new(resources);
            this.outputFolder = outputFolder;
            this.degreeOfParallelism = degree;
            DefineThread();
        }

        public override void DefineThread()
        {
            thread = new Thread(() =>
            {
                try
                {
                    this.Run();
                }
                finally
                {
                    if (jobState != JobState.Finished)
                        Finish();
                }
            });
        }

        private void CalculateMaximumRows()
        {
            foreach (var resource in resources)
            {
                maxRows += new Bitmap(resource.getResource()).Height - 2 * (Kernels.Laplacian.Length - 1);
            }
        }

        public override void Run()
        {
            CalculateMaximumRows();
            lockResources();
            if (resources.Count == 1)
            {
                ExecuteTask(((FileResource)resources.ElementAt(0)).getResource());
            }
            else
            {
                Parallel.For(0, resources.Count, new ParallelOptions { MaxDegreeOfParallelism = resources.Count }, i =>
                {
                    ExecuteTask(((FileResource)resources.ElementAt(i)).getResource());
                });
            }
            releaseResources();
        }

        private void ExecuteTask(string image)
        {
            string fileName = Path.GetFileName(image);
            Bitmap sharpenedImage = ImageSharpenParallel(new Bitmap(image));
            string outPath = string.Concat(outputFolder, Path.DirectorySeparatorChar + "sharpened_" + fileName);
            sharpenedImage.Save(outPath);
        }

        private Bitmap ImageSharpenParallel(Bitmap image)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int w = image.Width;
            int h = image.Height;
            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            int dim = Kernels.Laplacian.Length;
            int size = dim - 1;

            Parallel.For(size, h - size, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, i =>
            {
                checkPause();
                checkWaitingToResume();


                for (int j = size; j < w - size; j++)
                {
                    if (jobState == JobState.Finished)
                    {
                        break;
                    }
                    int p = j * dim + i * image_data.Stride;
                    for (int k = 0; k < dim; k++)
                    {
                        double val = 0d;
                        for (int xkernel = -1; xkernel < 2; xkernel++)
                        {
                            for (int ykernel = -1; ykernel < 2; ykernel++)
                            {
                                int kernel_p = k + p + xkernel * 3 + ykernel * image_data.Stride;
                                val += buffer[kernel_p] * Kernels.Laplacian[xkernel + 1, ykernel + 1];
                            }
                        }
                        val = val > 0 ? val : 0;
                        result[p + k] = (byte)((val + buffer[p + k]) > 255 ? 255 : (val + buffer[p + k]));
                    }
                }
                if (jobState != JobState.Finished)
                {
                    ++processedRows;
                    Interlocked.Exchange(ref progressBarPercentage, 1.0 * processedRows / maxRows);
                    if (updateProgressBar != null) updateProgressBar();
                }
            });
            Bitmap res_img = new Bitmap(w, h);
            BitmapData res_data = res_img.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            Marshal.Copy(result, 0, res_data.Scan0, bytes);
            res_img.UnlockBits(res_data);
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            return res_img;
        }

        private Bitmap ImageSharpen(Bitmap image)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int w = image.Width;
            int h = image.Height;
            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            for (int i = 2; i < w - 2; i++)
            {
                checkPause();
                checkWaitingToResume();

                if (jobState == JobState.Finished)
                {
                    break;
                }
                for (int j = 2; j < h - 2; j++)
                {
                    int p = i * 3 + j * image_data.Stride;
                    for (int k = 0; k < 3; k++)
                    {
                        double val = 0d;
                        for (int xkernel = -1; xkernel < 2; xkernel++)
                        {
                            for (int ykernel = -1; ykernel < 2; ykernel++)
                            {
                                int kernel_p = k + p + xkernel * 3 + ykernel * image_data.Stride;
                                val += buffer[kernel_p] * Kernels.Laplacian[xkernel + 1, ykernel + 1];
                            }
                        }
                        val = val > 0 ? val : 0;
                        result[p + k] = (byte)((val + buffer[p + k]) > 255 ? 255 : (val + buffer[p + k]));
                    }
                }
                Interlocked.Exchange(ref progressBarPercentage, 1.0 * i / w);
                if (updateProgressBar != null) updateProgressBar();
            }

            Bitmap res_img = new Bitmap(w, h);
            BitmapData res_data = res_img.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            Marshal.Copy(result, 0, res_data.Scan0, bytes);
            res_img.UnlockBits(res_data);
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            return res_img;
        }
    }
}
