using Booking.Application.Dtos;
using Booking.Domain.Entities;
using Mapster;

namespace Booking.Application.Mappings
{
    public class BookingMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BookingEntity, BookingDto>()
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.Payment, src => src.Payment)
                .Map(dest => dest.Cancellation, src => src.Cancellation);

            config.NewConfig<PaymentEntity, PaymentDto>()
                .Map(dest => dest.Provider, src => src.Provider.ToString())
                .Map(dest => dest.Method, src => src.Method != null ? src.Method.ToString() : null)
                .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<BookingCancellationEntity, BookingCancellationDto>()
                .Map(dest => dest.CancelledBy, src => src.CancelledBy.ToString());

            config.NewConfig<RefundEntity, RefundDto>()
                .Map(dest => dest.Status, src => src.Status.ToString());
        }
    }
}
