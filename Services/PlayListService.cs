using MusicBoxServer.Dtos;
using MusicBoxServer.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace MusicBoxServer.Services
{
    public class PlayListService
    {
        private readonly IConfiguration _configuration;

        public PlayListService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }

        public async Task<List<PlayList>> GetAllPlayListsAsync()
        {
            var playlists = new List<PlayList>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM playlists", conn);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        playlists.Add(new PlayList
                        {
                            PlayListID = reader.GetInt32("PlayListID"),
                            UserID = reader.GetInt32("UserID"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                            DateCreated = reader.GetDateTime("DateCreated")
                        });
                    }
                }
            }

            return playlists;
        }

        public async Task<PlayList> GetPlayListByIdAsync(int playlistId)
        {
            PlayList playlist = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM playlists WHERE PlayListID = @PlayListID", conn);
                command.Parameters.AddWithValue("@PlayListID", playlistId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        playlist = new PlayList
                        {
                            PlayListID = reader.GetInt32("PlayListID"),
                            UserID = reader.GetInt32("UserID"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                            DateCreated = reader.GetDateTime("DateCreated")
                        };
                    }
                }
            }

            return playlist;
        }

        public async Task AddPlayListAsync(PlayList playlist)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("INSERT INTO playlists (UserID, Name, Description, DateCreated) VALUES (@UserID, @Name, @Description, @DateCreated)", conn);
                command.Parameters.AddWithValue("@UserID", playlist.UserID);
                command.Parameters.AddWithValue("@Name", playlist.Name);
                command.Parameters.AddWithValue("@Description", playlist.Description);
                command.Parameters.AddWithValue("@DateCreated", playlist.DateCreated);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdatePlayListAsync(PlayList playlist)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("UPDATE playlists SET UserID = @UserID, Name = @Name, Description = @Description, DateCreated = @DateCreated WHERE PlayListID = @PlayListID", conn);
                command.Parameters.AddWithValue("@PlayListID", playlist.PlayListID);
                command.Parameters.AddWithValue("@UserID", playlist.UserID);
                command.Parameters.AddWithValue("@Name", playlist.Name);
                command.Parameters.AddWithValue("@Description", playlist.Description);
                command.Parameters.AddWithValue("@DateCreated", playlist.DateCreated);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeletePlayListAsync(int playlistId)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("DELETE FROM playlists WHERE PlayListID = @PlayListID", conn);
                command.Parameters.AddWithValue("@PlayListID", playlistId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<PlayListDetails> GetPlayListDetailsByIdAsync(int playListId)
        {
            var details = new PlayListDetails();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand(@"
            SELECT 
                pl.PlayListID, pl.UserID, pl.Name, pl.Description, pl.DateCreated,
                s.SongID, s.Title, s.AlbumID, s.Duration, s.Genre, s.BitRate, s.ViewCount,
                u.Username
            FROM 
                playlists pl
            LEFT JOIN 
                playlist_songs ps ON pl.PlayListID = ps.PlayListID
            LEFT JOIN 
                songs s ON ps.SongID = s.SongID
            LEFT JOIN 
                users u ON pl.UserID = u.UserID
            WHERE 
                pl.PlayListID = @PlayListID", conn);
                command.Parameters.AddWithValue("@PlayListID", playListId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var playlistRead = false;
                    while (await reader.ReadAsync())
                    {
                        if (!playlistRead)
                        {
                            details.PlayList = new PlayList
                            {
                                PlayListID = reader.GetInt32("PlayListID"),
                                UserID = reader.GetInt32("UserID"),
                                Name = reader.GetString("Name"),
                                Description = reader.GetString("Description"),
                                DateCreated = reader.GetDateTime("DateCreated")
                            };
                            details.Username = reader.GetString("Username");
                            playlistRead = true;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("SongID")))
                        {
                            var song = new Song
                            {
                                SongID = reader.GetInt32("SongID"),
                                Title = reader.GetString("Title"),
                                AlbumID = reader.GetInt32("AlbumID"),
                                Duration = TimeSpan.FromSeconds(reader.GetInt32("Duration")),
                                Genre = reader.GetString("Genre"),
                                BitRate = reader.GetInt32("BitRate"),
                                ViewCount = reader.GetInt32("ViewCount")
                            };
                            details.Songs.Add(song);
                        }
                    }
                }
            }

            return details;
        }

    }
}
