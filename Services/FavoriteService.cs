using MySql.Data.MySqlClient;
using MusicBoxServer.Dtos;
using System.Data;
using Google.Protobuf.WellKnownTypes;
using MusicBoxServer.Models;
using System.Data.Common;

namespace MusicBoxServer.Services
{
    public class FavoriteService
    {
        private readonly IConfiguration _configuration;

        public FavoriteService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }
        public async Task AddFavoriteSongAsync(FavoriteRequest favorite)
        {
            string checkQuery = "SELECT COUNT(1) FROM favorites_songs WHERE UserID = @UserID AND SongID = @SongID";

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                // 首先检查喜爱关系是否已存在
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserID", favorite.UserID);
                    checkCommand.Parameters.AddWithValue("@SongID", favorite.ID);

                    int exists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                    if (exists == 0)
                    {
                        // 如果不存在，插入新的喜爱关系
                        string insertQuery = "INSERT INTO favorites_songs (UserID, SongID, DateFavorited) VALUES (@UserID, @SongID, @DateFavorited)";
                        using (var insertCommand = new MySqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserID", favorite.UserID);
                            insertCommand.Parameters.AddWithValue("@SongID", favorite.ID);
                            insertCommand.Parameters.AddWithValue("@DateFavorited", favorite.DateFavorited);
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }

        public async Task RemoveFavoriteSongAsync(int userId, int songId)
        {
            string query = "DELETE FROM favorites_songs WHERE UserID = @UserID AND SongID = @SongID";

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@SongID", songId);
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }

        public async Task AddFavoriteArtistAsync(FavoriteRequest favorite)
        {
            string query = "INSERT INTO favorites_songs (UserID, SongID, DateFavorited) VALUES (@UserID, @ID, @DateFavorited)";

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", favorite.UserID);
                    command.Parameters.AddWithValue("@SongID", favorite.ID);
                    command.Parameters.AddWithValue("@DateFavorited", favorite.DateFavorited);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AddFavoritePlayListAsync(FavoriteRequest favorite)
        {
            string query = "INSERT INTO favorites_songs (UserID, SongID, DateFavorited) VALUES (@UserID, @ID, @DateFavorited)";

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", favorite.UserID);
                    command.Parameters.AddWithValue("@SongID", favorite.ID);
                    command.Parameters.AddWithValue("@DateFavorited", favorite.DateFavorited);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<FavoriteInfo> GetFavoritesByUserIdAsync(int userId)
        {
            var favoriteInfo = new FavoriteInfo
            {
                SongInfos = new List<SongInfo>(),
                ArtistInfos = new List<FavoriteItem>(),
                AlbumInfos = new List<FavoriteItem>(),
                PlayListInfos = new List<FavoriteItem>()
            };

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                // 获取收藏的歌曲
                await AddFavoriteSongsAsync(connection, userId, favoriteInfo.SongInfos);

                // 获取收藏的艺术家
                await AddFavoriteItemsAsync(connection, userId, @"
            SELECT ArtistID AS Id, DateFavorited
            FROM favorites_artists
            WHERE UserID = @UserId", favoriteInfo.ArtistInfos);

                // 获取收藏的专辑
                await AddFavoriteItemsAsync(connection, userId, @"
            SELECT AlbumID AS Id, DateFavorited
            FROM favorites_albums
            WHERE UserID = @UserId", favoriteInfo.AlbumInfos);

                // 获取收藏的播放列表
                await AddFavoriteItemsAsync(connection, userId, @"
            SELECT PlayListID AS Id, DateFavorited
            FROM favorites_playlists
            WHERE UserID = @UserId", favoriteInfo.PlayListInfos);
            }

            return favoriteInfo;
        }

        private async Task AddFavoriteSongsAsync(MySqlConnection connection, int userId, List<SongInfo> list)
        {
            string query = @"
        SELECT s.*, a.Title AS AlbumTitle, ar.Name AS ArtistName, ar.ArtistID, f.DateFavorited
        FROM favorites_songs f
        JOIN songs s ON f.SongID = s.SongID
        JOIN albums a ON s.AlbumID = a.AlbumID
        JOIN artists ar ON a.ArtistID = ar.ArtistID
        WHERE f.UserID = @UserId";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var songInfo = new SongInfo
                        {
                            SongID = reader.GetInt32("SongID"),
                            Title = reader.GetString("Title"),
                            AlbumID = reader.GetInt32("AlbumID"),
                            AlbumTitle = reader.GetString("AlbumTitle"),
                            ArtistID = reader.GetInt32("ArtistID"),
                            ArtistName = reader.GetString("ArtistName"),
                            Duration = reader.GetInt32("Duration"),
                            Genre = reader.GetString("Genre"),
                            Bitrate = reader.GetInt32("Bitrate"),
                            ViewCount = reader.GetInt32("ViewCount"),
                            TrackNumber = reader.GetInt32("TrackNumber"),
                            DateFavorited = reader.GetDateTime("DateFavorited")
                        };
                        list.Add(songInfo);
                    }
                }
            }
        }

        private async Task AddFavoriteItemsAsync(MySqlConnection connection, int userId, string query, List<FavoriteItem> list)
        {
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new FavoriteItem
                        {
                            ID = reader.GetInt32("Id"),
                            DateFavorited = reader.GetDateTime("DateFavorited")
                        });
                    }
                }
            }
        }
    }
}
