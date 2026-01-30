using System.Collections.Generic;
using System.Threading.Tasks;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Domain.Interfaces;

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<IEnumerable<PaymentTransaction>> GetByReservationIdAsync(int reservationId);
    Task<IEnumerable<PaymentTransaction>> GetByUserIdAsync(int userId);
}
