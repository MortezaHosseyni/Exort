namespace Shared.Utilities
{
    public class CodeGenerator
    {
        public static string GenerateNumberCode(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be a positive number");
            }

            var random = new Random();
            var code = new char[length];

            for (var i = 0; i < length; i++)
            {
                code[i] = (char)('0' + random.Next(0, 10));
            }

            return new string(code);
        }
    }
}
