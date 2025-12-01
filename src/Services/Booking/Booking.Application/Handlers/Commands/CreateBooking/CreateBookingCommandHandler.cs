using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Booking.Application.Dtos;
using Booking.Application.Events;
using Booking.Application.Interfaces;
using Booking.Application.Utils;
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
        private readonly IMomoService _momoService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork,
            IExperienceService experienceService,
            IMomoService momoService,
            ICurrentUserService currentUserService,
            IEventBus eventBus,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _experienceService = experienceService;
            _momoService = momoService;
            _currentUserService = currentUserService;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var experience = await _experienceService.GetExperienceAsync(request.ExperienceId) 
                ?? throw new BadRequestException("Experience not found");
            var isAvailable = await _experienceService.ValidateAvailabilityAsync(
                request.ExperienceId, 
                request.Date,
                request.StartTime, 
                request.EndTime, 
                request.Adults, 
                request.Children);

            if (!isAvailable)
                throw new BadRequestException("Selected time slot is not available");
            

            var totalPrice = (experience.AdultPrice * request.Adults) + (experience.ChildPrice * request.Children);

            var platformFeePercentage = 0.15m;
            var platformFee = totalPrice * platformFeePercentage;
            var hostAmount = totalPrice - platformFee;  // Host receives 85%

            var bookingCode = BookingCodeGenerator.Generate();

            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                HostId = experience.HostId,
                ExperienceId = request.ExperienceId,
                BookingCode = bookingCode,
                Status = BookingStatus.Pending,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Adults = request.Adults,
                Children = request.Children,
                TotalPrice = totalPrice,            
                PlatformFee = platformFee,            
                HostAmount = hostAmount,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var payment = new PaymentEntity
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                Amount = totalPrice,
                Currency = "VND",
                Provider = request.PaymentProvider,  
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _bookingRepository.AddAsync(booking);
            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangeAsync();

            // Create payment URL immediately after booking creation
            var paymentUrlResult = await _momoService.CreatePaymentUrlAsync(
                booking.Id,
                totalPrice,
                bookingCode,
                "127.0.0.1"); // IP address will be set from controller

            if (paymentUrlResult.success && !string.IsNullOrEmpty(paymentUrlResult.paymentUrl))
            {
                payment.PaymentUrl = paymentUrlResult.paymentUrl;
                await _unitOfWork.SaveChangeAsync();
            }

            var bookingCreatedEvent = new BookingCreatedEvent(
                booking.Id,
                booking.ExperienceId,
                booking.UserId,
                booking.HostId,
                booking.BookingCode,
                booking.Date,
                booking.StartTime,
                booking.EndTime,
                booking.Adults,
                booking.Children,
                booking.TotalPrice,
                booking.FirstName + " " + booking.LastName,
                booking.ContactEmail
            );
            await _eventBus.PublishAsync(bookingCreatedEvent, cancellationToken);

            var bookingDto = _mapper.Map<BookingDto>(booking);
            
            // Include payment info with URL in response
            if (payment.PaymentUrl != null)
            {
                bookingDto.Payment = new PaymentDto
                {
                    Id = payment.Id,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    Provider = payment.Provider.ToString(),
                    Status = payment.Status.ToString(),
                    PaymentUrl = payment.PaymentUrl,
                    CreatedAt = payment.CreatedAt
                };
            }

            return bookingDto;
        }
    }
}
