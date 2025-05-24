using EcommerceAPI.Data;
using EcommerceAPI.Models.Entities;
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
        /// Retrieves an address by its unique identifier.
        /// </summary>
        /// <param name="id">The address ID.</param>
        /// <returns>The matching address or null if not found.</returns>
        public async Task<AddressEntity?> GetAddressById(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all addresses associated with a specific user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>A list of addresses for the specified user.</returns>
        public async Task<IEnumerable<AddressEntity>> GetAddressesByUserId(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new address to the database.
        /// </summary>
        /// <param name="address">The address to add.</param>
        /// <returns>True if the operation was successful.</returns>
        public async Task<AddressEntity> AddAddress(AddressEntity address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        /// <summary>
        /// Updates an existing address in the database.
        /// </summary>
        /// <param name="address">The address with updated details.</param>
        /// <returns>True if the operation was successful, false if the address was not found.</returns>
        public async Task<AddressEntity?> UpdateAddress(AddressEntity address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
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
        public async Task<AddressEntity?> GetDefaultAddressForUserAsync(int userId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
        }

        /// <summary>
        /// Sets an address as the default for a user.
        /// </summary>
        /// <param name="id">The address ID.</param>
        /// <returns>True if the operation was successful, false if the address was not found.</returns>
        public async Task<AddressEntity?> SetDefaultAddress(int userId, int id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
                return null;

            await _context.Addresses
                .Where(a => a.UserId == userId)
                .ForEachAsync(a => a.IsDefault = a.Id == id);

            await _context.SaveChangesAsync();

            return address;
        }
    }
}
