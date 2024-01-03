using Microsoft.Extensions.Configuration;
using MusicBoxServer.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace MusicBoxServer.Services
{
    public class ConcertService
    {
        private readonly IConfiguration _configuration;

        public ConcertService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }

        public async Task<List<Concert>> GetAllConcertsAsync()
        {
            var concerts = new List<Concert>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM concerts", conn);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        concerts.Add(new Concert
                        {
                            ConcertID = reader.GetInt32("ConcertID"),
                            ArtistID = reader.GetInt32("ArtistID"),
                            Venue = reader.GetString("Venue"),
                            Date = reader.GetDateTime("Date"),
                            TicketLink = reader.GetString("TicketLink")
                        });
                    }
                }
            }

            return concerts;
        }

        public async Task<Concert> GetConcertByIdAsync(int concertId)
        {
            Concert concert = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM concerts WHERE ConcertID = @ConcertID", conn);
                command.Parameters.AddWithValue("@ConcertID", concertId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        concert = new Concert
                        {
                            ConcertID = reader.GetInt32("ConcertID"),
                            ArtistID = reader.GetInt32("ArtistID"),
                            Venue = reader.GetString("Venue"),
                            Date = reader.GetDateTime("Date"),
                            TicketLink = reader.GetString("TicketLink")
                        };
                    }
                }
            }

            return concert;
        }

        public async Task AddConcertAsync(Concert concert)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("INSERT INTO concerts (ArtistID, Venue, Date, TicketLink) VALUES (@ArtistID, @Venue, @Date, @TicketLink)", conn);
                command.Parameters.AddWithValue("@ArtistID", concert.ArtistID);
                command.Parameters.AddWithValue("@Venue", concert.Venue);
                command.Parameters.AddWithValue("@Date", concert.Date);
                command.Parameters.AddWithValue("@TicketLink", concert.TicketLink);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateConcertAsync(Concert concert)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("UPDATE concerts SET ArtistID = @ArtistID, Venue = @Venue, Date = @Date, TicketLink = @TicketLink WHERE ConcertID = @ConcertID", conn);
                command.Parameters.AddWithValue("@ConcertID", concert.ConcertID);
                command.Parameters.AddWithValue("@ArtistID", concert.ArtistID);
                command.Parameters.AddWithValue("@Venue", concert.Venue);
                command.Parameters.AddWithValue("@Date", concert.Date);
                command.Parameters.AddWithValue("@TicketLink", concert.TicketLink);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task DeleteConcertAsync(int concertId)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var command = new MySqlCommand("DELETE FROM concerts WHERE ConcertID = @ConcertID", conn);
                command.Parameters.AddWithValue("@ConcertID", concertId);

                await command.ExecuteNonQueryAsync();
            }
        }

    }
}
