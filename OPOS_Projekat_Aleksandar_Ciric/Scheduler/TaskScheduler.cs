using Newtonsoft.Json;

namespace Scheduler
{
    [Serializable]
    public abstract class TaskScheduler
    {
        [JsonProperty]
        protected int maxCurrentTasks { get; set; }
        protected static object schedulerLock = new object();
        [JsonProperty]
        protected List<Resource> allResources = new List<Resource>();
        [JsonProperty]
        public HashSet<Task> tasks = new HashSet<Task>();
        [JsonProperty]
        public Queue<Task> waitTasks = new Queue<Task>();
        [JsonProperty]
        public PriorityQueue<Task, int> waitTasksPriority = new PriorityQueue<Task, int>();
        [JsonProperty]
        public List<Task> allTasks = new List<Task>();

        public TaskScheduler()
        {

        }

        public void AddResources(List<Resource> resources)
        {
            this.allResources = resources;
        }

        public abstract void Schedule(Task task);

        public abstract void ScheduleBlocked(Task task);

        public abstract void SchedulerSetActions(Task task);

        public abstract void Add(Task task);
        public void waitStartTime(Task task)
        {
            TimeSpan delay = task.startTime - DateTime.Now;
            new Thread(() =>
            {
                if (delay > TimeSpan.Zero)
                {
                    Thread.Sleep(delay);
                }
                Schedule(task);
            }).Start();
        }


        public void HandleResourceUnblocked(Task task)
        {
            lock (schedulerLock)
            {
                if (task.jobState == Task.JobState.BlockedWaitingResource)
                {
                    ScheduleBlocked(task);
                }
            }
        }

        public void HandleCancle(Task task)
        {
            lock (schedulerLock)
            {
                lock (task)
                {
                    task.jobState = Task.JobState.Finished;
                    Monitor.Pulse(task);
                }
            }
        }

        public void setMaxConcurentTasks(int maxConcurentTasks)
        {
            this.maxCurrentTasks = maxConcurentTasks;
        }

        public int getMaxConcurentTasks()
        {
            return this.maxCurrentTasks;
        }

        public virtual void Serialize()
        {
            string fileName = "Scheduler.json";
            if (File.Exists(fileName))
                File.Delete(fileName);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            string jsonString = JsonConvert.SerializeObject(this, settings);
            File.WriteAllText(fileName, jsonString);
        }

        public static TaskScheduler Deserialize()
        {
            string fileName = "Scheduler.json";
            string jsonString = File.ReadAllText(fileName);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };
            TaskScheduler scheduler = JsonConvert.DeserializeObject<TaskScheduler>(jsonString, settings);
            RestoreDataStructures(scheduler);
            return scheduler;
        }

        public static void RestoreDataStructures(TaskScheduler scheduler)
        {
            scheduler.tasks.Clear();
            foreach (var task in scheduler.allTasks)
            {
                if (task.jobState == Scheduler.Task.JobState.Running)
                {
                    scheduler.tasks.Add(task);
                }
            }
            if (scheduler.waitTasks.Count > 0)
            {
                scheduler.waitTasks.Clear();
                foreach (var task in scheduler.allTasks)
                {
                    if (task.jobState != Scheduler.Task.JobState.Running || task.jobState != Scheduler.Task.JobState.Finished)
                    {
                        scheduler.waitTasks.Enqueue(task);
                    }
                }
            }
            if (scheduler.waitTasksPriority.Count > 0)
            {
                scheduler.waitTasksPriority.Clear();
                foreach (var task in scheduler.allTasks)
                {
                    if (task.jobState != Scheduler.Task.JobState.Running || task.jobState != Scheduler.Task.JobState.Finished)
                    {
                        scheduler.waitTasksPriority.Enqueue(task, task.priority);
                    }
                }
            }
        }
    }
}