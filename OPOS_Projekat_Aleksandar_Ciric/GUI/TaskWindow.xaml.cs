using Scheduler;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace GUI
{
    public partial class TaskWindow : Window
    {
        private Scheduler.TaskScheduler scheduler;
        private int autoSaveTime = 60_000; // 1 min
        public TaskWindow()
        {
            InitializeComponent();
            scheduler = Scheduler.TaskScheduler.Deserialize();
            RestoreWindow();
            new Thread(AutoSave) { IsBackground = true }.Start();
        }

        private void RestoreWindow()
        {
            foreach(Scheduler.Task task in scheduler.allTasks)
                RestoreTask(task);
        }

        public void RestoreTask(Task task)
        {
            if (task == null)
                System.Windows.MessageBox.Show("Task is null.", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
            ProgressBar taskProgressBar = new ProgressBar(task, scheduler);
            scheduler.SchedulerSetActions(task);
            if (task.thread != null)
            {
                Console.WriteLine();
            }
            if (task.jobState == Scheduler.Task.JobState.NotStarted)
            {
                taskProgressBar.startBtn.IsEnabled = true;
                taskProgressBar.removeBtn.IsEnabled = true;
                taskProgressBar.pauseBtn.IsEnabled = false;
                taskProgressBar.resumeBtn.IsEnabled = false;
                taskProgressBar.cancelBtn.IsEnabled = false;
                task.DefineThread();
            }
            else if (task.jobState == Scheduler.Task.JobState.Running)
            {
                taskProgressBar.startBtn.IsEnabled = false;
                taskProgressBar.removeBtn.IsEnabled = false;
                taskProgressBar.pauseBtn.IsEnabled = true;
                taskProgressBar.resumeBtn.IsEnabled = false;
                taskProgressBar.cancelBtn.IsEnabled = true;
                task.jobState = Scheduler.Task.JobState.NotStarted;
                task.Start();
            }
            else if (task.jobState == Scheduler.Task.JobState.WaitingToResume)
            {
                taskProgressBar.startBtn.IsEnabled = false;
                taskProgressBar.removeBtn.IsEnabled = false;
                taskProgressBar.pauseBtn.IsEnabled = false;
                taskProgressBar.resumeBtn.IsEnabled = true;
                taskProgressBar.cancelBtn.IsEnabled = true;
                task.DefineThread();
                task.ThreadStart();
            }
            else if (task.jobState == Scheduler.Task.JobState.Finished)
            {
                taskProgressBar.startBtn.IsEnabled = false;
                taskProgressBar.removeBtn.IsEnabled = true;
                taskProgressBar.pauseBtn.IsEnabled = false;
                taskProgressBar.resumeBtn.IsEnabled = false;
                taskProgressBar.cancelBtn.IsEnabled = false;
                task.DefineThread();
            }
            else
            {
                taskProgressBar.startBtn.IsEnabled = false;
                taskProgressBar.removeBtn.IsEnabled = false;
                taskProgressBar.pauseBtn.IsEnabled = true;
                taskProgressBar.resumeBtn.IsEnabled = false;
                taskProgressBar.cancelBtn.IsEnabled = true;
                task.DefineThread();
                task.ThreadStart();
            }
            taskProgressBar.RemoveProgressBar = () => tasksSP.Children.Remove(taskProgressBar);
            taskProgressBar.taskPB.Visibility = Visibility.Visible;
            tasksSP.Children.Add(taskProgressBar);
        }

        public TaskWindow(int numbTasks, string type)
        {
            InitializeComponent();
            switch(type)
            {
                case "FIFO":
                    scheduler = new FifoScheduler(numbTasks);
                    break;
                case "Preemptive":
                    scheduler = new PreemptiveScheduler(numbTasks);
                    break;
                case "Priority":
                    scheduler = new PriorityScheduler(numbTasks);
                    break;
                case "Round Robin":
                    scheduler = new RoundRobinScheduler(numbTasks);
                    break;
                default:
                    Console.WriteLine("Podaci nisu validni!");
                    break;
            }
            new Thread(AutoSave) { IsBackground = true }.Start();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewTaskWindow window = new NewTaskWindow();
            if (window.ShowDialog() == false && window.task != null)
            {
                AddTaskToStackPanel(window.task);
                scheduler.allTasks.Add(window.task);
            }
        }

        private void AddTaskToStackPanel(Scheduler.Task task)
        {
            if (task == null)
                System.Windows.MessageBox.Show("Task is null.", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
            ProgressBar taskProgressBar = new ProgressBar(task, scheduler);

            taskProgressBar.RemoveProgressBar = () => tasksSP.Children.Remove(taskProgressBar);
            taskProgressBar.taskPB.Visibility = Visibility.Visible;
            tasksSP.Children.Add(taskProgressBar);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            scheduler.Serialize();
        }

        public void AutoSave()
        {
            while (true)
            {
                Save();
                Thread.Sleep(autoSaveTime);
            }
        }

    }
}
