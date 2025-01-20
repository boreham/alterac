using Microsoft.Data.SqlClient;
using Nagrand.Models;
using System.Text;
using System.Security.Cryptography;

namespace Nagrand.Services;

public class UserService
{
    private readonly string _connectionString;

    public UserService(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Регистрация пользователя
    public bool RegisterUser(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            int userCount = (int)cmd.ExecuteScalar();

            if (userCount > 0)
                return false; // Имя пользователя уже существует

            var passwordHash = HashPassword(user.Password);

            cmd = new SqlCommand("INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)", connection);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", passwordHash);
            cmd.ExecuteNonQuery();
            return true;
        }
    }

    // Аутентификация пользователя
    public bool AuthenticateUser(string username, string password)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var cmd = new SqlCommand("SELECT Password FROM Users WHERE Username = @Username", connection);
            cmd.Parameters.AddWithValue("@Username", username);
            var passwordHash = cmd.ExecuteScalar() as string;

            if (passwordHash == null)
                return false;

            return VerifyPassword(password, passwordHash);
        }
    }

    // Хеширование пароля
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    // Проверка пароля
    private bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }
}
