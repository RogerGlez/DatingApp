using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using API.Entities;

using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/userSeedData.json");
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        users.ForEach(user =>
        {
            user.UserName = user.UserName.Trim().ToLower();
            using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            user.PasswordSalt = hmac.Key;
            context.Users.Add(user);
        });
        await context.SaveChangesAsync();

    }
}