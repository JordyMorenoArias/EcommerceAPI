using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IAddressRepository _addressRepository;

        public AddressService(IMapper mapper, ICacheService cacheService ,IAddressRepository addressRepository)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _addressRepository = addressRepository;
        }

        /// <summary>
        /// Gets the address by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Address not found</exception>
        public async Task<AddressDto> GetAddressById(int userId, int id)
        {
            var cacheKey = $"Address_{id}";
            var cachedAddress = await _cacheService.Get<AddressDto>(cacheKey);

            if (cachedAddress != null)
                return cachedAddress;

            var address = await _addressRepository.GetAddressById(id);

            if (address == null)
                throw new KeyNotFoundException("Address not found");

            if (userId != address.UserId)
                throw new InvalidOperationException("You are not authorized to access this address");

            var addressDto = _mapper.Map<AddressDto>(address);
            await _cacheService.Set(cacheKey, addressDto, TimeSpan.FromMinutes(30));
            return addressDto;
        }

        /// <summary>
        /// Gets the default address for user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
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
        /// Gets the addresses by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
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
        /// Adds the address.
        /// </summary>
        /// <param name="addressAdd">The address add.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Failed to add address</exception>
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
        /// Updates the address.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="addressUpdate">The address update.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Address not found</exception>
        /// <exception cref="System.InvalidOperationException">You are not authorized to update this address</exception>
        /// <exception cref="System.Exception">Failed to update address</exception>
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
        /// Deletes the address.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Address not found</exception>
        /// <exception cref="System.InvalidOperationException">You are not authorized to delete this address</exception>
        /// <exception cref="System.Exception">Failed to delete address</exception>
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
        /// Sets the default address.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Address not found</exception>
        /// <exception cref="System.InvalidOperationException">
        /// You are not authorized to set this address as default
        /// or
        /// Failed to set default address
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