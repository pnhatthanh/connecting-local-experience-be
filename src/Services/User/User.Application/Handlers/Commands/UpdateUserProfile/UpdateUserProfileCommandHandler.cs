using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using MapsterMapper;
using User.Application.DTOs;
using User.Application.Interfaces;
using User.Domain.Enums;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, 
            IPhotoService photoService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(_currentUserService.UserId);
            var user = await _userRepository.GetBySpecAsync(userSpec)
                ?? throw new BadRequestException($"User with Id {_currentUserService.UserId} not found");
            
            user.PhoneNumber = request.PhoneNumber;
            user.FullName = request.FullName ?? user.FullName;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender != null ? Enum.Parse<Gender>(request.Gender) : user.Gender;
            user.Country = request.Country;
            if (request.Avatar != null)
            {
                if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
                    await _photoService.DeleteImageAsync(user.AvatarUrl);
                user.AvatarUrl = await _photoService.UploadImageAsync(request.Avatar, "users/avatars");
            }  
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            
            return _mapper.Map<UserDto>(user);
        }
    }
}
