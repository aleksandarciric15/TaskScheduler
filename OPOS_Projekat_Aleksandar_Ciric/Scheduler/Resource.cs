using Newtonsoft.Json;

namespace Scheduler
{
    [Serializable]
    public abstract class Resource
    {
        [JsonProperty]
        public string? name { get;}
        [JsonProperty]
        private Task? owner = null;
        [JsonProperty]
        private int? thisPriority = null;
        [JsonProperty]
        private static DeadLockDetectorGraph graph = new DeadLockDetectorGraph();
        [JsonProperty]
        private PriorityQueue<Task, int> tasksWaiting = new PriorityQueue<Task, int>();

        public Resource()
        {
            this.name = null;
        }

        public Resource(string name)
        {
            this.name = name;
        }

        public Resource(string name, Task? owner)
        {
            this.name = name;
            this.owner = owner;
           
        }

        public abstract string getResource();

        public void Lock(Task task)
        {
            bool hasOwner = false;
            lock (this)
            {
                if (this.owner != null)
                {
                    graph.AddTransition(task, this.owner);
                    tasksWaiting.Enqueue(task, task.priority);
                    //PIP
                    if (task.priority < this.owner.priority)
                    {
                        thisPriority = this.owner.priority;
                        this.owner.priority = task.priority;
                    }
                    hasOwner = true;
                }
                else
                {
                    this.owner = task;
                }
            }
            if (hasOwner)
            {
                task.ResourceBlock();
                this.owner = task;
            }
        }


        public void Release()
        {
            lock (this)
            {
                if (this.owner != null)
                {
                    if (this.thisPriority != null)
                    {
                        this.owner.priority = (int) this.thisPriority;
                        this.thisPriority = null;
                    }
                }
                this.owner = null;
                if (tasksWaiting.Count > 0) 
                {
                    Task task = tasksWaiting.Dequeue(); 
                    graph.removeTransition(task);

                    task.ResourceUnblock();
                }
            }
        }

    }
}
