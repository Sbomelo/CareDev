using System.Reflection;
using System.Text;

public static class ExportUtils
{
    // Generic CSV from any IEnumerable<T>
    public static string ToCsv<T>(IEnumerable<T> items)
    {
        var sb = new StringBuilder();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.GetGetMethod() != null).ToArray();

        // Header
        sb.AppendLine(string.Join(",", props.Select(p => EscapeCsv(p.Name))));

        foreach (var item in items)
        {
            var values = props.Select(p =>
            {
                var val = p.GetValue(item);
                var s = val == null ? "" : (val is DateTime dt ? dt.ToString("o") : val.ToString());
                return EscapeCsv(s);
            });
            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    public static string EscapeCsv(string value)
    {
        if (value == null) value = "";
        value = value.Replace("\r", " ").Replace("\n", " ");
        if (value.Contains('"') || value.Contains(','))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    public static byte[] ToJsonBytes<T>(T obj)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }
}
