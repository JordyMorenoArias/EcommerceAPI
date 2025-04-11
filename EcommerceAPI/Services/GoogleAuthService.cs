using AutoMapper;
using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;
using EcommerceAPI.Services.Security.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceAPI.Services
{
    public class GoogleAuthService : IOAuthProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public GoogleAuthService(IConfiguration configuration, IUserRepository userRepository, IJwtService jwtService, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> GetUserInfoAsync(string code)
        {
            var clientId = _configuration["google_oauth:client_id"];
            var clientSecret = _configuration["google_oauth:client_secret"];
            var redirectUri = _configuration["google_oauth:redirect_uri"];
            var grantType = _configuration["google_oauth:grant_type"];

            var values = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "redirect_uri", redirectUri! },
                { "grant_type", grantType! }
            };

            using var httpClient = new HttpClient();
            var contentToSend = new FormUrlEncodedContent(values);

            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", contentToSend);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve token from Google.");

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<UserGoogleTokenResponse>(content);

            if (tokenResponse == null)
                throw new Exception("Failed to deserialize Google token response.");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenResponse.IdToken);

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var firstName = jwtToken.Claims.First(c => c.Type == "given_name").Value;
            var lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "";
            var providerId = jwtToken.Claims.First(c => c.Type == "sub").Value;
            var isEmailVerified = jwtToken.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerId))
                throw new Exception("Failed to retrieve user information from Google.");

            var existingUser = await _userRepository.GetUserByEmail(email);

            if (existingUser != null)
            {
                var jwtTokenForExistingUser = _jwtService.GenerateJwtToken(_mapper.Map<UserGenerateTokenDto>(existingUser));

                return new AuthResponseDto
                {
                    Token = jwtTokenForExistingUser,
                    Expires = DateTime.UtcNow.AddHours(3),
                    User = _mapper.Map<UserDto>(existingUser)
                };
            }

            var userCreated = await _userRepository.AddUser(new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Provider = Constants.UserProvider.google,
                ProviderId = providerId,
                IsEmailConfirmed = isEmailVerified
            });

            if (userCreated == null)
                throw new Exception("Failed to create user in the database.");

            var jwtTokenForNewUser = _jwtService.GenerateJwtToken(_mapper.Map<UserGenerateTokenDto>(userCreated));

            return new AuthResponseDto
            {
                Token = jwtTokenForNewUser,
                Expires = DateTime.UtcNow.AddHours(3),
                User = _mapper.Map<UserDto>(userCreated)
            };
        }
    }
}
