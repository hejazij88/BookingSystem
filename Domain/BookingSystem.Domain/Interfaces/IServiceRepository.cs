using BookingSystem.Domain.Models;

namespace BookingSystem.Domain.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(int id);
    Task<Service> AddServiceAsync(Service service);
    //void DeleteService(Service service); // چون DbContext را تزریق می‌کنیم، نیازی به Async برای حذف نیست
    Task<bool> SaveChangesAsync();
}