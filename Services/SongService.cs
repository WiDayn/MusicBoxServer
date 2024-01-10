using MusicBoxServer.Dtos;
using MusicBoxServer.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace MusicBoxServer.Services
{
    public class SongService
    {
        private readonly IConfiguration _configuration;

        public SongService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }

        public async Task<List<Song>> GetAllSongsAsync()
        {
            var songs = new List<Song>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM songs", conn);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        songs.Add(new Song
                        {
                            SongID = reader.GetInt32("SongID"),
                            Title = reader.GetString("Title"),
                            AlbumID = reader.GetInt32("AlbumID"),
                            Duration = TimeSpan.FromSeconds(reader.GetInt32("Duration")),
                            Genre = reader.GetString("Genre"),
                            BitRate = reader.GetInt32("BitRate"),
                            ViewCount = reader.GetInt32("ViewCount")
                        });
                    }
                }
            }

            return songs;
        }

        public async Task<Song> GetSongByIdAsync(int songId)
        {
            Song song = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM songs WHERE SongID = @SongID", conn);
                command.Parameters.AddWithValue("@SongID", songId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        song = new Song
                        {
                            SongID = reader.GetInt32("SongID"),
                            Title = reader.GetString("Title"),
                            AlbumID = reader.GetInt32("AlbumID"),
                            Duration = TimeSpan.FromSeconds(reader.GetInt32("Duration")),
                            Genre = reader.GetString("Genre"),
                            BitRate = reader.GetInt32("BitRate"),
                            ViewCount = reader.GetInt32("ViewCount")
                        };
                    }
                }
            }

            return song;
        }

        public async Task AddSongAsync(Song song)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("INSERT INTO songs (Title, AlbumID, Duration, FilePath, Genre, BitRate, ViewCount) VALUES (@Title, @AlbumID, @Duration, @Genre, @BitRate, @ViewCount)", conn);
                command.Parameters.AddWithValue("@Title", song.Title);
                command.Parameters.AddWithValue("@AlbumID", song.AlbumID);
                command.Parameters.AddWithValue("@Duration", song.Duration.TotalSeconds);
                command.Parameters.AddWithValue("@Genre", song.Genre);
                command.Parameters.AddWithValue("@BitRate", song.BitRate);
                command.Parameters.AddWithValue("@ViewCount", song.ViewCount);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateSongAsync(Song song)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("UPDATE songs SET Title = @Title, AlbumID = @AlbumID, Duration = @Duration, Genre = @Genre, BitRate = @BitRate, ViewCount = @ViewCount WHERE SongID = @SongID", conn);
                command.Parameters.AddWithValue("@SongID", song.SongID);
                command.Parameters.AddWithValue("@Title", song.Title);
                command.Parameters.AddWithValue("@AlbumID", song.AlbumID);
                command.Parameters.AddWithValue("@Duration", song.Duration.TotalSeconds);
                command.Parameters.AddWithValue("@Genre", song.Genre);
                command.Parameters.AddWithValue("@BitRate", song.BitRate);
                command.Parameters.AddWithValue("@ViewCount", song.ViewCount);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task DeleteSongAsync(int songId)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("DELETE FROM songs WHERE SongID = @SongID", conn);
                command.Parameters.AddWithValue("@SongID", songId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<string> GetLyricsAsync(int songId)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                var command = new MySqlCommand("SELECT Lyrics FROM songs WHERE SongID = @songId", connection);
                command.Parameters.AddWithValue("@songId", songId);

                var lyrics = await command.ExecuteScalarAsync() as string;
                return lyrics ?? string.Empty;
            }
        }

        public async Task<List<SearchSong>> SearchSongsAsync(string keyword)
        {
            var songs = new List<SearchSong>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand(@"
            SELECT 
                s.SongID, s.Title, s.AlbumID, s.Duration, s.Genre, s.Bitrate, s.ViewCount, s.TrackNumber,
                a.Title AS AlbumTitle, ar.Name AS ArtistName, ar.ArtistID
            FROM 
                songs s
            LEFT JOIN 
                albums a ON s.AlbumID = a.AlbumID
            LEFT JOIN 
                artists ar ON a.ArtistID = ar.ArtistID
            WHERE 
                s.Title LIKE @Keyword OR 
                a.Title LIKE @Keyword OR 
                ar.Name LIKE @Keyword", conn);

                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var song = new SearchSong
                        {
                            SongID = reader.GetInt32("SongID"),
                            Title = reader.GetString("Title"),
                            AlbumID = reader.GetInt32("AlbumID"),
                            ArtistID = reader.GetInt32("ArtistID"),
                            Duration = TimeSpan.FromSeconds(reader.GetInt32("Duration")),
                            Genre = reader.GetString("Genre"),
                            BitRate = reader.GetInt32("Bitrate"),
                            ViewCount = reader.GetInt32("ViewCount"),
                            TrackNumber = reader.GetInt32("TrackNumber"),
                            AlbumTitle = reader.GetString("AlbumTitle"),
                            ArtistName = reader.GetString("ArtistName")
                        };
                        songs.Add(song);
                    }
                }
            }

            return songs;
        }
    }
}
