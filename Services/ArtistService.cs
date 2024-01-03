using MusicBoxServer.Dtos;
using MusicBoxServer.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace MusicBoxServer.Services
{
    public class ArtistService
    {
        private readonly IConfiguration _configuration;

        public ArtistService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }

        public async Task<List<Artist>> GetAllArtistsAsync()
        {
            var artists = new List<Artist>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM artists", conn);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        artists.Add(new Artist
                        {
                            ArtistID = reader.GetInt32("ArtistID"),
                            Name = reader.GetString("Name"),
                            Bio = reader.GetString("Bio"),
                            DateOfBirth = reader.GetDateTime("DateOfBirth"),
                            INSLink = reader.GetString("INSLink"),
                            TwitterLink = reader.GetString("TwitterLink"),
                            FacebookLink = reader.GetString("FacebookLink"),
                            ListenerNum = reader.GetInt32("ListenerNum")
                        });
                    }
                }
            }

            return artists;
        }

        public async Task<Artist> GetArtistByIdAsync(int artistId)
        {
            Artist artist = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM artists WHERE ArtistID = @ArtistID", conn);
                command.Parameters.AddWithValue("@ArtistID", artistId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        artist = new Artist
                        {
                            ArtistID = reader.GetInt32("ArtistID"),
                            Name = reader.GetString("Name"),
                            Bio = reader.GetString("Bio"),
                            DateOfBirth = reader.GetDateTime("DateOfBirth"),
                            INSLink = reader.GetString("INSLink"),
                            TwitterLink = reader.GetString("TwitterLink"),
                            FacebookLink = reader.GetString("FacebookLink"),
                            ListenerNum = reader.GetInt32("ListenerNum")
                        };
                    }
                }
            }

            return artist;
        }

        public async Task AddArtistAsync(Artist artist)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("INSERT INTO artists (Name, Bio, DateOfBirth, INSLink, TwitterLink, FacebookLink, ListenerNum) VALUES (@Name, @Bio, @DateOfBirth, @INSLink, @TwitterLink, @FacebookLink, @ListenerNum)", conn);
                command.Parameters.AddWithValue("@Name", artist.Name);
                command.Parameters.AddWithValue("@Bio", artist.Bio);
                command.Parameters.AddWithValue("@DateOfBirth", artist.DateOfBirth);
                command.Parameters.AddWithValue("@INSLink", artist.INSLink);
                command.Parameters.AddWithValue("@TwitterLink", artist.TwitterLink);
                command.Parameters.AddWithValue("@FacebookLink", artist.FacebookLink);
                command.Parameters.AddWithValue("@ListenerNum", artist.ListenerNum);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateArtistAsync(Artist artist)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("UPDATE artists SET Name = @Name, Bio = @Bio, DateOfBirth = @DateOfBirth, INSLink = @INSLink, TwitterLink = @TwitterLink, FacebookLink = @FacebookLink, ListenerNum = @ListenerNum WHERE ArtistID = @ArtistID", conn);
                command.Parameters.AddWithValue("@ArtistID", artist.ArtistID);
                command.Parameters.AddWithValue("@Name", artist.Name);
                command.Parameters.AddWithValue("@Bio", artist.Bio);
                command.Parameters.AddWithValue("@DateOfBirth", artist.DateOfBirth);
                command.Parameters.AddWithValue("@INSLink", artist.INSLink);
                command.Parameters.AddWithValue("@TwitterLink", artist.TwitterLink);
                command.Parameters.AddWithValue("@FacebookLink", artist.FacebookLink);
                command.Parameters.AddWithValue("@ListenerNum", artist.ListenerNum);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteArtistAsync(int artistId)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("DELETE FROM artists WHERE ArtistID = @ArtistID", conn);
                command.Parameters.AddWithValue("@ArtistID", artistId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<ArtistDetails> GetArtistDetailsByIdAsync(int artistId)
        {
            var artistDetails = new ArtistDetails();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();

                // 获取艺术家信息及其专辑
                var artistAndAlbumsQuery = @"
            SELECT 
                ar.ArtistID, ar.Name, ar.Bio, ar.DateOfBirth, ar.INSLink, ar.FacebookLink, ar.TwitterLink, ar.ListenerNum,
                a.AlbumID, a.Title, a.ReleaseDate, a.Bio, a.Distributor
            FROM 
                artists ar
            LEFT JOIN 
                albums a ON ar.ArtistID = a.ArtistID
            WHERE 
                ar.ArtistID = @ArtistID";

                var command = new MySqlCommand(artistAndAlbumsQuery, conn);
                command.Parameters.AddWithValue("@ArtistID", artistId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (artistDetails.Artist == null)
                        {
                            artistDetails.Artist = new Artist
                            {
                                ArtistID = reader.GetInt32("ArtistID"),
                                Name = reader.GetString("Name"),
                                Bio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? "" : reader.GetString("Bio"),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? DateTime.Now : reader.GetDateTime("DateOfBirth"),
                                INSLink = reader.IsDBNull(reader.GetOrdinal("INSLink")) ? "" : reader.GetString("INSLink"),
                                FacebookLink = reader.IsDBNull(reader.GetOrdinal("FacebookLink")) ? "" : reader.GetString("FacebookLink"),
                                TwitterLink = reader.IsDBNull(reader.GetOrdinal("TwitterLink")) ? "" : reader.GetString("TwitterLink"),
                                ListenerNum = reader.IsDBNull(reader.GetOrdinal("ListenerNum")) ? 0 : reader.GetInt32("ListenerNum")
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("AlbumID")))
                        {
                            var album = new Album
                            {
                                AlbumID = reader.GetInt32("AlbumID"),
                                Title = reader.GetString("Title"),
                                ReleaseDate = reader.GetDateTime("ReleaseDate"),
                                Bio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? "" : reader.GetString("Bio"),
                                Distributor = reader.IsDBNull(reader.GetOrdinal("Distributor")) ? "" : reader.GetString("Distributor"),
                                ArtistID = artistDetails.Artist.ArtistID
                            };
                            artistDetails.Albums.Add(album);
                        }
                    }
                }

                // 获取浏览量最高的前10首歌曲
                var topSongsQuery = @"
            SELECT 
                s.SongID, s.Title, s.AlbumID, s.Duration, s.Genre, s.BitRate, s.ViewCount
            FROM 
                songs s
            JOIN 
                song_artists sa ON s.SongID = sa.SongID
            WHERE 
                sa.ArtistID = @ArtistID
            ORDER BY 
                s.ViewCount DESC
            LIMIT 10";

                command = new MySqlCommand(topSongsQuery, conn);
                command.Parameters.AddWithValue("@ArtistID", artistId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
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
                        artistDetails.TopSongs.Add(song);
                    }
                }
            }

            return artistDetails;
        }


    }
}
