using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace TestTaskApp.Services
{
    public class KinopoiskService
    {
        private readonly HttpClient _http;

        public KinopoiskService()
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("https://kinopoiskapiunofficial.tech/");
            _http.DefaultRequestHeaders.Add("X-API-KEY", "URAPIKEY");
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<ApiMovie>> GetPopularMoviesAsync()
        {
            var allMovies = new List<ApiMovie>();
            int page = 1;

            while (allMovies.Count < 100)
            {
                var response = await _http.GetAsync($"api/v2.2/films/collections?type=TOP_POPULAR_ALL&page={page}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiCollectionResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Items == null || result.Items.Count == 0)
                    break;

                allMovies.AddRange(result.Items);
                page++;
            }

            return allMovies.Take(100).ToList();
        }

        public async Task<ApiMovieDetails?> GetMovieDetailsAsync(int id)
        {
            var response = await _http.GetAsync($"api/v2.2/films/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiMovieDetails>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            
        }

    }

    public class ApiMovie
    {
        public int kinopoiskId { get; set; }
        public string? NameRU { get; set; }
        public int? Year { get; set; }
        public string? PosterUrl { get; set; }

    }
    public class ApiMovieDetails
    {
        public int kinopoiskId { get; set; }
        public string NameRu { get; set; }
        public string Description { get; set; }

        public int year { get; set; }
        public string PosterUrl { get; set; }
        public double? RatingKinopoisk { get; set; }
        public int? FilmLength { get; set; }
        public List<Country> Countries { get; set; }
        public List<Genre> Genres { get; set; }
        public double? ratingGoodReview { get; set; }
        public double? ratingRfCritics { get; set; }
        public string? RatingAgeLimits { get; set; }
        public string ConvertAge
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RatingAgeLimits)) return "Отсутствует";
                if (RatingAgeLimits.StartsWith("age") && int.TryParse(RatingAgeLimits[3..], out int age))
                    return $"{age}+";
                return RatingAgeLimits;
            }
        }

    }
    public class ApiCollectionResponse
    {
        public List<ApiMovie> Items { get; set; }
    }


    public class Country
    {
        public string country { get; set; }
    }

    public class Genre
    {
        public string genre { get; set; }
    }
}
