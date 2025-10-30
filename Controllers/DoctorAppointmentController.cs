using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CareDev.Models.ApplicationUser;
using CareDev.Data;


[Authorize(Roles = "Doctor")]
public class DoctorAppointmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAppointmentService _appointmentService;
    private readonly UserManager<ApplicationUser> _userManager;
    public DoctorAppointmentController(IAppointmentService appointmentService,
                                       UserManager<ApplicationUser> userManager,
                                       ApplicationDbContext context)
    {
        _appointmentService = appointmentService;
        _userManager = userManager;
        _context = context;
    }

    // View own schedule
    public async Task<IActionResult> MySchedule()
    {
        var doctorId = _userManager.GetUserId(User);
        var schedule = await _appointmentService.GetDoctorScheduleAsync(doctorId);
        return View(schedule);
    }

    // Confirm a pending appointment
    [HttpPost]
    public async Task<IActionResult> Confirm(int id)
    {
        var appt = await _context.Appointments.FindAsync(id);
        if (appt == null) return NotFound();
        appt.Status = AppointmentStatus.Confirmed;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MySchedule));
    }
}

