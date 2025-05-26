using InternPulse4.Core.Application.Interfaces.Repositories;
using InternPulse4.Infrastructure.Context;

namespace InternPulse4.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InternPulseContext _context;

        public UnitOfWork(InternPulseContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
