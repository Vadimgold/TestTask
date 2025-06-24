using System.Windows.Controls;
using System.Windows;
using TestTaskApp.Models;
using TestTaskApp.Services;


namespace TestTaskApp.Pages
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Movie movie)
            {
                var vm = DataContext as ViewModels.MainViewModel;
                vm?.ToggleFavoriteCommand.Execute(movie);
            }
        }
    }
}
