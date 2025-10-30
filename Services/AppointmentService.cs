using CareDev.Data;
using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.EntityFrameworkCore;
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

        // Check for overlapping confirmed appointments
        public async Task<bool> IsSlotAvailableAsync(string doctorId, DateTime start, DateTime end)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId
                && a.Status == AppointmentStatus.Confirmed
                && a.Start < end && start < a.End
            );
        }

        // Book a new appointment (patient request -> pending)
        public async Task BookAppointmentAsync(string doctorId, string patientId, DateTime start, DateTime end)
        {
            if (!await IsSlotAvailableAsync(doctorId, start, end))
                throw new InvalidOperationException("Selected time is not available.");
            var appt = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Start = start,
                End = end,
                Status = AppointmentStatus.Pending
            };
            _context.Appointments.Add(appt);
            await _context.SaveChangesAsync();
            // Optionally notify doctor of pending request (via NotificationService)
        }

        public async Task<List<Appointment>> GetDoctorScheduleAsync(string doctorId)
        {
            // Doctors see only their own confirmed/pending appointments
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
        }
        public async Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
        }
        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            // For admin: all non-cancelled appointments
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
        }

        public async Task RescheduleAsync(int appointmentId, DateTime newStart, DateTime newEnd)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) throw new KeyNotFoundException();
            // Optionally: check permissions (only owner or doctor)
            // Check availability for doctor on new time
            if (!await IsSlotAvailableAsync(appt.DoctorId, newStart, newEnd))
                throw new InvalidOperationException("New time is not available.");
            appt.Start = newStart;
            appt.End = newEnd;
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int appointmentId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) throw new KeyNotFoundException();
            appt.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
    }
}

