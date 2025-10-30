using CareDev.Models;

namespace CareDev.Services.IService
{
    public interface IAppointmentService
    {
        Task<bool> IsSlotAvailableAsync(string doctorId, DateTime start, DateTime end);
        Task BookAppointmentAsync(string doctorId, string patientId, DateTime start, DateTime end);
        Task<List<Appointment>> GetDoctorScheduleAsync(string doctorId);
        Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId);
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task RescheduleAsync(int appointmentId, DateTime newStart, DateTime newEnd);
        Task CancelAsync(int appointmentId);
    }
}
