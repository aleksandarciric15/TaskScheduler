using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class SampleTask : Task
    {
        private static int redniBroj = 0;
        private int iD;
        private Resource r1;
        private Resource r2;
        private int timeSleep = 0;

        public SampleTask()
        {
            redniBroj++;
            iD = redniBroj;
        }

        public void InitializeThread(Resource r1, Resource r2, int sleep)
        {
            this.r1 = r1;
            this.r2 = r2;
            timeSleep = sleep;
            DefineThread();
        }

        public override void DefineThread()
        {
            thread = new Thread(() =>
            {
                this.r1.Lock(this);
                Thread.Sleep(timeSleep);
                this.r2.Lock(this);
                Thread.Sleep(timeSleep);
                this.r1.Release();
                this.r2.Release();
            });
        }

        public override void Run()
        {
            if (thread != null)
            {
                thread.Start();
            }
            else
            {
                Console.WriteLine("Thread is null!");
            }
        }
    }
}
