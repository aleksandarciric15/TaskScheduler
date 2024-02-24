using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUI;
using Ookii.Dialogs.Wpf;

namespace GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            schedulingType.Items.Add("Priority");
            schedulingType.Items.Add("FIFO");
            schedulingType.Items.Add("Preemptive");
            schedulingType.Items.Add("Round Robin");
            Saved();
        }

        public void Saved()
        {
            string path = Directory.GetCurrentDirectory();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles("*.json");

            foreach(FileInfo file in files)
            {
                if (file.Name.StartsWith("Scheduler"))
                {
                    if (MessageBox.Show("Do you want to restore previous version of task scheduler?", 
                        "Restore", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        TaskWindow tasksWindow = new();
                        this.Hide();
                        tasksWindow.Show();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (maxTasks.Text.Length != 0 || schedulingType.Text.Length != 0)
            {
                try
                {
                    int tasks = Int32.Parse(maxTasks.Text);
                    if (tasks < 0)
                    {
                        throw new Exception();
                    }

                    TaskWindow tasksWindow = new TaskWindow(tasks, schedulingType.Text);
                    this.Hide();
                    tasksWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid parameteres.", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Missing parameteres.", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
