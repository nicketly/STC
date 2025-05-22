using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using STC.WPF.Models;

namespace STC.WPF.Database
{
    public static class DatabaseInitializer
    {
        private const string DbFileName = "materials.db";
        private const string ConnectionString = "Data Source=materials.db;Version=3;";

        static DatabaseInitializer()
        {
            EnsureDatabaseCreated();
        }

        public static void EnsureDatabaseCreated()
        {
            if (!File.Exists(DbFileName))
            {
                SQLiteConnection.CreateFile(DbFileName);

                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    var createTableCommand = new SQLiteCommand(@"
                        CREATE TABLE IF NOT EXISTS Materials (
                            Name TEXT NOT NULL,
                            Category TEXT,
                            MaxTemp REAL,
                            TCF TEXT
                        );
                    ", connection);
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        public static List<Material> LoadMaterials()
        {
            var materials = new List<Material>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT Name, Category, MaxTemp, TCF FROM Materials", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    materials.Add(new Material
                    {
                        Name = reader["Name"].ToString(),
                        Category = reader["Category"].ToString(),
                        MaxTemp = double.TryParse(reader["MaxTemp"].ToString(), out var maxTemp) ? maxTemp : 0,
                        TCF = reader["TCF"].ToString()
                    });
                }
            }

            return materials;
        }

        public static void SaveMaterials(List<Material> materials)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                var deleteCommand = new SQLiteCommand("DELETE FROM Materials", connection);
                deleteCommand.ExecuteNonQuery();

                foreach (var material in materials)
                {
                    var insertCommand = new SQLiteCommand("INSERT INTO Materials (Name, Category, MaxTemp, TCF) VALUES (@Name, @Category, @MaxTemp, @TCF)", connection);
                    insertCommand.Parameters.AddWithValue("@Name", material.Name);
                    insertCommand.Parameters.AddWithValue("@Category", material.Category);
                    insertCommand.Parameters.AddWithValue("@MaxTemp", material.MaxTemp);
                    insertCommand.Parameters.AddWithValue("@TCF", material.TCF);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
