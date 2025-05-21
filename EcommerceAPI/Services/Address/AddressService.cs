using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;

namespace EcommerceAPI.Services.Address
{
    /// <summary>
    /// Service responsible for managing address-related operations.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Address.Interfaces.IAddressService" />
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, ICacheService cacheService, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the address by its identifier, validating access permissions based on the user's role.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the address.</param>
        /// <param name="userRole">The role of the user (e.g., Admin, Seller, Client).</param>
        /// <param name="id">The address ID.</param>
        /// <returns>An <see cref="AddressDto"/> representing the address.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the address is not found.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the user is not authorized to access the address.</exception>
        public async Task<AddressDto> GetAddressById(int userId, UserRole userRole, int id)
        {
            var cacheKey = $"Address_{id}";
            var cachedAddress = await _cacheService.Get<AddressDto>(cacheKey);

            if (cachedAddress != null)
            {
                if (userRole == UserRole.Customer && cachedAddress.UserId != userId)
                    throw new InvalidOperationException("You are not authorized to access this address");

                if (userRole != UserRole.Customer && userRole != UserRole.Admin)
                    throw new InvalidOperationException("Only customers and admins can access addresses");

                return cachedAddress;
            }

            var address = await _addressRepository.GetAddressById(id);

            if (address == null)
                throw new KeyNotFoundException("Address not found");

            if (userRole == UserRole.Customer && address.UserId != userId)
                throw new InvalidOperationException("You are not authorized to access this address");

            if (userRole != UserRole.Customer && userRole != UserRole.Admin)
                throw new InvalidOperationException("Only customers and admins can access addresses"); ;

            var addressDto = _mapper.Map<AddressDto>(address);
            await _cacheService.Set(cacheKey, addressDto, TimeSpan.FromMinutes(30));
            return addressDto;
        }

        /// <summary>
        /// Gets the default address for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An <see cref="AddressDto"/> representing the default address, or <c>null</c> if not found.</returns>
        public async Task<AddressDto?> GetDefaultAddressForUser(int userId)
        {
            var cacheKey = $"DefaultAddress_User_{userId}";
            var cachedAddress = await _cacheService.Get<AddressDto>(cacheKey);

            if (cachedAddress != null)
                return cachedAddress;

            var address = await _addressRepository.GetDefaultAddressForUserAsync(userId);

            if (address == null)
                return null;

            var addressDto = _mapper.Map<AddressDto>(address);
            await _cacheService.Set(cacheKey, addressDto, TimeSpan.FromMinutes(30));
            return addressDto;
        }

        /// <summary>
        /// Gets all addresses associated with a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of <see cref="AddressDto"/> representing the user's addresses.</returns>
        public async Task<IEnumerable<AddressDto>> GetAddressesByUserId(int userId)
        {
            var cacheKey = $"Addresses_User_{userId}";
            var cachedAddresses = await _cacheService.Get<IEnumerable<AddressDto>>(cacheKey);

            if (cachedAddresses != null)
                return cachedAddresses;

            var addresses = await _addressRepository.GetAddressesByUserId(userId);

            var addressesDto = _mapper.Map<IEnumerable<AddressDto>>(addresses);
            await _cacheService.Set(cacheKey, addressesDto, TimeSpan.FromMinutes(30));
            return addressesDto;
        }

        /// <summary>
        /// Adds a new address for a user. Sets it as default if it's the user's first address.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="addressAdd">The address details to add.</param>
        /// <returns>An <see cref="AddressDto"/> representing the added address.</returns>
        /// <exception cref="System.Exception">Thrown when the address could not be added.</exception>
        public async Task<AddressDto> AddAddress(int userId, AddressAddDto addressAdd)
        {
            var addressEntity = _mapper.Map<AddressEntity>(addressAdd);

            var addresses = await _addressRepository.GetAddressesByUserId(userId);

            if (addresses.Count() == 0)
                addressEntity.IsDefault = true;

            addressEntity.UserId = userId;
            var address = await _addressRepository.AddAddress(addressEntity);

            if (address == null)
                throw new Exception("Failed to add address");

            await _cacheService.Remove($"Address_{address.Id}");
            await _cacheService.Remove($"Addresses_User_{address.UserId}");

            var addressDto = _mapper.Map<AddressDto>(address);
            return addressDto;
        }

        /// <summary>
        /// Updates an existing address. Only the owner of the address can perform this operation.
        /// </summary>
        /// <param name="userId">The ID of the user attempting the update.</param>
        /// <param name="addressUpdate">The updated address data.</param>
        /// <returns>An <see cref="AddressDto"/> representing the updated address.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the address is not found.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the user is not authorized to update the address.</exception>
        /// <exception cref="System.Exception">Thrown when the update operation fails.</exception>
        public async Task<AddressDto> UpdateAddress(int userId, AddressUpdateDto addressUpdate)
        {
            var address = await _addressRepository.GetAddressById(addressUpdate.Id);

            if (address == null)
                throw new KeyNotFoundException("Address not found");

            if (address.UserId != userId)
                throw new InvalidOperationException("You are not authorized to update this address");

            _mapper.Map(addressUpdate, address);
            var updatedAddress = await _addressRepository.UpdateAddress(address);

            if (updatedAddress == null)
                throw new Exception("Failed to update address");

            await _cacheService.Remove($"Address_{addressUpdate.Id}");
            await _cacheService.Remove($"Addresses_User_{userId}");
            
            var addressDto = _mapper.Map<AddressDto>(updatedAddress);
            return addressDto;
        }

        /// <summary>
        /// Deletes an address. Only the owner of the address can perform this operation.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to delete the address.</param>
        /// <param name="id">The ID of the address to delete.</param>
        /// <returns><c>true</c> if the address was successfully deleted; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the address is not found.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the user is not authorized to delete the address.</exception>
        /// <exception cref="System.Exception">Thrown when the delete operation fails.</exception>
        public async Task<bool> DeleteAddress(int userId, int id)
        {
            var address = await _addressRepository.GetAddressById(id);

            if (address == null)
                throw new KeyNotFoundException("Address not found");

            if (address.UserId != userId)
                throw new InvalidOperationException("You are not authorized to delete this address");

            var result = await _addressRepository.DeleteAddress(id);

            if (!result)
                throw new Exception("Failed to delete address");

            await _cacheService.Remove($"Address_{id}");
            await _cacheService.Remove($"Addresses_User_{userId}");
            return result;
        }

        /// <summary>
        /// Sets an address as the default for a user. Only the owner can set their default address.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="id">The ID of the address to set as default.</param>
        /// <returns>An <see cref="AddressDto"/> representing the updated default address.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the address is not found.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the user is not authorized to set the address as default or if the operation fails.
        /// </exception>
        public async Task<AddressDto> SetDefaultAddress(int userId, int id)
        {
            var addressEntity = await _addressRepository.GetAddressById(id);

            if (addressEntity == null)
                throw new KeyNotFoundException("Address not found");

            if (addressEntity.UserId != userId)
                throw new InvalidOperationException("You are not authorized to set this address as default");

            var address = await _addressRepository.SetDefaultAddress(userId, id);

            if (address == null)
                throw new InvalidOperationException("Failed to set default address");

            await _cacheService.Remove($"Address_{id}");
            await _cacheService.Remove($"Addresses_User_{userId}");
            await _cacheService.Remove($"DefaultAddress_User_{userId}");

            var addressDto = _mapper.Map<AddressDto>(address);
            return addressDto;
        }
    }
}