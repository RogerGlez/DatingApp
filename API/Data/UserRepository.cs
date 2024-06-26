﻿using API.Entities;
using API.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByNameAsync(string username)
    {
        return await _context.Users.SingleOrDefaultAsync(user => user.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users.AsNoTracking().ToArrayAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        var records = await _context.SaveChangesAsync();
        return records > 0;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
