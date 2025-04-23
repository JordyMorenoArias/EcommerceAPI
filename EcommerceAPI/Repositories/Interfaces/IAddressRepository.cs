using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<AddressEntity> AddAddress(AddressEntity address);
        Task<bool> DeleteAddress(int id);
        Task<AddressEntity?> GetAddressById(int id);
        Task<IEnumerable<AddressEntity>> GetAddressesByUserId(int userId);
        Task<AddressEntity?> GetDefaultAddressForUserAsync(int userId);
        Task<AddressEntity?> SetDefaultAddress(int userId, int id);
        Task<AddressEntity?> UpdateAddress(AddressEntity address);
    }
}