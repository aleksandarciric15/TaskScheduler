using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class RoundRobinScheduler : PriorityScheduler
    {
        [JsonProperty]
        private const int timeSlice = 3000;

        public RoundRobinScheduler(int maxCurrentTasks) : base(maxCurrentTasks)
        {
        }

        public override void Schedule(Task task)
        {
            task.setActions(HandleJobFinished, HandleJobPaused, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
            if (tasks.Count < maxCurrentTasks)
            {
                tasks.Add(task);
                task.Start();
                timeElapsed(task);
            }
            else
            {
                bool added = false;
                foreach (Task t in tasks)
                {
                    if (t.priority > task.priority && !task.HasResources()) 
                    {
                        waitTasksPriority.Enqueue(task, task.priority);
                        t.PauseTimeElapsed();
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    waitTasksPriority.Enqueue(task, task.priority);
                }
            }
        }

        public new void Add(Task task)
        {
            task.setActions(HandleJobFinished, HandleJobPaused, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
            waitTasksPriority.Enqueue(task, task.priority);
        }

        public override void SchedulerSetActions(Task task)
        {
            task.setActions(HandleJobFinished, HandleJobPaused, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
        }

        private void timeElapsed(Task task)
        {
            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(timeSlice);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.StackTrace);
                }
                task.PauseTimeElapsed();
            }).Start();
        }

        private new void HandleJobFinished(Task task)
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
                    timeElapsed(dequeuedTask);
                }
            }
        }

        protected new void HandleContinueRequested(Task task) 
        {
            lock (schedulerLock)
            {
                if (PriorityQueueContainsTask(task))
                {
                    task.jobState = Task.JobState.PausedWithTimeElapsed;
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
                            timeElapsed(task);
                        }
                    }
                    else
                    {
                        task.jobState = Task.JobState.PausedWithTimeElapsed;
                        bool added = false;
                        foreach (Task t in tasks)
                        {
                            if (t.priority > task.priority)
                            {
                                waitTasksPriority.Enqueue(task, task.priority);
                                t.PauseTimeElapsed();
                                added = true;
                                break;
                            }
                        }
                        if (!added)
                        {
                            waitTasksPriority.Enqueue(task, task.priority);
                        }
                    }
                }
            }
        }

        protected new void HandleWaitingToResume(Task task)
        {
            lock (schedulerLock)
            {
                if (task.jobState == Task.JobState.Running)
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
                        if (dequeuedTask.jobState == Task.JobState.PausedWithTimeElapsed)
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
                        timeElapsed(dequeuedTask);
                    }
                }
            }
        }

        private void HandleJobPaused(Task task)
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
                    if (dequeuedTask.jobState == Task.JobState.PausedWithTimeElapsed)
                    {
                        lock (dequeuedTask)
                        {
                            dequeuedTask.jobState = Task.JobState.Running;
                            Monitor.Pulse(dequeuedTask);
                        }
                    }
                    else
                    {
                        dequeuedTask.Start();
                    }
                    timeElapsed(dequeuedTask);

                    waitTasksPriority.Enqueue(task, task.priority);
                }
                else
                {
                    if (task.jobState != Task.JobState.Finished)
                    {
                        lock (task)
                        {
                            task.jobState = Task.JobState.Running;
                            tasks.Add(task);
                            Monitor.Pulse(task);
                            timeElapsed(task);
                        }
                    }
                }
            }
        }
    }
}
