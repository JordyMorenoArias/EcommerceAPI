using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Address;

namespace EcommerceAPI.Services.Address.Interfaces
{
    /// <summary>
    /// Interface for Address Service that manages user address operations.
    /// </summary>
    public interface IAddressService
    {
        /// <summary>
        /// Adds a new address for the user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="addressAdd">The data transfer object containing the address to be added.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AddressDto"/> containing the added address details.</returns>
        Task<AddressDto> AddAddress(int userId, AddressAddDto addressAdd);

        /// <summary>
        /// Deletes an address for the user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="id">The identifier of the address to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the address was successfully deleted.</returns>
        Task<bool> DeleteAddress(int userId, int id);

        /// <summary>
        /// Retrieves an address by its identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userRole">The role of the user to determine access permissions.</param>
        /// <param name="id">The identifier of the address to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AddressDto"/> containing the address details.</returns>
        Task<AddressDto> GetAddressById(int userId, UserRole userRole, int id);

        /// <summary>
        /// Retrieves all addresses associated with a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="IEnumerable{AddressDto}"/> containing the list of addresses.</returns>
        Task<IEnumerable<AddressDto>> GetAddressesByUserId(int userId);

        /// <summary>
        /// Retrieves the default address for the user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AddressDto"/> containing the default address details, or null if no default address exists.</returns>
        Task<AddressDto?> GetDefaultAddressForUser(int userId);

        /// <summary>
        /// Sets the default address for the user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="id">The identifier of the address to set as default.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AddressDto"/> containing the updated default address details.</returns>
        Task<AddressDto> SetDefaultAddress(int userId, int id);

        /// <summary>
        /// Updates an existing address for the user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="addressUpdate">The data transfer object containing the updated address information.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AddressDto"/> containing the updated address details.</returns>
        Task<AddressDto> UpdateAddress(int userId, AddressUpdateDto addressUpdate);
    }
}