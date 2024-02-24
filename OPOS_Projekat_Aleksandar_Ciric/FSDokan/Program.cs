using FSDokan;
using DokanNet;
using DokanNet.Logging;
using Scheduler;

char driveLetter = 'Y';

Scheduler.TaskScheduler scheduler = new FifoScheduler(1);
Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();


Thread observingInput = new Thread(() =>
{
    while (true)
    {
        Thread.Sleep(1000);
        string folder = $"{driveLetter}:\\" + "input\\";
        var files = Directory.GetFiles(folder);
        foreach (var file in files)
        {
            if (!processedFiles.ContainsKey(file))
            {
                Thread.Sleep(100);
                List<Resource> resources = new List<Resource>();
                resources.Add(new FileResource(file));
                scheduler.Schedule(new ImageSharpeningTask(resources, "Y:\\output\\", 2));
                processedFiles.Add(file, true);
            }
        }
    }
});
observingInput.IsBackground = true;
observingInput.Start();

using (ConsoleLogger consoleLogger = new("[Dokan]"))
using (Dokan dokan = new(consoleLogger))
{
    string mountPoint = $"{driveLetter}:\\";
    SimpleFS myFs = new();
    DokanInstanceBuilder dokanInstanceBuilder = new DokanInstanceBuilder(dokan)
        .ConfigureLogger(() => consoleLogger)
        .ConfigureOptions(options =>
        {
            options.Options = DokanOptions.DebugMode;
            options.MountPoint = mountPoint;
        });
    using DokanInstance dokanInstance = dokanInstanceBuilder.Build(myFs);
    Console.ReadLine();
}

