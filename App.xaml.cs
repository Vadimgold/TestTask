using System.Windows;
using TestTaskApp.Pages;
using TestTaskApp.Views;

namespace TestTaskApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow();
            window.Show();


            window.MainFrame.Navigate(new MainPage());
        }
    }
}
