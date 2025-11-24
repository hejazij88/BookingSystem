using BookingSystem.Applications.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BookingSystem.Client.HubsConnection;

public class AppointmentSignalRService
{
    private HubConnection? _hubConnection;

    public event Action<AppointmentDto>? OnAppointmentCreated;
    public event Action<AppointmentDto>? OnAppointmentUpdated;
    public event Action<int>? OnAppointmentDeleted;

    public async Task StartAsync(NavigationManager navigation)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigation.ToAbsoluteUri("/appointmentHub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<AppointmentDto>("ReceiveAppointment", appointment =>
        {
            OnAppointmentCreated?.Invoke(appointment);
        });

        _hubConnection.On<AppointmentDto>("ReceiveAppointmentUpdated", appointment =>
        {
            OnAppointmentUpdated?.Invoke(appointment);
        });

        _hubConnection.On<int>("ReceiveAppointmentDeleted", appointmentId =>
        {
            OnAppointmentDeleted?.Invoke(appointmentId);
        });

        await _hubConnection.StartAsync();
    }

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
}