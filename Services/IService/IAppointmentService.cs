using CareDev.Models;

namespace CareDev.Services.IService
{
    public interface IAppointmentService
    {
        Task<bool> IsSlotAvailableAsync(string doctorId, DateTime start, DateTime endUtc);
        Task BookAppointmentAsync(string doctorId, string patientId, DateTime startUtc, DateTime endUtc, string? notes= null);
        Task<IEnumerable<Appointment>> GetDoctorScheduleAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(string patientId);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task RescheduleAsync(int appointmentId, DateTime newStartUtc, DateTime newEndUtc);
        Task ConfirmAsync(int appointmentId);
        Task CancelAsync(int appointmentId);
        Task<Appointment?> GetByIdAsync(int appointmentId);
    }
}
