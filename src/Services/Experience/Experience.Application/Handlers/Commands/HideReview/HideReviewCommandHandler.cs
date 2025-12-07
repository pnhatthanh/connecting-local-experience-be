using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Repositories;

namespace Experience.Application.Handlers.Commands.HideReview
{
    public class HideReviewCommandHandler : ICommandHandler<HideReviewCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HideReviewCommandHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(HideReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId) 
                ?? throw new BadRequestException("Review not found");
            if (review.IsHidden == request.IsHidden)
                throw new BadRequestException("Review is already in the desired state");

            review.IsHidden = request.IsHidden;
            review.UpdatedAt = DateTime.UtcNow;
            _reviewRepository.Update(review);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
