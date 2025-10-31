namespace CareDev.Heplers
{
    public class IdNumberHelper
    {
        // Parse SA ID's first 6 digits YYMMDD to a DateTime. Returns null if invalid.
        public static DateTime? ParseSouthAfricanIdDob(string idNumber)
        {
            if (string.IsNullOrWhiteSpace(idNumber) || idNumber.Length < 6) return null;

            var yy = idNumber.Substring(0, 2);
            var mm = idNumber.Substring(2, 2);
            var dd = idNumber.Substring(4, 2);

            if (!int.TryParse(yy, out var y) ||
                !int.TryParse(mm, out var m) ||
                !int.TryParse(dd, out var d))
                return null;

            // Resolve century: assume people older than (current year % 100) are 1900s
            var now = DateTime.UtcNow;
            var currentTwoDigitYear = now.Year % 100;
            int fullYear = (y > currentTwoDigitYear) ? 1900 + y : 2000 + y;

            try
            {
                return new DateTime(fullYear, m, d);
            }
            catch
            {
                return null;
            }
        }
    }
}
