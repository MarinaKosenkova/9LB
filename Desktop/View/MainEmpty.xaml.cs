using Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Desktop.View
{
    public partial class MainEmpty : Page
    {
        private string userName;
        private List<TaskDictionary> userTasks;
        private Guid userId;

        public MainEmpty(string userName, Guid userId)
        {
            InitializeComponent();
            this.userName = userName;
            UserNameBlock.Text = userName;
            userTasks = new List<TaskDictionary>();
            this.userId = userId;
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

        private void zada4aB_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateNewTask(userName, userTasks, userId));
        }
    }
}
