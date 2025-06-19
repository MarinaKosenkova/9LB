using Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Desktop.TaskFolder
{
    public partial class TaskItem : UserControl
    {
        public TaskDictionary? Task;

        public TaskItem()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            ItemTaskBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eeedfe"));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ItemTaskBorder.Background = Brushes.White;
        }

        public void InfoLoad(TaskDictionary taskItem)
        {
            this.Task = taskItem;
            InitializeTaskInfo();
        }

        public void InitializeTaskInfo()
        {
            if (this.Task == null) return;

            finishedEllipseCanvas.Visibility = Visibility.Collapsed;
            unfinishedEllipse.Visibility = Visibility.Collapsed;
            TitleTask.Text = this.Task.Name;
            TimeTask.Text = this.Task.Time;

            if (this.Task.IsCompleted)
            {
                TitleTask.TextDecorations = TextDecorations.Strikethrough;
                TimeTask.TextDecorations = TextDecorations.Strikethrough;
                finishedEllipseCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                unfinishedEllipse.Visibility = Visibility.Visible;
            }
        }
    }
}
