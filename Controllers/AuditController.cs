using CareDev.Data;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CareDev.Controllers
{
    [Authorize(Roles = "Admin,Audit")] 
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditController> _logger;

        public AuditController(ApplicationDbContext context, ILogger<AuditController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Audit
        public async Task<IActionResult> Index(string? tableName, string? userId, DateTime? from, DateTime? to, int page = 1, int pageSize = 50)
        {
            var q = _context.AuditEntries.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(tableName))
                q = q.Where(a => a.TableName == tableName);

            if (!string.IsNullOrWhiteSpace(userId))
                q = q.Where(a => a.UserId == userId);

            if (from.HasValue)
                q = q.Where(a => a.CreatedAt >= from.Value.ToUniversalTime());

            if (to.HasValue)
                q = q.Where(a => a.CreatedAt <= to.Value.ToUniversalTime());

            q = q.OrderByDescending(a => a.CreatedAt);

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;
            ViewBag.TableName = tableName;
            ViewBag.UserId = userId;
            ViewBag.From = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.To = to?.ToString("yyyy-MM-dd") ?? "";

            return View(items);
        }

        // GET: /Audit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.AuditEntries.AsNoTracking().FirstOrDefaultAsync(a => a.AuditEntryId == id.Value);
            if (entry == null) return NotFound();

            var vm = new AuditDetailsViewModel
            {
                AuditEntry = entry
            };

            // deserialize old/new JSON into dictionaries of JsonElement so we can compare values and pretty-print complex values
            Dictionary<string, JsonElement>? oldDict = null;
            Dictionary<string, JsonElement>? newDict = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(entry.OldValues))
                    oldDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(entry.OldValues);

                if (!string.IsNullOrWhiteSpace(entry.NewValues))
                    newDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(entry.NewValues);
            }
            catch
            {
                // if JSON parse fails, fall back to showing raw strings in view (not ideal but safe)
            }

            // list of keys to consider (union)
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (oldDict != null) foreach (var k in oldDict.Keys) keys.Add(k);
            if (newDict != null) foreach (var k in newDict.Keys) keys.Add(k);

            // fields we should mask when showing audit values (PII)
            var sensitiveFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
              "PasswordHash", "SecurityStamp", "TwoFactorSecret", "SSN", "NationalId", "CreditCardNumber", "ApplicationUserId","Active", "RoleId","JobTitle"
              // add more field names that you want to hide/mask
            };

            foreach (var key in keys.OrderBy(k => k))
            {
                string? oldVal = null;
                string? newVal = null;

                if (oldDict != null && oldDict.TryGetValue(key, out var oe))
                    oldVal = JsonElementToReadableString(oe);

                if (newDict != null && newDict.TryGetValue(key, out var ne))
                    newVal = JsonElementToReadableString(ne);

                // Normalize comparison: null/empty treated same; compare trimmed strings
                var oldNorm = (oldVal ?? string.Empty).Trim();
                var newNorm = (newVal ?? string.Empty).Trim();

                if (oldNorm == newNorm)
                {
                    // unchanged -> skip
                    continue;
                }

                // mask sensitive fields
                if (sensitiveFields.Contains(key))
                {
                    if (!string.IsNullOrEmpty(oldVal)) oldVal = MaskString(oldVal);
                    if (!string.IsNullOrEmpty(newVal)) newVal = MaskString(newVal);
                }

                vm.Changes.Add(new AuditFieldChange
                {
                    FieldName = key,
                    OldValue = string.IsNullOrEmpty(oldVal) ? null : oldVal,
                    NewValue = string.IsNullOrEmpty(newVal) ? null : newVal
                });
            }

            return View(vm);
        }

        // helper: convert a JsonElement to a human readable string
        private static string JsonElementToReadableString(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.String:
                    return el.GetString() ?? string.Empty;
                case JsonValueKind.Number:
                    return el.GetRawText(); // keep number format
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return el.GetRawText();
                case JsonValueKind.Null:
                    return string.Empty;
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    // pretty-print JSON for complex types
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    return JsonSerializer.Serialize(el, options);
                default:
                    return el.ToString() ?? string.Empty;
            }
        }

        // helper: mask a string, keep last 2 chars visible (simple heuristic)
        private static string MaskString(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var visible = 2;
            if (input.Length <= visible) return new string('*', input.Length);
            return new string('*', Math.Max(0, input.Length - visible)) + input.Substring(input.Length - visible);
        }

        // helper to pretty print JSON; returns original string if invalid
        private static string PrettyJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return string.Empty;
            try
            {
                var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions { WriteIndented = true };
                return JsonSerializer.Serialize(doc.RootElement, options);
            }
            catch
            {
                return json ?? string.Empty;
            }
        }
    }
}

