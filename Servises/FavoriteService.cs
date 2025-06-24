using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TestTaskApp.Models;


namespace TestTaskApp.Services
{
    public class FavoriteService
    {
        private readonly string _filePath;

        public FavoriteService()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestTaskApp");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "favorites.json");
        }

        public List<FavoriteMovie> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<FavoriteMovie>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<FavoriteMovie>>(json) ?? new List<FavoriteMovie>();
        }

        public void SaveAll(List<FavoriteMovie> movies)
        {
            var json = JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void Add(FavoriteMovie movie)
        {
            var list = GetAll();
            if (list.Exists(m => m.Id == movie.Id)) return;
            list.Add(movie);
            SaveAll(list);
        }

        public void Remove(int id)
        {
            var list = GetAll();
            list.RemoveAll(m => m.Id == id);
            SaveAll(list);
        }

        public bool Contains(int id)
        {
            return GetAll().Exists(m => m.Id == id);
        }
    }
}
