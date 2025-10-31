using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;
using CareDev.Services.IService;
using static CareDev.Models.ApplicationUser;

namespace CareDev.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSlotAvailableAsync(string doctorId, DateTime startUtc, DateTime endUtc)
        {
            return !await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId
                               && a.Status == AppointmentStatus.Confirmed
                               && a.Start < endUtc && startUtc < a.End);
        }

        public async Task BookAppointmentAsync(string doctorId, string patientId, DateTime startUtc, DateTime endUtc, string? notes = null)
        {
            if (!await IsSlotAvailableAsync(doctorId, startUtc, endUtc))
                throw new InvalidOperationException("Selected time is not available.");

            var appt = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Start = startUtc,
                End = endUtc,
                Notes = notes,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appt);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDoctorScheduleAsync(string doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.Start)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(string patientId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.Start)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.Start)
                .ToListAsync();
        }

        public async Task RescheduleAsync(int appointmentId, DateTime newStartUtc, DateTime newEndUtc)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found");

            if (!await IsSlotAvailableAsync(appt.DoctorId, newStartUtc, newEndUtc))
                throw new InvalidOperationException("Selected time is not available.");

            appt.Start = newStartUtc;
            appt.End = newEndUtc;
            appt.Status = AppointmentStatus.Confirmed;
            appt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmAsync(int appointmentId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found");
            appt.Status = AppointmentStatus.Confirmed;
            appt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int appointmentId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found");

            appt.Status = AppointmentStatus.Cancelled;
            appt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                       .Include(a => a.Patient)
                       .Include(a => a.Doctor)
                       .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }
    }
}
