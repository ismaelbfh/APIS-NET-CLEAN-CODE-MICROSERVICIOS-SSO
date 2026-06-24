using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.Entities
{
    public class UserEntity
    {
        public int PkIdUser { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int? FkRoleId { get; set; }

        private UserEntity() { } // Constructor privado para forzar el uso del método de fábrica

        public static UserEntity CreateUser(string pUsername, string pPlainPassword, int? pRoleId)
        {
            if (string.IsNullOrWhiteSpace(pUsername))
                throw new ArgumentException("El nombre de usuario es requerido");

            if (string.IsNullOrWhiteSpace(pPlainPassword))
                throw new ArgumentException("La contraseña es requerida");

            return new UserEntity
            {
                PkIdUser = 0,
                Username = pUsername,
                Password = BCrypt.Net.BCrypt.HashPassword(pPlainPassword),
                FkRoleId = pRoleId
            };
        }

        // Método para validar la contraseña ingresada
        public bool ValidatePassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, this.Password);
        }
    }
}