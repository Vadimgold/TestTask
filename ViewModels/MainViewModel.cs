using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TestTaskApp.Models;
using TestTaskApp.Pages;
using TestTaskApp.Services;
using TestTaskApp.Views;

namespace TestTaskApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<Movie> Movies { get; set; } = new();

        private readonly KinopoiskService _kinopoiskService = new();
        private readonly FavoriteService _favoriteService = new();

        private List<ApiMovie> _topMoviesCache;

        private string _yearFilter;


        [ObservableProperty]
        private bool isFavoritesView;

        public MainViewModel()
        {
            _ = LoadMoviesAsync();
        }

        public ObservableCollection<string> AvailableYears { get; } = new()
        {
            "2025", "2024", "2023", "2022", "2021", "2020"
        };

        public string YearFilter
        {
            get => _yearFilter;
            set
            {
                SetProperty(ref _yearFilter, value);
                ApplyYearFilter();
            }
        }

        private void ApplyYearFilter()
        {
            var all = IsFavoritesView
                    ? _favoriteService.GetAll().Select(f => new ApiMovie
                    {
                        kinopoiskId = f.Id,
                        NameRU = f.Title,
                        Year = f.Year,
                        PosterUrl = f.PosterUrl
                    }).ToList()
                    : _topMoviesCache ?? new();

            Movies.Clear();

            var filtered = all;
            if (int.TryParse(YearFilter, out var year))
            {
                filtered = all.Where(f => f.Year.HasValue && f.Year.Value == year).ToList();
            }

            foreach (var apiMovie in filtered)
            {
                Movies.Add(new Movie
                {
                    Id = apiMovie.kinopoiskId,
                    Title = apiMovie.NameRU,
                    Year = apiMovie.Year ?? 0,
                    PosterUrl = apiMovie.PosterUrl,
                    IsFavorite = _favoriteService.Contains(apiMovie.kinopoiskId)
                });
            }

            OnPropertyChanged(nameof(Movies));
        }

        [RelayCommand]
        private async Task DownloadAndSaveTopMoviesAsync()
        {
            YearFilter = string.Empty;
            var apiMovies = await _kinopoiskService.GetPopularMoviesAsync();

            var path = Path.Combine(Environment.CurrentDirectory, "TopFilms.json");

            var json = JsonSerializer.Serialize(apiMovies, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(path, json);

            _topMoviesCache = null;

            IsFavoritesView = false;
            await LoadMoviesAsync();
        }

        [RelayCommand]
        private async Task ShowPopularAsync()
        {
            YearFilter = string.Empty;
            IsFavoritesView = false;
            await LoadMoviesAsync();
        }


        [RelayCommand]
        private async Task ShowFavoritesAsync()
        {
            YearFilter = string.Empty;
            IsFavoritesView = true;
            await LoadMoviesAsync();
        }

        private async Task<List<ApiMovie>> GetTopMoviesCachedAsync()
        {
            if (_topMoviesCache != null)
                return _topMoviesCache;

            var path = Path.Combine(Environment.CurrentDirectory, "TopFilms.json");
            if (!File.Exists(path)) return new List<ApiMovie>();

            var json = await File.ReadAllTextAsync(path);
            _topMoviesCache = JsonSerializer.Deserialize<List<ApiMovie>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ApiMovie>();

            return _topMoviesCache;
        }

        [RelayCommand]
        private void ShowDetails(Movie movie)
        {
            //MessageBox.Show($"Переход к деталям фильма ID = {movie.Id}", "Отладка");
            var page = new DetailsPage(movie.Id);
            MainWindow.Instance.NavigateTo(page);
        }



        [RelayCommand]
        private void ToggleFavorite(Movie movie)
        {
            if (_favoriteService.Contains(movie.Id))
            {
                _favoriteService.Remove(movie.Id);
                movie.IsFavorite = false;
            }
            else
            {
                _favoriteService.Add(new FavoriteMovie
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    PosterUrl = movie.PosterUrl,
                    Year = movie.Year
                });
                movie.IsFavorite = true;
            }
        }

        private async Task LoadMoviesAsync()
        {
            Movies.Clear();

            if (IsFavoritesView)
            {
                var favorites = _favoriteService.GetAll();
                foreach (var fav in favorites)
                {
                    Movies.Add(new Movie
                    {
                        Id = fav.Id,
                        Title = fav.Title,
                        PosterUrl = fav.PosterUrl,
                        Year = fav.Year,
                        IsFavorite = true
                    });
                }
            }
            else
            {
                var apiMovies = await GetTopMoviesCachedAsync();
                int index = 1;

                foreach (var apiMovie in apiMovies)
                {
                    Movies.Add(new Movie
                    {
                        Id = apiMovie.kinopoiskId,
                        Title = apiMovie.NameRU,
                        Year = apiMovie.Year ?? 0,
                        PosterUrl = apiMovie.PosterUrl,
                        IsFavorite = _favoriteService.Contains(apiMovie.kinopoiskId)
                    });
                }
            }

            OnPropertyChanged(nameof(Movies));
        }

    }
}

