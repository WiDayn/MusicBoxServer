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

    }
}
