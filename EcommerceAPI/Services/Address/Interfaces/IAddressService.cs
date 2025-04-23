using EcommerceAPI.Models.DTOs.Address;

namespace EcommerceAPI.Services.Address.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDto> AddAddress(int userId, AddressAddDto addressAdd);
        Task<bool> DeleteAddress(int userId, int id);
        Task<AddressDto> GetAddressById(int userId, int id);
        Task<IEnumerable<AddressDto>> GetAddressesByUserId(int userId);
        Task<AddressDto> GetDefaultAddressForUser(int userId);
        Task<AddressDto> SetDefaultAddress(int userId, int id);
        Task<AddressDto> UpdateAddress(int userId, AddressUpdateDto addressUpdate);
    }
}