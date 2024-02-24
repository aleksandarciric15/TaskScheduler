using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class FifoScheduler : TaskScheduler
    {
        public FifoScheduler(int maxCurrentTasks)
        {
            this.maxCurrentTasks = maxCurrentTasks;
        }

        public override void Schedule(Task task)
        {
            lock (schedulerLock)
            {
                task.setActions(HandleJobFinished, null, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
                if (tasks.Count < maxCurrentTasks)
                {
                    tasks.Add(task); 
                    task.Start(); 
                }
                else
                {
                    waitTasks.Enqueue(task); 
                }
            }
        }

        public void StartScheduler()
        {
            for (int i = 0; i < maxCurrentTasks && waitTasks.Count>0; i++)
            {
                Task task = waitTasks.Dequeue();
                task.Start();
            }
        }

        public override void Add(Task task)
        {
            task.setActions(HandleJobFinished, null, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
            waitTasks.Enqueue(task);
        }

        public override void SchedulerSetActions(Task task)
        {
            task.setActions(HandleJobFinished, null, HandleContinueRequested, HandleWaitingToResume, HandleCancle, HandleResourceUnblocked);
        }

        public override void ScheduleBlocked(Task task)
        {
            if (tasks.Count == maxCurrentTasks)
            {
                task.jobState = Task.JobState.Paused;
                waitTasks.Enqueue(task);
            }
            else
            {
                task.jobState = Task.JobState.Running;
                tasks.Add(task);
            }
        }

        private void HandleJobFinished(Task task)
        {
            lock (schedulerLock)
            {
                tasks.Remove(task);
                
                while (waitTasks.Count > 0 && (waitTasks.Peek().jobState == Task.JobState.WaitingToResume
                        || waitTasks.Peek().jobState == Task.JobState.Finished))
                {
                    Task pomTask = waitTasks.Dequeue();
                }

                if (waitTasks.Count > 0) 
                {
                    Task dequeuedTask = waitTasks.Dequeue();
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
        

        public void HandleContinueRequested(Task task)
        {
            lock (schedulerLock)
            {
                if (waitTasks.Contains(task))
                {
                    task.jobState = Task.JobState.Paused;
                }
                else
                {
                    if (waitTasks.Count == 0 && tasks.Count < maxCurrentTasks)
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
                        waitTasks.Enqueue(task);
                    }
                }
            }
        }


        public void HandleWaitingToResume(Task task)
        {
            lock (schedulerLock)
            {
                if (task.jobState == Task.JobState.Running && tasks.Contains(task))
                {
                    tasks.Remove(task);

                    while (waitTasks.Count > 0 && (waitTasks.Peek().jobState == Task.JobState.WaitingToResume
                        || waitTasks.Peek().jobState == Task.JobState.Finished)) 
                    {
                        waitTasks.Dequeue();
                    }

                    if (waitTasks.Count > 0)
                    {
                        Task dequeuedTask = waitTasks.Dequeue();
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
    }
}
