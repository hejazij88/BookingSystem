using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;

namespace BookingSystem.Client.HubsConnection;

public class AppointmentSignalRService
{
    private HubConnection? _hubConnection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public event Action<AppointmentRealtimeDto>? OnAppointmentCreated;
    public event Action<AppointmentRealtimeDto>? OnAppointmentUpdated;
    public event Action<AppointmentRealtimeDto>? OnAppointmentDeleted;

    public async Task EnsureConnectedAsync(NavigationManager navigation)
    {
        await _connectionLock.WaitAsync();
        try
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(navigation.ToAbsoluteUri("/appointmentHub"))
                    .WithAutomaticReconnect()
                    .Build();

                RegisterHandlers();
            }

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private void RegisterHandlers()
    {
        if (_hubConnection == null)
        {
            return;
        }

        _hubConnection.On<AppointmentRealtimeDto>(AppointmentHub.AppointmentCreated,
            appointment => OnAppointmentCreated?.Invoke(appointment));

        _hubConnection.On<AppointmentRealtimeDto>(AppointmentHub.AppointmentUpdated,
            appointment => OnAppointmentUpdated?.Invoke(appointment));

        _hubConnection.On<AppointmentRealtimeDto>(AppointmentHub.AppointmentDeleted,
            appointment => OnAppointmentDeleted?.Invoke(appointment));
    }

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
}