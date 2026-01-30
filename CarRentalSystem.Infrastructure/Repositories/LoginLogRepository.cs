using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Interfaces;
using CarRentalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Repositories;

public class LoginLogRepository : ILoginLogRepository
{
    private readonly ApplicationDbContext _context;

    public LoginLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LoginLog?> GetByIdAsync(int id)
    {
        return await _context.LoginLogs.FindAsync(id);
    }

    public async Task<IEnumerable<LoginLog>> GetAllAsync()
    {
        return await _context.LoginLogs.ToListAsync();
    }

    public async Task<LoginLog> AddAsync(LoginLog entity)
    {
        await _context.LoginLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(LoginLog entity)
    {
        _context.LoginLogs.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.LoginLogs.FindAsync(id);
        if (entity != null)
        {
            _context.LoginLogs.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.LoginLogs.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<LoginLog>> GetLogsByUserIdAsync(int userId)
    {
        return await _context.LoginLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.AttemptDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<LoginLog>> GetRecentLogsAsync(int count)
    {
        return await _context.LoginLogs
            .OrderByDescending(l => l.AttemptDate)
            .Take(count)
            .ToListAsync();
    }
}
