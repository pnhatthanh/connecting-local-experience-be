using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    /// <summary>
    /// Repository interface cho Account operations
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Đánh dấu account là inactive
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>True nếu thành công</returns>
        Task<bool> DeactivateAccountAsync(Guid accountId);
        
        /// <summary>
        /// Kiểm tra account có tồn tại và active không
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>True nếu account tồn tại và active</returns>
        Task<bool> IsAccountActiveAsync(Guid accountId);

        /// <summary>
        /// Kích hoạt (mở khóa) account
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>True nếu kích hoạt thành công</returns>
        Task<bool> ActivateAccountAsync(Guid accountId);

        /// <summary>
        /// Lấy thông tin account theo Id
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>AccountEntity hoặc null nếu không tồn tại</returns>
        Task<AccountEntity?> GetByIdAsync(Guid accountId);
        
        /// <summary>
        /// Lấy danh sách tài khoản có phân trang và lọc cơ bản
        /// Trả về items và tổng số phần tử phù hợp
        /// </summary>
        Task<(List<AccountEntity> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? keyword = null, bool? isActive = null, string? roleName = null, string? sortBy = null, bool isAscending = true);
    }
}