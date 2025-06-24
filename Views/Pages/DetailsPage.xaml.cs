using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TestTaskApp.Models;
using TestTaskApp.Services;


namespace TestTaskApp.Pages
{
    public partial class DetailsPage : Page
    {
        private readonly KinopoiskService _service = new();

        public DetailsPage(int movieId)
        {
            InitializeComponent();
            _ = LoadMovieAsync(movieId);
        }

        private async Task LoadMovieAsync(int id)
        {
            try
            {
                var movie = await _service.GetMovieDetailsAsync(id);

                if (movie == null)
                {
                    MessageBox.Show($"Не удалось загрузить данные для фильма с ID = {id}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DataContext = movie;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильма (ID = {id}):\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

    }
}