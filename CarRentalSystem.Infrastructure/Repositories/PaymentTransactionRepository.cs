using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Interfaces;
using CarRentalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Repositories;

public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentTransaction?> GetByIdAsync(int id)
    {
        return await _context.PaymentTransactions
            .Include(p => p.Reservation)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PaymentTransaction>> GetAllAsync()
    {
        return await _context.PaymentTransactions
            .Include(p => p.Reservation)
            .Include(p => p.User)
            .ToListAsync();
    }

    public async Task<PaymentTransaction> AddAsync(PaymentTransaction entity)
    {
        await _context.PaymentTransactions.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(PaymentTransaction entity)
    {
        _context.PaymentTransactions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.PaymentTransactions.FindAsync(id);
        if (entity != null)
        {
            _context.PaymentTransactions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.PaymentTransactions.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<PaymentTransaction>> GetByReservationIdAsync(int reservationId)
    {
        return await _context.PaymentTransactions
            .Where(p => p.ReservationId == reservationId)
            .OrderByDescending(p => p.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentTransaction>> GetByUserIdAsync(int userId)
    {
        return await _context.PaymentTransactions
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.TransactionDate)
            .ToListAsync();
    }
}
