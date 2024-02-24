using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace GUI
{
    public partial class ProgressBar : UserControl
    {
        private Scheduler.Task task;
        private Scheduler.TaskScheduler scheduler;
        public Action RemoveProgressBar { get; set; }
        public ProgressBar(Scheduler.Task task, Scheduler.TaskScheduler scheduler)
        {
            InitializeComponent();
            this.task = task;
            this.scheduler = scheduler;
            this.task.updateProgressBar = () =>     this.Dispatcher.Invoke(() => { taskPB.Value = task.progressBarPercentage; });
            this.task.progressBarFinshed = () => this.Dispatcher.Invoke(() => { disableAllButtons(); });
            this.task.progressBarStart = () => this.Dispatcher.Invoke(() => { setButtonsAtBegining(); });
            taskPB.Minimum = 0.0;
            taskPB.Maximum = 1.0;
            cancelBtn.IsEnabled = false;
            resumeBtn.IsEnabled = false;
            pauseBtn.IsEnabled = false;
        }


        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            scheduler.Schedule(task);
            startBtn.IsEnabled = false;
            removeBtn.IsEnabled = false;
            pauseBtn.IsEnabled = true;
            cancelBtn.IsEnabled = true;
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseBtn.IsEnabled = false;
            resumeBtn.IsEnabled = true;
            task.Wait();
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseBtn.IsEnabled = true;
            resumeBtn.IsEnabled = false;
            task.Resume();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            cancelBtn.IsEnabled = false;
            startBtn.IsEnabled = false;
            pauseBtn.IsEnabled = false;
            resumeBtn.IsEnabled = false;
            removeBtn.IsEnabled = true;
            task.Finish();
        }

        public void disableAllButtons()
        {
            cancelBtn.IsEnabled = false;
            startBtn.IsEnabled = false;
            pauseBtn.IsEnabled = false;
            resumeBtn.IsEnabled = false;
            removeBtn.IsEnabled = true;
        }

        public void setButtonsAtBegining()
        {
            startBtn.IsEnabled = true;
            cancelBtn.IsEnabled = false;
            resumeBtn.IsEnabled = false;
            pauseBtn.IsEnabled = false;
            removeBtn.IsEnabled = true;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            RemoveProgressBar();
        }
    }
}
