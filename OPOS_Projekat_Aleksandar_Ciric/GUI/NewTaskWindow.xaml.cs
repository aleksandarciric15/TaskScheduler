using Scheduler;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GUI
{
    public partial class NewTaskWindow : Window
    {
        public Scheduler.Task task { get; set; }
        public NewTaskWindow()
        {
            InitializeComponent();
            typeOfTasks.Items.Add("SimpleTask");
            typeOfTasks.Items.Add("ImageSharpeningTask");
            taskPriority.ItemsSource = Enumerable.Range(1, 10);
        }

        // Adding resources
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                Multiselect = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var files = dialog.FileNames;
                files.ToList().ForEach(file => resourceLb.Items.Add(file.ToString()));
            }
        }

        // Adding output folder
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() == true)
                outputFolder.Content = dialog.SelectedPath;
        }

        // Creating new task
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (deadlineTime.SelectedDate == null || typeOfTasks.SelectedItem.ToString().Length == 0 || maxExecTime.Text.Length == 0 ||
                maxDegreeOfParallelism.Text.Length == 0 || taskPriority.Text.Length == 0)
            {
                System.Windows.MessageBox.Show("Missing parameteres.", "Error.", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }
            else
            {
                string? str = typeOfTasks.SelectedItem.ToString();
                try
                {
                    switch (str)
                    {
                        case "SimpleTask":
                            task = new SimpleTask(100, deadlineTime.SelectedDate.Value.ToString());
                            task.priority = Int32.Parse(taskPriority.Text);
                            task.durationTime = Int32.Parse(maxExecTime.Text) * 1000;
                            task.endTime = DateTime.Parse(deadlineTime.Text);
                            break;
                        case "ImageSharpeningTask":
                            if (resourceLb.Items.Count != 0 && outputFolder.Content.ToString().Length != 0)
                            {
                                List<Resource> resources = new();
                                foreach (string r in resourceLb.Items)
                                    resources.Add(new FileResource(r));
                                task = new ImageSharpeningTask(resources, outputFolder.Content.ToString(), Int32.Parse(maxDegreeOfParallelism.Text));
                                task.priority = Int32.Parse(taskPriority.Text);
                                task.durationTime = Int32.Parse(maxExecTime.Text) * 1000;
                                task.endTime = DateTime.Parse(deadlineTime.Text);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Missing parameteres.", "Error.", MessageBoxButton.OK, MessageBoxImage.Error); return;
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }
                catch (Exception ex) 
                {
                    System.Windows.MessageBox.Show("Invalid parameteres." + ex, "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                System.Windows.MessageBox.Show("Task added.", "Information.", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Hide();
            }
        }
    }
}
