using System;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestTaskApp.Models;
using TestTaskApp.Services;
using TestTaskApp.ViewModels;

namespace TestTaskApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly FavoriteService _favoriteService = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            TestGetMovieById();
        }
        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Movie movie)
            {
                if (movie.IsFavorite)
                    _favoriteService.Remove(movie.Id);
                else
                    _favoriteService.Add(FavoriteMovie.FromMovie(movie));

                movie.IsFavorite = !movie.IsFavorite; // обновление UI при добавлении в избранное и наоброт
            }
        }

        private async void TestGetMovieById()
        {
            var service = new KinopoiskService();

            int testId = 7527789; // test unmatch id
            try
            {
                var json = await service.GetMovieDetailsJsonAsync(testId);

                var details = JsonSerializer.Deserialize<ApiMovieDetails>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (details != null)
                {
                    MessageBox.Show(
                        $"Название: {details.NameRu}\n" +
                        $"Описание: {details.Description ?? "—"}\n" +
                        $"Год: {details.year}\n" +        
                        $"Рейтинг: {details.RatingKinopoisk ?? 0}");
                }
                else
                {
                    MessageBox.Show("Фильм не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запроса:\n{ex.Message}");
            }
        }

    }
}