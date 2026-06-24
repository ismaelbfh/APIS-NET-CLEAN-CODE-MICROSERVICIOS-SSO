using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Entities;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Goikoa.Domain.ApiAdmin.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AuthService(IRoleRepository pRoleRepository, IUserRepository pUserRepository, IRefreshTokenRepository pRefreshTokenRepository, IMapper pMapper, IConfiguration pConfiguration)
        {
            _roleRepository = pRoleRepository;
            _userRepository = pUserRepository;
            _mapper = pMapper;
            _configuration = pConfiguration;
            _refreshTokenRepo = pRefreshTokenRepository;
        }

         public async Task<TokenResponseDTO> RegisterAsync(UserRequest pUserDto)
         {
            // 1. Validar si el usuario ya existe
            var lExistingUser = await _userRepository.GetUserByNameAsync(pUserDto.Username);
            if (lExistingUser != null)
                throw new ApplicationException("El usuario ya existe, no puedes registrar el mismo usuario.");

            // 2. Crear nuevo usuario
            UserEntity lNewUser = UserEntity.CreateUser(pUserDto.Username, pUserDto.Password, pUserDto.FkRoleId);

            lNewUser = await _userRepository.AddAsync(lNewUser);

            // 3. Generar tokens
            return await GenerateTokens(lNewUser);
         }

        public async Task<TokenResponseDTO> LoginAsync(UserRequest pLoginDto)
        {
            // 1. Buscar usuario
            UserEntity? lUser = await _userRepository.GetUserByNameAsync(pLoginDto.Username);
        
            if (lUser == null || !lUser.ValidatePassword(pLoginDto.Password))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            // 2. Revocar tokens anteriores del usuario
            await RevokeAllRefreshTokensForUser(lUser.PkIdUser);

            // 3. Generar nuevos tokens
            return await GenerateTokens(lUser);
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            // 1. Validar refresh token
            var storedRefreshToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
            if (storedRefreshToken == null)
                throw new UnauthorizedAccessException("Refresh token no válido");

            if (storedRefreshToken == null || !storedRefreshToken.IsActive())
                throw new UnauthorizedAccessException("Refresh token no válido o inactivo");

            // 2. Obtener usuario asociado
            var user = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // 3. Revocar el refresh token actual
            storedRefreshToken.Revoke();
            await _refreshTokenRepo.UpdateAsync(storedRefreshToken);

            // 4. Generar nuevos tokens
            return await GenerateTokens(user);
        }

        private async Task<TokenResponseDTO> GenerateTokens(UserEntity user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateAndSaveRefreshToken(user.PkIdUser);

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]))
            };
        }

        private string GenerateAccessToken(UserEntity user)
        {
            var role = _roleRepository.GetRoleByIdAsync(user.FkRoleId);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.PkIdUser.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role.NombreRol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshTokenEntity> GenerateAndSaveRefreshToken(int pUserId)
        {
            int refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]);
            
            // Usamos el método de fábrica de la entidad para crear el refresh token
            RefreshTokenEntity lRefreshToken = RefreshTokenEntity.Create(pUserId, refreshTokenExpirationDays);

            await _refreshTokenRepo.AddAsync(lRefreshToken);
            return lRefreshToken;
        }

        private async Task RevokeAllRefreshTokensForUser(int pUserId)
        {
            List<RefreshTokenEntity>? lListTokens = await _refreshTokenRepo.GetActiveTokensForUserAsync(pUserId);
            foreach (RefreshTokenEntity? lToken in lListTokens)
            {
                lToken.Revoke();
                await _refreshTokenRepo.UpdateAsync(lToken);
            }
        }
    }
}
