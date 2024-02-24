using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class SimpleTask : Task
    {
        [JsonProperty]
        private int numLoops { get; set; }
        [JsonProperty]
        private string name { get; set; }
        [JsonProperty]
        private int endLoopNumber = 0;

        public SimpleTask(int numLoops, string name)
        {
            resources = new List<Resource>();
            this.numLoops = numLoops;
            this.name = name;
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

        public override void Run()
        {
            stopwatch.Start();
            lockResources(); // locks needed resources 
            for (int i = endLoopNumber; i < numLoops && deadLockDetected != true; i++,    endLoopNumber = i)
            {
                checkPause();
                checkWaitingToResume();
                //checkTime();

                Console.WriteLine("This is " + i + " loop! = " + name);
                Thread.Sleep(300);
                if (jobState == JobState.Finished)
                {
                    break;
                }

                Interlocked.Exchange(ref progressBarPercentage, (double) i / numLoops);
                if (updateProgressBar != null) updateProgressBar();
            }
            releaseResources(); 
            stopwatch.Stop();
        }

        public override string ToString()
        {
            return this.name + " (" + numLoops + ")";
        }
    }
}
