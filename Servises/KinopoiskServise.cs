using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestTaskApp.Services
{
    public class KinopoiskService
    {
        private readonly HttpClient _http;

        public KinopoiskService()
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("https://kinopoiskapiunofficial.tech/");
            _http.DefaultRequestHeaders.Add("X-API-KEY", "ApiKeyTempHolder");
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<ApiMovie>> GetPopularMoviesAsync()
        {
            var response = await _http.GetAsync("api/v2.2/films/top?type=TOP_100_POPULAR_FILMS&page=1");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Films ?? new List<ApiMovie>();
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
        // temp for json
        public async Task<string> GetMovieDetailsJsonAsync(int id)
        {
            var response = await _http.GetAsync($"api/v2.2/films/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class ApiResponse
    {
        public List<ApiMovie> Films { get; set; }
    }

    public class ApiMovie
    {
        public int FilmId { get; set; }
        public string NameRU { get; set; }
        public string Year { get; set; }
        public string PosterUrl { get; set; }
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

    public class Country
    {
        public string country { get; set; }
    }

    public class Genre
    {
        public string genre { get; set; }
    }
}
