using Application.Contracts.Services;
using Shared.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using System.Text;
using Shared.Exceptions;
using Shared.Enums;

namespace Infrastructure.Authentication.PasswordHashing
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly Argon2HashingOptions _options;

        public PasswordHasher(IOptions<Argon2HashingOptions> options)
        {
            _options = options.Value;
        }

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 64)
                throw new DomainException("Senha deve possuir entre 8 e 64 caracteres", ErrorType.InvalidInput);

            // Gerar salt aleatório
            var salt = new byte[_options.SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Criar hash Argon2id
            var hash = HashPassword(password, salt);

            // Combinar salt + hash em uma string base64
            var combinedBytes = new byte[_options.SaltSize + _options.HashSize];
            Buffer.BlockCopy(salt, 0, combinedBytes, 0, _options.SaltSize);
            Buffer.BlockCopy(hash, 0, combinedBytes, _options.SaltSize, _options.HashSize);

            return Convert.ToBase64String(combinedBytes);
        }

        [Obsolete("Será movido para lambda de validação")]
        public bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                var combinedBytes = Convert.FromBase64String(hash);
                if (combinedBytes.Length != _options.SaltSize + _options.HashSize)
                    return false;

                // Extrair salt e hash
                var salt = new byte[_options.SaltSize];
                var originalHash = new byte[_options.HashSize];
                Buffer.BlockCopy(combinedBytes, 0, salt, 0, _options.SaltSize);
                Buffer.BlockCopy(combinedBytes, _options.SaltSize, originalHash, 0, _options.HashSize);

                // Recriar hash com a mesma senha e salt
                var newHash = HashPassword(password, salt);

                // Comparar hashes de forma segura (constant-time)
                return ConstantTimeEquals(originalHash, newHash);
            }
            catch
            {
                return false;
            }
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = _options.DegreeOfParallelism,
                Iterations = _options.Iterations,
                MemorySize = _options.MemorySize
            };

            return argon2.GetBytes(_options.HashSize);
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}