using MusicBoxServer.Dtos;
using MusicBoxServer.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace MusicBoxServer.Services
{
    public class AlbumService
    {
        private readonly IConfiguration _configuration;

        public AlbumService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }

        public async Task<List<Album>> GetAllAlbumsAsync()
        {
            var albums = new List<Album>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM albums", conn);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        albums.Add(new Album
                        {
                            AlbumID = reader.GetInt32("AlbumID"),
                            Title = reader.GetString("Title"),
                            ArtistID = reader.GetInt32("ArtistID"),
                            Bio = reader.GetString("Bio"),
                            ReleaseDate = reader.GetDateTime("ReleaseDate"),
                            Distributor = reader.GetString("Distributor")
                        });
                    }
                }
            }

            return albums;
        }

        public async Task<Album> GetAlbumByIdAsync(int albumId)
        {
            Album album = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM albums WHERE AlbumID = @AlbumID", conn);
                command.Parameters.AddWithValue("@AlbumID", albumId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        album = new Album
                        {
                            AlbumID = reader.GetInt32("AlbumID"),
                            Title = reader.GetString("Title"),
                            ArtistID = reader.GetInt32("ArtistID"),
                            Bio = reader.GetString("Bio"),
                            ReleaseDate = reader.GetDateTime("ReleaseDate"),
                            Distributor = reader.GetString("Distributor")
                        };
                    }
                }
            }

            return album;
        }

        public async Task AddAlbumAsync(Album album)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("INSERT INTO albums (Title, ArtistID, Bio, ReleaseDate, Distributor) VALUES (@Title, @ArtistID, @Bio, @ReleaseDate, @Distributor)", conn);
                command.Parameters.AddWithValue("@Title", album.Title);
                command.Parameters.AddWithValue("@ArtistID", album.ArtistID);
                command.Parameters.AddWithValue("@Bio", album.Bio);
                command.Parameters.AddWithValue("@ReleaseDate", album.ReleaseDate);
                command.Parameters.AddWithValue("@Distributor", album.Distributor);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAlbumAsync(Album album)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("UPDATE albums SET Title = @Title, ArtistID = @ArtistID, Bio = @Bio, ReleaseDate = @ReleaseDate, Distributor = @Distributor WHERE AlbumID = @AlbumID", conn);
                command.Parameters.AddWithValue("@AlbumID", album.AlbumID);
                command.Parameters.AddWithValue("@Title", album.Title);
                command.Parameters.AddWithValue("@ArtistID", album.ArtistID);
                command.Parameters.AddWithValue("@Bio", album.Bio);
                command.Parameters.AddWithValue("@ReleaseDate", album.ReleaseDate);
                command.Parameters.AddWithValue("@Distributor", album.Distributor);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task DeleteAlbumAsync(int albumId)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("DELETE FROM albums WHERE AlbumID = @AlbumID", conn);
                command.Parameters.AddWithValue("@AlbumID", albumId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<AlbumDetails> GetAlbumDetailsByIdAsync(int albumId)
        {
            var albumDetails = new AlbumDetails();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand(@"
            SELECT 
                a.AlbumID, a.Title, a.ReleaseDate, a.Bio, a.Distributor, a.ArtistID,
                s.SongID, s.Title AS SongTitle, s.AlbumID, s.Duration, s.Genre, s.BitRate, s.ViewCount,
                ar.Name AS ArtistName
            FROM 
                albums a
            LEFT JOIN 
                songs s ON a.AlbumID = s.AlbumID
            LEFT JOIN 
                artists ar ON a.ArtistID = ar.ArtistID
            WHERE 
                a.AlbumID = @AlbumID", conn);
                command.Parameters.AddWithValue("@AlbumID", albumId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (albumDetails.Album == null)
                        {
                            albumDetails.Album = new Album
                            {
                                AlbumID = reader.GetInt32("AlbumID"),
                                Title = reader.GetString("Title"),
                                ReleaseDate = reader.GetDateTime("ReleaseDate"),
                                Bio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? "" : reader.GetString("Bio"),
                                Distributor = reader.IsDBNull(reader.GetOrdinal("Distributor")) ? "" : reader.GetString("Distributor"),
                                ArtistID = reader.GetInt32("ArtistID"),
                            };
                            albumDetails.ArtistName = reader.GetString("ArtistName");
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("SongID")))
                        {
                            var song = new Song
                            {
                                SongID = reader.GetInt32("SongID"),
                                Title = reader.GetString("SongTitle"),
                                AlbumID = reader.GetInt32("albumID"),
                                Duration = TimeSpan.FromSeconds(reader.GetInt32("Duration")),
                                Genre = reader.GetString("Genre"),
                                BitRate = reader.GetInt32("BitRate"),
                                ViewCount = reader.GetInt32("ViewCount")
                            };
                            albumDetails.Songs.Add(song);
                        }
                    }
                }
            }

            return albumDetails;
        }

        public async Task<List<int>> GetRecentAlbumsAsync()
        {
            var recentAlbums = new List<int>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand(@"SELECT AlbumID FROM albums ORDER BY CreateTime DESC LIMIT 20", conn);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var album = new Album
                        {
                            AlbumID = reader.GetInt32("AlbumID"),
                        };
                        recentAlbums.Add(album.AlbumID);
                    }
                }
            }

            return recentAlbums;
        }
    }
}
