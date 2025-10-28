using CareDev.Data;
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

            // optional: pretty-print JSON for view
            string prettyOld = PrettyJson(entry.OldValues);
            string prettyNew = PrettyJson(entry.NewValues);
            ViewBag.PrettyOld = prettyOld;
            ViewBag.PrettyNew = prettyNew;

            return View(entry);
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

