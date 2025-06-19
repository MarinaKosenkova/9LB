using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Desktop.Repository;
using Entities;

namespace Desktop.View
{
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();

            usernameBox.Foreground = Brushes.Gray;
            mailBox.Foreground = Brushes.Gray;
            passBlock.Foreground = Brushes.Gray;
            pass2Block.Foreground = Brushes.Gray;
        }

        private void UsernameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text == "Введите имя пользователя" && usernameBox.Foreground == Brushes.Gray)
            {
                usernameBox.Text = "";
                usernameBox.Foreground = Brushes.Black;
            }
        }

        private void UsernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text == "")
            {
                usernameBox.Foreground = Brushes.Gray;
                usernameBox.Text = "Введите имя пользователя";
            }
        }

        private void MailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mailBox.Text == "exam@yandex.ru" && mailBox.Foreground == Brushes.Gray)
            {
                mailBox.Text = "";
                mailBox.Foreground = Brushes.Black;
            }
        }

        private void MailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (mailBox.Text == "")
            {
                mailBox.Foreground = Brushes.Gray;
                mailBox.Text = "exam@yandex.ru";
            }
        }

        private void PassPASSwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passPASSwordBox.Password == "")
                passBlock.Visibility = Visibility.Hidden;
        }

        private void PassPASSwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passPASSwordBox.Password == "")
                passBlock.Visibility = Visibility.Visible;
        }

        private void PassPASSwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passPASSwordBox.Password != "")
                passBlock.Visibility = Visibility.Hidden;
            else
                passBlock.Visibility = Visibility.Visible;
        }

        private void Pass2PASSwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pass2PASSwordBox.Password == "")
                pass2Block.Visibility = Visibility.Hidden;
        }

        private void Pass2PASSwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pass2PASSwordBox.Password == "")
                pass2Block.Visibility = Visibility.Visible;
        }

        private void Pass2PASSwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pass2PASSwordBox.Password != "")
                pass2Block.Visibility = Visibility.Hidden;
            else
                pass2Block.Visibility = Visibility.Visible;
        }

        private void NazadB_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LogIn());
        }

        private async void RegistrationB_Click(object sender, RoutedEventArgs e)
        {
            InputValidator inputValidator = new();
            string mail = mailBox.Text;
            string name = usernameBox.Text;
            string password = passPASSwordBox.Password;

            inputValidator.IsValidEmail(mail);
            inputValidator.IsValidName(name);
            inputValidator.IsValidPassword(password);

            if (!inputValidator.IsValidName(name) || name == "Введите имя пользователя")
            {
                MessageBox.Show("Имя пользователя слишком короткое!\nИмя пользователя обязательно должно содержать не менее трех символов.", "Ошибка регистрации [Имя пользователя]", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!inputValidator.IsValidEmail(mail) || mailBox.Foreground == Brushes.Gray && mailBox.Text == "exam@yandex.ru")
                MessageBox.Show("Указан неверный формат Почты!\nПример правильной Почты: example@mail.ru", "Ошибка регистрации [Почта]", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!inputValidator.IsValidPassword(password))
                MessageBox.Show("Слишком короткий Пароль!\nПароль обязательно должен содержать не менее шести символов.", "Ошибка регистрации [Пароль]", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (password != pass2PASSwordBox.Password)
                MessageBox.Show("Пароли должны совпадать!", "Ошибка регистрации [Пароль]", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var apiClient = new ApiClient("http://45.144.64.179");
                try
                {
                    var newUser = new UserModel
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Email = mail,
                        Password = password
                    };
                    await apiClient.PostAsync<UserModel>("/api/users", newUser);
                    this.NavigationService.Navigate(new MainEmpty(name, newUser.Id));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
