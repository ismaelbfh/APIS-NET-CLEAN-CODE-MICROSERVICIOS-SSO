using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
        public int UserId { get; set; }
        public string ReplacedByToken { get; set; }

        private RefreshTokenEntity() { }

        public static RefreshTokenEntity Create(int userId, int refreshTokenExpirationDays)
        {
            return new RefreshTokenEntity
            {
                Token = GenerateRefreshTokenString(),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                UserId = userId
            };
        }

        public bool IsExpired() => DateTime.UtcNow > Expires;
        public bool IsActive() => !IsExpired() && Revoked == null;
        public void Revoke() => Revoked = DateTime.UtcNow;

        private static string GenerateRefreshTokenString()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                .Replace("/", "_")
                .Replace("+", "-");
        }
    }
}