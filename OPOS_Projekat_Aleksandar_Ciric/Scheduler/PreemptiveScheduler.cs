using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Scheduler
{
    public class PreemptiveScheduler : PriorityScheduler
    {

        public PreemptiveScheduler(int maxCurrentTasks) : base(maxCurrentTasks)
        {
        }

        public override void Schedule(Task task)
        {
            task.setActions(HandleJobFinished, HandleJobPaused, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
            if (tasks.Count < maxCurrentTasks)
            {
                tasks.Add(task);
                task.Start();
            }
            else
            {
                bool added = false;
                foreach (Task t in tasks)
                {
                    // cannot preempt a task with a lower priority if it has
                    // locked some resources in order to avoid a deadlock situation

                    if (t.priority > task.priority && !task.HasResources()) 
                    {
                        waitTasksPriority.Enqueue(task, task.priority);
                        t.Pause();
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

        protected new void HandleContinueRequested(Task task) 
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
                        bool added = false;
                        foreach (Task t in tasks)
                        {
                            if (t.priority > task.priority)
                            {
                                waitTasksPriority.Enqueue(task, task.priority);
                                t.Pause();
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
                    if (dequeuedTask.jobState == Task.JobState.Paused)
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
                }
                waitTasksPriority.Enqueue(task, task.priority);
            }
        }
    }
}
