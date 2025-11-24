using Microsoft.AspNetCore.SignalR;

namespace BookingSystem.Applications.Hubs;

public class AppointmentHub : Hub
{
    // Central place to keep method names consistent across server and client.
    public const string AppointmentCreated = "ReceiveAppointment";
    public const string AppointmentUpdated = "ReceiveAppointmentUpdated";
    public const string AppointmentDeleted = "ReceiveAppointmentDeleted";
}