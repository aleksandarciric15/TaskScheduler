using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class PriorityScheduler : TaskScheduler
    {

        public PriorityScheduler(int maxCurrentTasks)
        {
            this.maxCurrentTasks = maxCurrentTasks;
        }

        public override void Schedule(Task task)
        {
            task.setActions(HandleJobFinished, null, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
            if (tasks.Count < maxCurrentTasks)
            {
                tasks.Add(task);
                task.Start();
            }
            else
            {
                waitTasksPriority.Enqueue(task, task.priority);
            }
        }

        public override void Add(Task task)
        {
            task.setActions(HandleJobFinished, null, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
            waitTasksPriority.Enqueue(task, task.priority);
        }

        public override void SchedulerSetActions(Task task)
        {
            task.setActions(HandleJobFinished, null, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
        }

        public void StartScheduler()
        {
            for (int i = 0; i < maxCurrentTasks && waitTasksPriority.Count> 0; i++)
            {
                Task task = waitTasksPriority.Dequeue();
                task.Start();
            }
        }

        public override void ScheduleBlocked(Task task)
        {
            if (tasks.Count == maxCurrentTasks)
            {
                task.jobState = Task.JobState.Paused;
                waitTasksPriority.Enqueue(task, task.priority);
            }
            else
            {
                task.jobState = Task.JobState.Running;
                tasks.Add(task);
            }
        }

        protected void HandleJobFinished(Task task) 
        {
            lock (schedulerLock)
            {
                tasks.Remove(task);

                while (waitTasksPriority.Count > 0 && (waitTasksPriority.Peek().jobState == Task.JobState.WaitingToResume
                        || waitTasksPriority.Peek().jobState == Task.JobState.Finished))
                {
                    Task pomTask = waitTasksPriority.Dequeue(); 
                }

                if (waitTasksPriority.Count > 0) 
                {
                    Task dequeuedTask = waitTasksPriority.Dequeue();
                    tasks.Add(dequeuedTask);
                    if (dequeuedTask.jobState == Task.JobState.NotStarted)
                    {
                        dequeuedTask.Start();
                    }
                    else 
                    {
                        lock (dequeuedTask)
                        {
                            dequeuedTask.jobState = Task.JobState.Running;
                            Monitor.Pulse(dequeuedTask);
                        }
                    }
                }
            }
        }

        protected void HandleContinueRequested(Task task)
        {
            lock (schedulerLock)
            {
                if (PriorityQueueContainsTask(task))
                {
                    task.jobState = Task.JobState.Paused;
                }
                else
                {
                    if (waitTasksPriority.Count == 0 && tasks.Count < maxCurrentTasks)
                    {
                        tasks.Add(task);
                        lock (task)
                        {
                            task.jobState = Task.JobState.Running;
                            Monitor.Pulse(task);
                        }
                    }
                    else
                    {
                        task.jobState = Task.JobState.Paused;
                        waitTasksPriority.Enqueue(task, task.priority);
                    }
                }
            }
        }


        protected void HandleWaitingToResume(Task task)
        {
            lock (schedulerLock)
            {
                if (task.jobState == Task.JobState.Running && tasks.Contains(task))
                {
                    tasks.Remove(task);

                    while (waitTasksPriority.Count > 0 && (waitTasksPriority.Peek().jobState == Task.JobState.WaitingToResume
                        || waitTasksPriority.Peek().jobState == Task.JobState.Finished)) 
                    {
                        waitTasksPriority.Dequeue();
                    }

                    if (waitTasksPriority.Count > 0)
                    {
                        Task dequeuedTask = waitTasksPriority.Dequeue();
                        tasks.Add(dequeuedTask);
                        if (dequeuedTask.jobState == Task.JobState.Paused)
                        {
                            lock (dequeuedTask)
                            {
                                dequeuedTask.jobState = Task.JobState.Running;
                                Monitor.Pulse(dequeuedTask);
                            }
                        }
                        else if (dequeuedTask.jobState == Task.JobState.NotStarted)
                        {
                            dequeuedTask.Start();
                        }
                    }
                }
            }
        }

        protected bool PriorityQueueContainsTask(Task task)
        {
            for (int i = 0; i < waitTasksPriority.Count; i++)
            {
                Task temp = waitTasksPriority.Dequeue();
                if (temp.Equals(task))
                {
                    waitTasksPriority.Enqueue(temp, temp.priority);
                    return true; // PriorityQueue contains task
                }
                waitTasksPriority.Enqueue(temp, temp.priority);
            }
            return false;
        }
    }
}




/*
Creating MyQueue class that has Queue and PriorityQueue data structures.
And implements methods that are needed!
 */