namespace MusicBoxServer.Utils
{
    public class Hash
    {
        public static string HashString(string str, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(str, salt);
        }

        public static string GenSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(6);
        }
        public static bool VerifyHash(string strHashed, string str, string salt)
        {
            return strHashed == BCrypt.Net.BCrypt.HashPassword(str, salt);
        }
    }
}
