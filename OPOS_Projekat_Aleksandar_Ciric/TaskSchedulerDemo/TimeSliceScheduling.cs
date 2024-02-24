using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSchedulerDemo
{
    public class TimeSliceScheduling
    {
        public enum JobState
        {
            NotStarted,
            Running,
            Finished,
            Paused
        }

        private PriorityQueue<Task, int> waitTasks = new PriorityQueue<Task, int>();
        private HashSet<Task> tasks = new HashSet<Task>();
        private JobState jobState = JobState.NotStarted;
        private object lockTimer = new object();

        public TimeSliceScheduling() { }    

        public void Schedule()
        {

        }

        public void timeElapsed()
        {
            new Thread(() =>
            {
                Thread.Sleep(0);
                if (jobState != JobState.Finished)
                    jobState = JobState.Paused;
                Console.WriteLine(jobState);
            }).Start();
        }

        public void Run()
        {
            timeElapsed();
            for (int i=0; i < 100; i++)
            {
                // do something
            }
            jobState = JobState.Finished;
        }

        public static void Main(string[] args)
        {
            Singleton obj = Singleton.getInstance();
            Singleton obj2 = Singleton.getInstance();
            obj2.setVrijdenost(32);

            Console.WriteLine(obj.Equals(obj2));
            Console.WriteLine(obj == obj2);
        }
    }


    class Task
    {
        private  string name;

        public Task() { }

        public Task(string name)
        {
            this.name = name;
        }

    }

    class Singleton
    {
        private static Singleton instance;
        private int vrijednost = 10;
        private Singleton()
        {

        }

        public static Singleton getInstance()
        {
            if (instance == null)
                instance = new Singleton();
            return instance;
        }

        public int getVrijednost()
        {
            return vrijednost;
        }

        public void setVrijdenost(int vrijednost)
        {
            this.vrijednost = vrijednost;
        }
    }
}
