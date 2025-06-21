using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestTaskApp.Models;
using TestTaskApp.Services;
using TestTaskApp.Views;

namespace TestTaskApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<Movie> Movies { get; set; } = new();

        private readonly KinopoiskService _kinopoiskService = new();

        private bool showingFavorites;
        public string FavoritesButtonText => showingFavorites ? "Топ 100 фильмов" : "Избранное";


        [RelayCommand]
        private void ToggleFavoritesView()
        {
            showingFavorites = !showingFavorites;
            LoadMoviesAsync();
            OnPropertyChanged(nameof(FavoritesButtonText));
        }

        [RelayCommand]
        private void ShowDetails(Movie movie)
        {
            var win = new FilmDetailsWindow(movie.Id);
            win.ShowDialog();
        }
        public MainViewModel()
        {
            _ = LoadMoviesAsync();

        }

        private readonly FavoriteService _favoriteService = new();


        [RelayCommand]
        private void ToggleFavorite(Movie movie)
        {
            if (_favoriteService.Contains(movie.Id))
            {
                _favoriteService.Remove(movie.Id);
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
            }
            OnPropertyChanged(nameof(Movies));
        }

        public bool IsInFavorites(Movie movie)
        {
            return _favoriteService.Contains(movie.Id);
        }

        private async Task LoadMoviesAsync()
        {
            Movies.Clear();

            if (showingFavorites)
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
                var apiMovies = await _kinopoiskService.GetPopularMoviesAsync();

                foreach (var apiMovie in apiMovies)
                {
                    Movies.Add(new Movie
                    {
                        Id = apiMovie.FilmId,
                        Title = apiMovie.NameRU,
                        Year = int.TryParse(apiMovie.Year, out var year) ? year : 0,
                        PosterUrl = apiMovie.PosterUrl,
                        IsFavorite = _favoriteService.Contains(apiMovie.FilmId)
                    });
                }
            }

            OnPropertyChanged(nameof(Movies));
        }
    }
}
