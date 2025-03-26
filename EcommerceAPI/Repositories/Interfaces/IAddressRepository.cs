using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<bool> AddAddress(Address address);
        Task<bool> DeleteAddress(int id);
        Task<Address?> GetAddressById(int id);
        Task<IEnumerable<Address>> GetAddressesByUserId(int userId);
        Task<IEnumerable<Address>> GetAllAddresses();
        Task<Address?> GetDefaultAddressForUserAsync(int userId);
        Task<bool> SetDefaultAddress(int id);
        Task<bool> UpdateAddress(Address address);
    }
}