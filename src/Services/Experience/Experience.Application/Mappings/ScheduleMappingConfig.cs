using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Mapster;

namespace Experience.Application.Mappings
{
    public class ScheduleMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ExperienceScheduleEntity, ExperienceScheduleDto>()
                .Map(dest => dest.RecurrenceType, src => src.RecurrenceType.ToString())
                .Map(dest => dest.DaysOfWeek, src => src.DaysOfWeek)
                .Map(dest => dest.TimeSlots, src => src.TimeSlots);
            config.NewConfig<ScheduleTimeSlot, TimeSlotDto>();
            config.NewConfig<ExperienceScheduleSlotEntity, ExperienceScheduleSlotDto>()
                .Map(dest => dest.Status, src => src.Status.ToString());
        }
    }
}
