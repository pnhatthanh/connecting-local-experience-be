using MediatR;

namespace UserService.Application.Commands.DeleteCurrentUser
{
    /// <summary>
    /// Command để "xóa" tài khoản user hiện tại (đánh dấu inactive)
    /// </summary>
    public class DeleteCurrentUserCommand : IRequest<bool>
    {
        // Không cần properties vì lấy userId từ CurrentUserService
    }
}