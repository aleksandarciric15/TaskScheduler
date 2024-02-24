using Newtonsoft.Json;
using System.Diagnostics;

namespace Scheduler
{
    public abstract class Task
    {
        public enum JobState
        {
            NotStarted,
            Running,
            Finished,
            Paused,
            WaitingToResume,
            PausedWithTimeElapsed,
            BlockedWaitingResource
        }

        [JsonProperty]
        private int id;
        [JsonProperty]
        private static int serialNumber = 0;
        private readonly object taskLock = new();
        [JsonIgnore]
        public readonly object lockResource = new();
        public bool deadLockDetected = false;

        public JobState jobState = JobState.NotStarted;
        [JsonIgnore]
        public Thread thread = null;
        public int priority { get; set; }
        [JsonProperty]
        protected int degreeOfParallelism;
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int durationTime { get; set; } // ms

        public Stopwatch stopwatch = new Stopwatch();

        [JsonProperty]
        public double progressBarPercentage = 0;
        [JsonIgnore]
        public Action updateProgressBar { get; set; }
        [JsonIgnore]
        public Action progressBarFinshed { get; set; }
        [JsonIgnore]
        public Action progressBarStart { get; set; }

        [JsonIgnore]
        private Action<Task> onJobFinished;
        [JsonIgnore]
        private Action<Task> onJobPaused;
        [JsonIgnore]
        private Action<Task> onJobContinueRequested;
        [JsonIgnore]
        private Action<Task> onWaitingToResume;
        [JsonIgnore]
        private Action<Task> onJobCanceled;
        [JsonIgnore]
        private Action<Task> onJobUnblockedWaitingToResume;
        [JsonProperty]
        protected List<Resource> resources;
        protected static List<Resource> allResources = new List<Resource>();

        public Task()
        {
            id = ++serialNumber;
        }

        public void setActions(Action<Task> onJobFinished, Action<Task> onJobPaused, Action<Task> onJobContinueRequested,
            Action<Task> onWaitingToResume, Action<Task> onJobCanceled, Action<Task> onJobUnblockedWaitingToResume)
        {
            this.onJobFinished = onJobFinished;
            this.onJobPaused = onJobPaused;
            this.onJobContinueRequested = onJobContinueRequested;
            this.onWaitingToResume = onWaitingToResume;
            this.onJobCanceled = onJobCanceled;
            this.onJobUnblockedWaitingToResume = onJobUnblockedWaitingToResume;
        }

        public bool HasResources()
        {
            return resources.Count > 0;
        }

        public void AddResource(Resource resource)
        {
            resources.Add(resource);
            if (!allResources.Contains(resource))
            {
                allResources.Add(resource);
            }
        }

        protected void lockResources()
        {
            if (resources.Count > 0)
            {
                foreach (var res in resources)
                {
                    res.Lock(this);
                }
            }
        }

        protected void releaseResources()
        {
            if (resources.Count > 0)
            {
                foreach (var res in resources)
                {
                    res.Release();
                }
            }
        }

        public void ResourceBlock()
        {
            this.jobState = JobState.BlockedWaitingResource;
            lock (lockResource)
            {
                Monitor.Wait(lockResource);
            }
            onWaitingToResume(this);
        }

        public void ResourceUnblock()
        {
            lock (taskLock)
            {
                if (this.jobState == JobState.BlockedWaitingResource)
                {
                    onJobUnblockedWaitingToResume(this);
                    lock (lockResource)
                    {
                        Monitor.Pulse(lockResource);
                    }
                }
            }
        }

        public abstract void Run();
        public abstract void DefineThread();
        public void ThreadStart()
        {
            thread.Start();
        }

        public void Start()
        {
            lock (taskLock)
            { 
                switch (jobState)
                {
                    case JobState.NotStarted:
                        jobState = JobState.Running;
                        thread.Start();
                        break;
                    case JobState.Running:
                        throw new Exception();
                    default:
                        break;
                }
            }
        }


        public void Finish()
        {
            lock (taskLock)
            {
                switch (jobState)
                {
                    case JobState.NotStarted:
                        jobState = JobState.Finished;
                        break;
                    case JobState.WaitingToResume:
                    case JobState.Paused:
                        onJobCanceled(this);
                        break;
                    case JobState.PausedWithTimeElapsed:
                        jobState = JobState.Finished;
                        onJobCanceled(this);
                        progressBarFinshed();
                        break;
                    case JobState.Running:
                        jobState = JobState.Finished;
                        if (onJobFinished != null)
                            onJobFinished(this);
                        if (progressBarFinshed != null)
                            progressBarFinshed();
                        break;
                    case JobState.Finished:
                        throw new InvalidOperationException("Job already finished.");
                    default:
                        throw new InvalidOperationException("Invalid job state.");
                }
                if (deadLockDetected) // when the task completes, this allows a restart in case a deadlock is detected
                {
                    deadLockDetected = false;
                    progressBarStart();
                }
            }
        }

        public void Resume()
        {
            lock (taskLock)
            {
                switch (jobState)
                {
                    case JobState.WaitingToResume:
                        onJobContinueRequested(this);
                        break;
                    default:
                        break;
                }
            }
        }


        public void Pause()
        {
            lock (taskLock)
            {
                switch (jobState)
                {
                    case JobState.Running:
                        jobState = JobState.Paused;
                        onJobPaused(this);
                        break;
                    case JobState.Paused:
                        throw new InvalidOperationException();
                    default:
                        break;
                }
            }
        }

        public void PauseTimeElapsed()
        {
            lock (taskLock)
            {
                switch (jobState)
                {
                    case JobState.Running:
                        jobState = JobState.PausedWithTimeElapsed;
                        onJobPaused(this);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Wait()
        {
            lock (taskLock)
            {
                switch (jobState)
                {
                    case JobState.Paused:
                    case JobState.PausedWithTimeElapsed:
                    case JobState.NotStarted:
                    case JobState.Running:
                        onWaitingToResume(this);
                        jobState = JobState.WaitingToResume;
                        break;
                    default:
                        break;
                }
            }
        }


        public bool checkTime()
        {
            if (stopwatch.ElapsedMilliseconds > durationTime || DateTime.Now > endTime)
            {
                if (jobState != JobState.Finished)
                {
                    this.Finish();
                    return true;
                }
            }
            return false;
        }

        public void checkWaitingToResume()
        {
            lock (this)
            {
                if (jobState == JobState.WaitingToResume)
                {
                    Monitor.Wait(this);
                }
            }
        }

        public void checkPause()
        {
            lock (this)
            {
                if (jobState == JobState.Paused || jobState == JobState.PausedWithTimeElapsed)
                {
                    Monitor.Wait(this);
                }
            }
        }

        public virtual void Serialize()
        {
            string fileName = id + ".json";
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(fileName, jsonString);
        }

        public Task Deserialize()
        {
            string jsonString = File.ReadAllText(id + ".json");
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };
            return JsonConvert.DeserializeObject<Task>(jsonString, settings);
        }

        public override string ToString()
        {
            return id.ToString();
        }
    }
}
