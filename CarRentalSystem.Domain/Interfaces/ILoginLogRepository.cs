using System.Collections.Generic;
using System.Threading.Tasks;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Domain.Interfaces;

public interface ILoginLogRepository : IRepository<LoginLog>
{
    Task<IEnumerable<LoginLog>> GetLogsByUserIdAsync(int userId);
    Task<IEnumerable<LoginLog>> GetRecentLogsAsync(int count);
}
