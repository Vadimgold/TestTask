namespace TestTaskApp.Models
{
    public class FavoriteMovie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public int Year { get; set; }
        public static FavoriteMovie FromMovie(Movie movie)
        {
            return new FavoriteMovie
            {
                Id = movie.Id,
                Title = movie.Title,
                PosterUrl = movie.PosterUrl,
                Year = movie.Year
            };
        }

    }
}
