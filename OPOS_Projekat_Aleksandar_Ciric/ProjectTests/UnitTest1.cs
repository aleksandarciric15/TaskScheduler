using Scheduler;
using System.Threading.Tasks;

namespace ProjectTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ScheduleWithStarting() // adding task with starting 
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task1");
            fifo.Schedule(task);
            Assert.AreEqual(Scheduler.Task.JobState.Running, task.jobState);
        }

        [Test]
        public void ScheduleWithoutStarting()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task1 = new SimpleTask(10, "task1");
            fifo.Add(task1);
            Assert.AreEqual(Scheduler.Task.JobState.NotStarted, task1.jobState);
            fifo.StartScheduler();
        }

        [Test]
        public void SchedulingMoreTasks() // scheduling while others are executing
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task1 = new SimpleTask(10, "task1");
            SimpleTask task2 = new SimpleTask(10, "task2");
            fifo.Schedule(task1);
            Assert.IsTrue(task2.jobState == Scheduler.Task.JobState.NotStarted);
            fifo.Schedule(task2);
            task2.Finish();
            Assert.IsTrue(task2.jobState == Scheduler.Task.JobState.Finished);
            Thread.Sleep(4000);
            Assert.IsTrue(task1.jobState == Scheduler.Task.JobState.Finished);
        }

        [Test]
        public void StartingTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            fifo.Add(task);
            Assert.IsTrue(task.jobState == Scheduler.Task.JobState.NotStarted);
            fifo.StartScheduler();
            Assert.IsTrue(task.jobState == Scheduler.Task.JobState.Running);
        }

        [Test]
        public void PausingTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            fifo.Schedule(task);
            task.Wait();
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.WaitingToResume);
        }

        [Test]
        public void ContinuingTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            fifo.Schedule(task);
            task.Wait();
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.WaitingToResume);
            task.Resume();
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.Running);
        }

        [Test]
        public void CancelingTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            fifo.Schedule(task);
            task.Finish();
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.Finished);
        }

        [Test]
        public void WaitingTaskToCompleteTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            fifo.Schedule(task);
            task.thread.Join();
            Assert.IsTrue(task.jobState == Scheduler.Task.JobState.Finished);
        }

        [Test]
        public void FifoSchedulingTest()
        {
            FifoScheduler fifo = new FifoScheduler(2); 
            SimpleTask task1 = new SimpleTask(10, "task1");
            SimpleTask task2 = new SimpleTask(10, "task2");
            fifo.Schedule(task1);
            fifo.Schedule(task2);
            Assert.AreEqual(task1.jobState, Scheduler.Task.JobState.Running);
            Assert.AreEqual(task2.jobState, Scheduler.Task.JobState.Running);
        }

        [Test]
        public void PriorityScheduling()
        {
            PriorityScheduler priority = new PriorityScheduler(1);
            SimpleTask task1 = new SimpleTask(10, "task1"); task1.priority = 3;
            SimpleTask task2 = new SimpleTask(10, "task2"); task2.priority = 8;
            SimpleTask task3 = new SimpleTask(10, "task3"); task3.priority = 5;
            priority.Schedule(task1);
            priority.Schedule(task2);
            priority.Schedule(task3);
            Assert.AreEqual(task1.jobState, Scheduler.Task.JobState.Running);
            Thread.Sleep(4000);
            Assert.AreEqual(task1.jobState, Scheduler.Task.JobState.Finished);
            Assert.AreEqual(task2.jobState, Scheduler.Task.JobState.NotStarted); 
            Assert.AreEqual(task3.jobState, Scheduler.Task.JobState.Running);

        }

        [Test]
        public void StartTimeTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            task.startTime = new DateTime(2023, 9, 17, 13, 54, 50);
            fifo.waitStartTime(task);
            Thread.Sleep(2000);
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.NotStarted);
        }

        [Test]
        public void DurationTimeTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            task.durationTime = 1000;
            task.endTime = new DateTime(2023, 9, 20, 13, 54, 50);
            fifo.waitStartTime(task);
            Thread.Sleep(3000);
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.Finished);
        }

        [Test]
        public void EndTimeTaskTest()
        {
            FifoScheduler fifo = new FifoScheduler(1);
            SimpleTask task = new SimpleTask(10, "task");
            task.durationTime = 1000;
            task.endTime = new DateTime(2023, 9, 17, 14, 43, 50);
            fifo.waitStartTime(task);
            Thread.Sleep(1000);
            Assert.AreEqual(task.jobState, Scheduler.Task.JobState.Finished);
        }

        [Test]
        public void PreemptiveScheduling()
        {
            PreemptiveScheduler preemptiveScheduler = new PreemptiveScheduler(1);
            SimpleTask firstTask = new SimpleTask(8, "A");
            firstTask.priority = 4;
            SimpleTask secondTask = new SimpleTask(12, "B");
            secondTask.priority = 3;
            preemptiveScheduler.Schedule(firstTask);
            preemptiveScheduler.Schedule(secondTask);
            Thread.Sleep(2000);
            Assert.AreEqual(secondTask.jobState, Scheduler.Task.JobState.Running);
            Assert.AreEqual(firstTask.jobState, Scheduler.Task.JobState.Paused);
        }

        [Test]
        public void RoundRobinScheduling()
        {
            RoundRobinScheduler roundRobinScheduler = new RoundRobinScheduler(1);
            SimpleTask firstTask = new SimpleTask(10, "A");
            firstTask.priority = 5;
            roundRobinScheduler.Schedule(firstTask);
            SimpleTask secondTask = new SimpleTask(10, "B");
            secondTask.priority = 2;
            roundRobinScheduler.Schedule(secondTask);
            Thread.Sleep(2500);
            Assert.AreEqual(firstTask.jobState, Scheduler.Task.JobState.PausedWithTimeElapsed);
            Assert.AreEqual(secondTask.jobState, Scheduler.Task.JobState.Running);
        }

        [Test]
        public void TaskResourceTest1()
        {
            FifoScheduler fifo = new FifoScheduler(2);
            SimpleTask task1 = new SimpleTask(10, "A");
            SimpleTask task2 = new SimpleTask(10, "B");
            Resource r = new FileResource("resource");
            task1.AddResource(r);
            task2.AddResource(r);
            fifo.Schedule(task1);
            Thread.Sleep(100);
            fifo.Schedule(task2);
            Assert.AreEqual(task1.jobState, Scheduler.Task.JobState.Running);
            Assert.AreEqual(task2.jobState, Scheduler.Task.JobState.BlockedWaitingResource);
        }

        [Test]
        public void TaskResourceTest2()
        {
            PriorityScheduler fifo = new PriorityScheduler(2);
            SimpleTask task1 = new SimpleTask(10, "A"); task1.priority = 10;
            SimpleTask task2 = new SimpleTask(10, "C"); task2.priority = 1;
            Resource r = new FileResource("resource");
            task1.AddResource(r);
            task2.AddResource(r);
            fifo.Schedule(task1);
            Thread.Sleep(1000);
            fifo.Schedule(task2);
            Thread.Sleep(500);
            Assert.AreEqual(task1.priority, 1);
        }
    }
}