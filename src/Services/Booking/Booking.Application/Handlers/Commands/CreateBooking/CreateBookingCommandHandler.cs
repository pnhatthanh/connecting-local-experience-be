using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Booking.Application.Dtos;
using Booking.Application.Events;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using MapsterMapper;

namespace Booking.Application.Handlers.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, BookingDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExperienceService _experienceService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork,
            IExperienceService experienceService,
            ICurrentUserService currentUserService,
            IEventBus eventBus,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _experienceService = experienceService;
            _currentUserService = currentUserService;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // Validate experience exists and get details
            var experience = await _experienceService.GetExperienceAsync(request.ExperienceId);
            if (experience == null)
                throw new NotFoundException("Experience not found");

            if (experience.Status != "Published")
                throw new BadRequestException("Experience is not available for booking");

            // Validate availability
            var isAvailable = await _experienceService.ValidateAvailabilityAsync(
                request.ExperienceId, 
                request.StartTime, 
                request.EndTime, 
                request.Adults, 
                request.Children);

            if (!isAvailable)
                throw new BadRequestException("Selected time slot is not available");

            // Calculate total price
            var totalPrice = (experience.AdultPrice * request.Adults) + (experience.ChildPrice * request.Children);

            // Generate booking code
            var bookingCode = await _bookingRepository.GenerateBookingCodeAsync();

            // Create booking entity
            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                HostId = experience.HostId,
                ExperienceId = request.ExperienceId,
                BookingCode = bookingCode,
                Status = BookingStatus.Pending,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Adults = request.Adults,
                Children = request.Children,
                TotalPrice = totalPrice,
                ContactName = request.ContactName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            // Create payment record
            var payment = new PaymentEntity
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                Amount = totalPrice,
                Currency = "VND",
                Provider = PaymentProvider.VnPay,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);
            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangeAsync();

            // Publish event to update experience slots
            var bookingCreatedEvent = new BookingCreatedEvent(
                booking.Id,
                booking.ExperienceId,
                booking.UserId,
                booking.HostId,
                booking.BookingCode,
                booking.StartTime,
                booking.EndTime,
                booking.Adults,
                booking.Children,
                booking.TotalPrice,
                booking.ContactName,
                booking.ContactEmail
            );

            await _eventBus.PublishAsync(bookingCreatedEvent, cancellationToken);

            return _mapper.Map<BookingDto>(booking);
        }
    }
}
