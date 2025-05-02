using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for address repository operations.
    /// </summary>
    public interface IAddressRepository
    {
        /// <summary>
        /// Adds a new address for a user.
        /// </summary>
        /// <param name="address">The address entity to add.</param>
        /// <returns>The created address entity.</returns>
        Task<AddressEntity> AddAddress(AddressEntity address);

        /// <summary>
        /// Deletes an address by its identifier.
        /// </summary>
        /// <param name="id">The address identifier.</param>
        /// <returns>True if the address was successfully deleted, otherwise false.</returns>
        Task<bool> DeleteAddress(int id);

        /// <summary>
        /// Retrieves an address by its identifier.
        /// </summary>
        /// <param name="id">The address identifier.</param>
        /// <returns>The address entity if found, otherwise null.</returns>
        Task<AddressEntity?> GetAddressById(int id);

        /// <summary>
        /// Retrieves all addresses associated with a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A collection of address entities.</returns>
        Task<IEnumerable<AddressEntity>> GetAddressesByUserId(int userId);

        /// <summary>
        /// Retrieves the default address for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The default address entity if found, otherwise null.</returns>
        Task<AddressEntity?> GetDefaultAddressForUserAsync(int userId);

        /// <summary>
        /// Sets a specific address as the default address for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The address identifier to set as default.</param>
        /// <returns>The updated address entity.</returns>
        Task<AddressEntity?> SetDefaultAddress(int userId, int id);

        /// <summary>
        /// Updates an existing address.
        /// </summary>
        /// <param name="address">The address entity to update.</param>
        /// <returns>The updated address entity.</returns>
        Task<AddressEntity?> UpdateAddress(AddressEntity address);
    }
}