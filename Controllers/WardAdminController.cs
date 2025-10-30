using CareDev.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareDev.Controllers
{ 
    [Authorize(Roles = "WardAdmin,Admin")]
    public class WardAdminController : Controller
    {

        private readonly ApplicationDbContext _context;

        public WardAdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> SearchPatients(string q, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(new { results = new object[0], more = false });

            q = q.Trim();

            var query = _context.Patients
                .AsNoTracking()
                .Where(p => !p.IsAdmitted &&
                           (EF.Functions.Like(p.Name, $"%{q}%") ||
                            EF.Functions.Like(p.SurName, $"%{q}%") ||
                            EF.Functions.Like(p.PhoneNumber, $"%{q}%") ||
                            EF.Functions.Like(p.IDNumber, $"%{q}%")));

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new {
                    id = p.PatientId,
                    text = p.Name + " — " + (p.IDNumber ?? "") + " — " +
                           (p.DateOfBirth != null ? p.DateOfBirth.Value.ToString("yyyy-MM-dd") : "")
                }).ToListAsync();

            var more = (page * pageSize) < total;

            // Return in Select2 expected format:
            return Json(new { results = items, pagination = new { more } });
        }
    }
}
