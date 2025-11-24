using BookingSystem.Domain.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class ServiceRepository:IServiceRepository
{

    private readonly BookingDbContext _context;

    public ServiceRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services.ToListAsync();
    }

    public async Task<Service?> GetServiceByIdAsync(int id)
    {
        return await _context.Services.FindAsync(id);
    }

    public async Task<Service> AddServiceAsync(Service service)
    {
        await _context.Services.AddAsync(service);
        return service;
    }

    //public void DeleteService(Service service)
    //{
    //    _context.Services.Remove(service);
    //}

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}