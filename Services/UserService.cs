using MusicBoxServer.Dtos;
using MusicBoxServer.Utils;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using System.Data.Common;
using System.Diagnostics;

namespace MusicBoxServer.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_configuration["MysqlSetting:ConnString"]);
        }

        public int AuthenticateUser(string username, string password)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT UserID, PasswordHash, Salt FROM users WHERE Username = @Username";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string PasswordHash = reader.GetString("PasswordHash");
                            string salt = reader.GetString("Salt");
                            int userID = (int)reader.GetInt64(reader.GetOrdinal("userID"));
                            if (Hash.VerifyHash(strHashed: PasswordHash, password, salt))
                            {
                                return userID;
                            }
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                // 处理异常，例如记录日志
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public bool RegisterUser(RegisterRequest request)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    if (IsUsernameExists(request.Username, connection))
                    {
                        return false; // 用户名已存在
                    }

                    string salt = Hash.GenSalt();
                    string hashedPassword = Hash.HashString(request.Password, salt); // 实现密码哈希
                    string query = "INSERT INTO users (Username, PasswordHash, Email, Area, Salt, DateCreated, LastLogin) VALUES (@Username, @Password, @Email, @Area, @Salt, @DateCreated, @LastLogin)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Username", request.Username);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@Email", request.Email);
                    cmd.Parameters.AddWithValue("@Area", request.Area);
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private bool IsUsernameExists(string username, MySqlConnection connection)
        {
            string query = "SELECT COUNT(1) FROM users WHERE username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        public UserInfo GetUserInfo(int userID)
        {
            UserInfo userInfo = null;

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    string query = "SELECT username, email, area, DateCreated, LastLogin FROM users WHERE userID = @UserID";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userInfo = new UserInfo
                            {
                                UserID = userID,
                                Username = reader.GetString("username"),
                                Email = reader.GetString("email"),
                                Area = reader.GetString("area"),
                                DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                                LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                                // 为其他字段赋值
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return userInfo;
        }

    }

}
