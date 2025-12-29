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
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExperienceService _experienceService;
        private readonly IVnPayService _vnPayService;
        private readonly ICurrentUserService _currentUserService;

        public CreateBookingCommandHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork, IExperienceService experienceService, IVnPayService vnPayService,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _experienceService = experienceService;
            _vnPayService = vnPayService;
            _currentUserService = currentUserService;
        }

        public async Task<CreateBookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var experience = await _experienceService.GetExperienceAsync(request.ExperienceId) 
                ?? throw new BadRequestException("Experience not found");
            if (request.Date <=  DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BadRequestException("Booking date must be in the future");
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
            var bookingCode = BookingCodeGenerator.Generate();
            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                HostId = experience.HostId,
                ExperienceId = request.ExperienceId,
                ExperienceTitle = experience.Title,
                ImageUrl = experience.Media != null && experience.Media.Any() ? experience.Media[0].Url : string.Empty,
                BookingCode = bookingCode,
                Status = BookingStatus.Pending,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Location = experience.City + ", " + experience.Country,
                Adults = request.Adults,
                Children = request.Children,
                TotalPrice = totalPrice,        
                FirstName = request.FirstName,
                LastName = request.LastName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Notes = request.Notes,
                ClientType = request.ClientType == "App" ? ClientType.App : ClientType.Web,
                CreatedAt = DateTime.UtcNow
            };

            var payment = new PaymentEntity
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                Amount = totalPrice,
                Currency = "VND",
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var paymentUrlResult = await _vnPayService.CreatePaymentUrlAsync(
                booking.Id,
                totalPrice,
                bookingCode);
            if (paymentUrlResult.success && !string.IsNullOrEmpty(paymentUrlResult.paymentUrl))
            {
                payment.PaymentUrl = paymentUrlResult.paymentUrl;
                payment.TransactionId = paymentUrlResult.transactionId;
            }
            await _bookingRepository.AddAsync(booking);
            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangeAsync();
            return new CreateBookingResponse(
                Success: true,
                PaymentUrl: payment.PaymentUrl,
                Message: payment.PaymentUrl != null 
                    ? "Booking created successfully. Please proceed to payment." 
                    : "Booking created but payment URL not available"
            );
        }
    }
}
