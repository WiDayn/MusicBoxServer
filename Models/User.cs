namespace MusicBoxServer.Models
{
    public class User(string userName, string hashedPassword, string email, DateTime createTime, DateTime lastLogin, string hashSalt)
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = userName;
        public string HashedPassword { get; set; } = hashedPassword;
        public string Email { get; set; } = email;
        public DateTime CreatedTime = createTime;
        public DateTime LastLogin = lastLogin;
        public string HashSalt = hashSalt;
    }
}
