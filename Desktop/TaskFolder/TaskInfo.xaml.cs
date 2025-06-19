using Desktop.Repository;
using Entities;
using System.Windows;
using System.Windows.Controls;

namespace Desktop.TaskFolder
{
    public partial class TaskInfo : UserControl
    {
        public delegate void DeleteTaskItemDel(TaskDictionary task);
        public event DeleteTaskItemDel? DeleteTaskItem;

        private TaskDictionary? Task;

        public TaskInfo()
        {
            InitializeComponent();
            DeleteB.Click -= DeleteB_Click;
            DoneB.Click -= DoneB_Click;
            DeleteB.Click += DeleteB_Click;
            DoneB.Click += DoneB_Click;
        }

        public void InfoLoad(TaskDictionary? Task)
        {
            this.Task = Task;
            InitializeTaskInfo();
        }

        private void InitializeTaskInfo()
        {
            if (Task == null) return;

            TitleItem.Text = Task.Name;
            TimeItem.Text = Task.Time;
            DateItem.Text = Task.Date;
            SubtitleItem.Text = Task.Description;
        }

        private async void DoneB_Click(object sender, RoutedEventArgs e)
        {
            if (Task != null)
            {
                Task.IsCompleted = !Task.IsCompleted;
                await TaskRepository.UpdateTaskDictionaryAsync(Task);
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteB_Click(object sender, RoutedEventArgs e)
        {
            if (Task != null)
            {
                DeleteTaskItem?.Invoke(Task);
            }
        }
    }
}
