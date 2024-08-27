namespace Shared.Utilities
{
    public class Password
    {
        public static string Hash(string password, int workFactor = 12)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
