using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing Address entities in the Ecommerce application.
    /// </summary>
    public class AddressRepository : IAddressRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AddressRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all addresses from the database.
        /// </summary>
        /// <returns>A list of all addresses.</returns>
        public async Task<IEnumerable<Address>> GetAllAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        /// <summary>
        /// Retrieves an address by its unique identifier.
        /// </summary>
        /// <param name="id">The address ID.</param>
        /// <returns>The matching address or null if not found.</returns>
        public async Task<Address?> GetAddressById(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all addresses associated with a specific user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>A list of addresses for the specified user.</returns>
        public async Task<IEnumerable<Address>> GetAddressesByUserId(int userId)
        {
            return await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Adds a new address to the database.
        /// </summary>
        /// <param name="address">The address to add.</param>
        /// <returns>True if the operation was successful.</returns>
        public async Task<bool> AddAddress(Address address)
        {
            _context.Addresses.Add(address);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Updates an existing address in the database.
        /// </summary>
        /// <param name="address">The address with updated details.</param>
        /// <returns>True if the operation was successful, false if the address was not found.</returns>
        public async Task<bool> UpdateAddress(Address address)
        {
            var existingAddress = await GetAddressById(address.Id);

            if (existingAddress is null)
                return false;

            _context.Addresses.Update(address);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Deletes an address by its unique identifier.
        /// </summary>
        /// <param name="id">The address ID.</param>
        /// <returns>True if the operation was successful, false if the address was not found.</returns>
        public async Task<bool> DeleteAddress(int id)
        {
            var address = await GetAddressById(id);

            if (address is null)
                return false;

            _context.Addresses.Remove(address);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Retrieves the default address for a specified user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>The default address or null if none is found.</returns>
        public async Task<Address?> GetDefaultAddressForUserAsync(int userId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
        }

        /// <summary>
        /// Sets an address as the default for a user.
        /// </summary>
        /// <param name="id">The address ID.</param>
        /// <returns>True if the operation was successful, false if the address was not found.</returns>
        public async Task<bool> SetDefaultAddress(int id)
        {
            var address = await GetAddressById(id);

            if (address is null)
                return false;

            await _context.Addresses
                .Where(a => a.UserId == address.UserId)
                .ForEachAsync(a => a.IsDefault = a.Id == id);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
