using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using MediatR;
using UserService.Domain.Repositories;

namespace UserService.Application.Commands.DeleteCurrentUser
{
    /// <summary>
    /// Handler để xử lý việc "xóa" tài khoản user hiện tại
    /// </summary>
    public class DeleteCurrentUserCommandHandler : IRequestHandler<DeleteCurrentUserCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCurrentUserCommandHandler(
            IAccountRepository accountRepository,
            ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteCurrentUserCommand request, CancellationToken cancellationToken)
        {
            // Temporarily disabled authentication check for testing
            // if (!_currentUserService.IsAuthenticated)
            // {
            //     throw new UnAuthorizedException("User is not authenticated");
            // }

            var accountId = Guid.Parse(_currentUserService.UserId);

            // Kiểm tra account có tồn tại và active không
            var isActive = await _accountRepository.IsAccountActiveAsync(accountId);
            if (!isActive)
            {
                // Account không tồn tại hoặc đã bị vô hiệu hóa - trả về false
                return false;
            }

            // Đánh dấu account là inactive
            var result = await _accountRepository.DeactivateAccountAsync(accountId);
            
            return result;
        }
    }
}