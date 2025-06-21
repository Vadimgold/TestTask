using CommunityToolkit.Mvvm.ComponentModel;

namespace TestTaskApp.Models
{
    public partial class Movie : ObservableObject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string PosterUrl { get; set; }

        [ObservableProperty]
        private bool isFavorite;

    }
}

