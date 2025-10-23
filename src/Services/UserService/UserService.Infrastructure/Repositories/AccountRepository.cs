using Microsoft.EntityFrameworkCore;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation cho Account operations
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly UserDbContext _context;

        public AccountRepository(UserDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Đánh dấu account là inactive
        /// </summary>
        public async Task<bool> DeactivateAccountAsync(Guid accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.IdAccount == accountId);
            
            if (account == null)
                return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Kiểm tra account có tồn tại và active không
        /// </summary>
        public async Task<bool> IsAccountActiveAsync(Guid accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.IdAccount == accountId);
            
            return account?.IsActive ?? false;
        }

        /// <summary>
        /// Kích hoạt (mở khóa) account
        /// </summary>
        public async Task<bool> ActivateAccountAsync(Guid accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.IdAccount == accountId);

            if (account == null)
                return false;

            if (account.IsActive)
                return true; // already active

            account.IsActive = true;
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Lấy account theo Id
        /// </summary>
        public Task<UserService.Domain.Entities.AccountEntity?> GetByIdAsync(Guid accountId)
        {
            return _context.Accounts.FirstOrDefaultAsync(a => a.IdAccount == accountId)!;
        }

        public async Task<(List<UserService.Domain.Entities.AccountEntity> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? keyword = null, bool? isActive = null, string? roleName = null, string? sortBy = null, bool isAscending = true)
        {
            // Join with tbl_role to enable filtering by role
            var query = from account in _context.Accounts.AsNoTracking()
                        join role in _context.Roles on account.RoleId equals role.Id into roleGroup
                        from role in roleGroup.DefaultIfEmpty()
                        select new { Account = account, Role = role };

            if (isActive.HasValue)
                query = query.Where(x => x.Account.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                // Filter by role name (case-insensitive)
                query = query.Where(x => x.Role != null && x.Role.Name.ToString().ToLower() == roleName.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.ToLower();
                query = query.Where(x => x.Account.Email.ToLower().Contains(kw) || (x.Account.FullName ?? "").ToLower().Contains(kw));
            }

            // Sorting fallback
            query = !string.IsNullOrWhiteSpace(sortBy) ? (sortBy.ToLower() switch
            {
                "email" => isAscending ? query.OrderBy(x => x.Account.Email) : query.OrderByDescending(x => x.Account.Email),
                "fullname" => isAscending ? query.OrderBy(x => x.Account.FullName) : query.OrderByDescending(x => x.Account.FullName),
                "createdat" => isAscending ? query.OrderBy(x => x.Account.CreatedAt) : query.OrderByDescending(x => x.Account.CreatedAt),
                _ => isAscending ? query.OrderBy(x => x.Account.CreatedAt) : query.OrderByDescending(x => x.Account.CreatedAt)
            }) : query.OrderByDescending(x => x.Account.CreatedAt);

            var total = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query.Skip(skip).Take(pageSize).Select(x => x.Account).ToListAsync();

            return (items, total);
        }
    }
}