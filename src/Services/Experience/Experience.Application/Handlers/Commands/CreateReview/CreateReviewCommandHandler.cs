using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Events;
using Experience.Application.Interfaces;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using MapsterMapper;

namespace Experience.Application.Handlers.Commands.CreateReview
{
    public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly IBookingServiceClient _bookingServiceClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public CreateReviewCommandHandler(IReviewRepository reviewRepository, IExperienceRepository experienceRepository,
            IBookingServiceClient bookingServiceClient, ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork, IMapper mapper, IEventBus eventBus)
        {
            _reviewRepository = reviewRepository;
            _experienceRepository = experienceRepository;
            _bookingServiceClient = bookingServiceClient;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId) 
                ?? throw new NotFoundException("Experience not found");

            // Check if user has completed a booking for this experience
            var hasBooked = await _bookingServiceClient.HasUserBookedExperienceAsync(userId, request.ExperienceId);
            if (!hasBooked)
                throw new BadRequestException("You must complete a booking for this experience before reviewing");

            // Create review
            var review = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                ExperienceId = request.ExperienceId,
                UserId = userId,
                HostId = experience.HostId,
                Rating = request.Rating,
                Description = request.Description,
                IsHidden = false,
                CreatedAt = DateTime.UtcNow
            };
            await _reviewRepository.AddAsync(review);

            experience.TotalReviews ++;
            experience.AverageRating = ((experience.AverageRating * (experience.TotalReviews - 1)) + request.Rating) / experience.TotalReviews;
            _experienceRepository.Update(experience);   

            await _unitOfWork.SaveChangeAsync();
            // Publish event
            var userRatedEvent = new UserRatedExperienceEvent(
                userId,
                experience.HostId,
                request.ExperienceId, 
                request.Rating);
            await _eventBus.PublishAsync(userRatedEvent);

            return _mapper.Map<ReviewDto>(review);
        }
    }
}
