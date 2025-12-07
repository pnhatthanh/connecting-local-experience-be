using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;

namespace Experience.Application.Handlers.Queries.GetReviewsByExperience
{
    public class GetReviewsByExperienceQueryHandler : IQueryHandler<GetReviewsByExperienceQuery, PaginationResult<ReviewDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IMapper _mapper;

        public GetReviewsByExperienceQueryHandler(IReviewRepository reviewRepository, IUserServiceClient userServiceClient, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _userServiceClient = userServiceClient;
            _mapper = mapper;
        }

        public async Task<PaginationResult<ReviewDto>> Handle(GetReviewsByExperienceQuery request, CancellationToken cancellationToken)
        {
            var specification = new ReviewByExperienceIdSpecification(request.ExperienceId);
            var reviews = await _reviewRepository.GetPagedListAsync(
                specification: specification.And(new ReviewByIsHiddenSpecification(false)), 
                pageNumber: request.PageNumber, 
                pageSize: request.PageSize, 
                sortBy: "CreatedAt",
                isAscending: false);
            
            var userIds = reviews.Select(r => r.UserId).Distinct().ToList();
            var usersInfo = await _userServiceClient.GetUsersInfoAsync(userIds);
            var reviewDtos = reviews.Select(review =>
            {
                var dto = _mapper.Map<ReviewDto>(review);
                if (usersInfo.TryGetValue(review.UserId, out var userInfo))
                {
                    dto = dto with
                    {
                        FullName = userInfo.FullName,
                        UserAvatar = userInfo.Avatar
                    };
                }
                return dto;
            }).ToList();
            
            return new PaginationResult<ReviewDto>
            {
                Data = reviewDtos,
                TotalCount = await _reviewRepository.CountAsync(specification),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
