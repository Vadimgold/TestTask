using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using TestTaskApp.Models;
using TestTaskApp.Services;

namespace TestTaskApp.Views
{
    public partial class FilmDetailsWindow : Window
    {
        public FilmDetailsWindow(int filmId)
        {
            InitializeComponent();
            LoadDetails(filmId);
        }

        private async void LoadDetails(int id)
        {
            var service = new KinopoiskService();

            // дебаг для отладки временный
            string json = await service.GetMovieDetailsJsonAsync(id);

#if DEBUG
            {
                var path = Path.Combine(@"E:\Repos\TestTaskApp", $"debug-movie-{id}.json");

                try
                {
                    var pretty = JsonSerializer.Serialize(
                        JsonSerializer.Deserialize<JsonElement>(json),
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    await File.WriteAllTextAsync(path, pretty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении JSON:\n{ex.Message}");
                }
            }
#endif
            // конец

            var details = JsonSerializer.Deserialize<ApiMovieDetails>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (details != null)
            {
                DataContext = details;
            }
            else
            {
                MessageBox.Show("Не удалось загрузить детали фильма.");
                Close();
            }
        }
    }
}