using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Desktop.Repository;
using Desktop.TaskFolder;
using Entities;
using System.Windows.Media.Effects;
using System.Threading.Tasks;

namespace Desktop.View
{
    public partial class Main : Page
    {
        private bool showCompletedTasks = false;
        private List<TaskDictionary> userTasks;
        private string userName;
        private string selectedCategory = "All";
        private Guid userId;

        public Main(string name, List<TaskDictionary> tasks, Guid userId)
        {
            InitializeComponent();
            userName = name;
            UserNameBlock.Text = name;
            userTasks = tasks;
            this.userId = userId;
            InitializeTasks(userTasks);
            InitializeCategories();

            TaskRepository.TaskItemsChanged += RefreshTasks;
        }

        private void InitializeCategories()
        {
            var categories = userTasks.Select(t => t.Category).Distinct().ToList();
            foreach (var category in categories)
            {
                if (!CategorySPanel.Children.Cast<TextBlock>().Any(tb => tb.Text == category))
                {
                    var textBlock = new TextBlock
                    {
                        Text = category,
                        FontSize = 18,
                        Margin = new Thickness(10, 0, 0, 0),
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.DemiBold
                    };
                    textBlock.MouseDown += CategoryTextBlock_MouseDown;
                    CategorySPanel.Children.Add(textBlock);
                }
            }
        }

        private async void InitializeTasks(List<TaskDictionary> taskItems)
        {
            ItemsTasksSPanel.Children.Clear();

            foreach (TaskDictionary task in taskItems)
            {
                if ((showCompletedTasks && task.IsCompleted) || (!showCompletedTasks && !task.IsCompleted && (selectedCategory == "All" || task.Category == selectedCategory)))
                {
                    var item = new TaskItem();
                    item.InfoLoad(task);
                    item.Margin = new Thickness(10);
                    item.Effect = new DropShadowEffect
                    {
                        ShadowDepth = 5,
                        Direction = -90,
                        BlurRadius = 20,
                        Color = Colors.Gray,
                    };

                    switch (task.Category)
                    {
                        case "Дом":
                            item.TitleTask.Foreground = Brushes.Green;
                            break;
                        case "Работа":
                            item.TitleTask.Foreground = Brushes.Orange;
                            break;
                        case "Учеба":
                            item.TitleTask.Foreground = Brushes.Blue;
                            break;
                        case "Отдых":
                            item.TitleTask.Foreground = Brushes.Purple;
                            break;
                        default:
                            item.TitleTask.Foreground = Brushes.Gray;
                            break;
                    }

                    item.MouseDown += TaskItem_MouseDown;
                    ItemsTasksSPanel.Children.Add(item);
                }
            }
        }

        private async void TaskItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var taskItem = (TaskItem)sender;
            if (taskItem == null || taskItem.Task == null) return;

            var item = taskItem.Task;

            TaskI.Visibility = Visibility.Visible;
            var itemInfo = new TaskInfo();
            itemInfo.InfoLoad(item);

            TaskInfoBlock.Children.Clear();
            TaskInfoBlock.Children.Add(itemInfo);
            itemInfo.DeleteTaskItem += async (deletedTask) =>
            {
                TaskInfoBlock.Children.Clear();
                await TaskRepository.RemoveTaskDictionaryAsync(deletedTask);
                userTasks.Remove(deletedTask);
                InitializeTasks(userTasks);
                InitializeCategories();
            };
        }

        private void ProfileImageSwitch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image Files (*.png;*.jpg)| *.png;*.jpg",
                Title = "Выберите изображение профиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                profileImageButton.Source = bitmap;
            }
        }

        private void ProfileImageButton_MouseEnter(object sender, MouseEventArgs e)
        {
            menuGrid.Visibility = Visibility.Visible;
        }

        private void MenuGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            menuGrid.Visibility = Visibility.Hidden;
        }

        private void ExitB_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LogIn());
        }

        private void CreateNewTaskB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new CreateNewTask(userName, userTasks, userId));
        }

        private void TasksTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            showCompletedTasks = false;
            selectedCategory = "All";
            InitializeTasks(userTasks);
        }

        private void HistoryTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            showCompletedTasks = true;
            selectedCategory = "All";
            InitializeTasks(userTasks);
        }

        private void CategoryTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var categoryTextBlock = (TextBlock)sender;
                var categoryName = categoryTextBlock.Text;

                var contextMenu = new ContextMenu();

                var deleteMenuItem = new MenuItem { Header = "Удалить категорию" };
                deleteMenuItem.Click += (s, args) => DeleteCategory(categoryName);

                if (categoryName == "Дом" || categoryName == "Отдых" || categoryName == "Работа" || categoryName == "Учеба")
                {
                    deleteMenuItem.IsEnabled = false;
                }

                contextMenu.Items.Add(deleteMenuItem);

                categoryTextBlock.ContextMenu = contextMenu;
                contextMenu.IsOpen = true;
            }
            else
            {
                showCompletedTasks = false;
                selectedCategory = ((TextBlock)sender).Text;
                InitializeTasks(userTasks);
            }
        }

        private void DeleteCategory(string categoryName)
        {
            userTasks.RemoveAll(task => task.Category == categoryName);
            InitializeTasks(userTasks);

            var textBlockToRemove = CategorySPanel.Children.Cast<TextBlock>().FirstOrDefault(tb => tb.Text == categoryName);
            if (textBlockToRemove != null)
            {
                CategorySPanel.Children.Remove(textBlockToRemove);
            }

            MessageBox.Show("Категория удалена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void RefreshTasks()
        {
            userTasks = await TaskRepository.GetTaskReposesAsync(userId);
            InitializeTasks(userTasks);
        }
    }
}