namespace Booking.Application.Utils
{
    public static class BookingCodeGenerator
    {
        private static readonly Random _random = new();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string Generate()
        {
            var code = new char[6];
            for (int i = 0; i < 6; i++)
            {
                code[i] = Chars[_random.Next(Chars.Length)];
            }
            return new string(code);
        }
    }
}
