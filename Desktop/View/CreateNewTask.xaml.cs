using Desktop.Repository;
using Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Desktop.View
{
    public partial class CreateNewTask : Page
    {
        private string name;
        private List<TaskDictionary> userTasks;
        private Guid userId;

        public CreateNewTask(string name, List<TaskDictionary> userTasks, Guid userId)
        {
            InitializeComponent();
            this.name = name;
            this.userTasks = userTasks;
            this.userId = userId;
        }

        private void TimePickerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"[\d:]"))
            {
                e.Handled = true;
            }
        }

        private void TimePickerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            string text = textBox.Text;

            if (text.Length == 2 && !text.Contains(":"))
            {
                textBox.Text += ":";
                textBox.CaretIndex = 3;
            }

            if (text.Count(c => c == ':') > 1)
            {
                textBox.Text = string.Empty;
                return;
            }

            var parts = text.Split(':');
            if (parts.Length > 2)
            {
                textBox.Text = string.Empty;
                return;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 2)
                {
                    textBox.Text = string.Empty;
                    return;
                }

                if (i == 0 && parts.Length == 2)
                {
                    if (int.TryParse(parts[i], out int hours) && (hours < 0 || hours > 23))
                    {
                        textBox.Text = string.Empty;
                        return;
                    }
                }

                if (i == 1 && parts.Length == 2)
                {
                    if (int.TryParse(parts[i], out int minutes) && (minutes < 0 || minutes > 59))
                    {
                        textBox.Text = string.Empty;
                        return;
                    }
                }
            }
        }

        private async void DoneNewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string titleText = TitleNewTaskTextBox.Text.Trim();
            if (string.IsNullOrEmpty(titleText))
            {
                MessageBox.Show("Укажите название задачи!", "Ошибка - пустое поле [Название задачи]", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string categoryText = CategoryNewTaskTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(categoryText) || categoryText.Contains(" "))
            {
                MessageBox.Show("Название категории не может содержать пробелы и быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DateNewTaskDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Указана неверная дата!", "Ошибка - неверный формат [Дата]", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string timeText = TimePickerTextBox.Text;
            if (!IsValidTimeFormat(timeText))
            {
                MessageBox.Show("Указан неверный формат времени!", "Ошибка - неверный формат [Время]", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                DateTime selectedDate = DateNewTaskDatePicker.SelectedDate.Value;
                MessageBox.Show($"Название: {titleText}\nКатегория: {categoryText}\nДата: {selectedDate.ToLongDateString()}\nВремя: {timeText}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                var newTask = new TaskDictionary
                {
                    Name = titleText,
                    Category = categoryText,
                    Date = selectedDate.ToShortDateString(),
                    Time = timeText,
                    IsCompleted = false,
                    UserId = userId // Убедитесь, что UserId установлен
                };

                await TaskRepository.AddTaskDictionaryAsync(newTask);

                userTasks.Add(newTask);
                this.NavigationService.Navigate(new Main(name, userTasks, userId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidTimeFormat(string time)
        {
            if (string.IsNullOrEmpty(time)) return false;

            var parts = time.Split(':');
            if (parts.Length != 2) return false;

            if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
            {
                return hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59;
            }

            return false;
        }

        private void CancelNewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Задача отменена!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.NavigationService.GoBack();
        }

        private void DeleteCategoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string categoryText = CategoryNewTaskTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(categoryText))
            {
                MessageBox.Show("Название категории не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            userTasks.RemoveAll(task => task.Category == categoryText);
            MessageBox.Show("Категория удалена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}