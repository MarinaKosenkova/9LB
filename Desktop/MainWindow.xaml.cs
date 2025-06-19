using System.Windows;
using System.Windows.Media.Animation;
using Desktop.Repository;
using Desktop.View;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigating += MainFrame_Navigating;
            MainFrame.Navigated += MainFrame_Navigated;

            string token = LocalStorage.LoadToken();
            if (token != null)
            {
                MainFrame.Navigate(new MainEmpty("UserName", Guid.NewGuid()));
            }
            else
            {
                MainFrame.Navigate(new LogIn());
            }
        }

        private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            MainFrame.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            MainFrame.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            LocalStorage.ClearTasks();
            LocalStorage.ClearToken();
        }
    }
}
