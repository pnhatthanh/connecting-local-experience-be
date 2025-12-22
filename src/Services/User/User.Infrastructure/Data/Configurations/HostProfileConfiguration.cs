using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Infrastructure.Data.Configurations
{
    public class HostProfileConfiguration : IEntityTypeConfiguration<HostProfileEntity>
    {
        public void Configure(EntityTypeBuilder<HostProfileEntity> builder)
        {
            builder.ToTable("tbl_host_profiles");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();

            var jsonArrayConverter = new ValueConverter<string[], string?>(
                v => v == null || v.Length == 0 ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v) ? Array.Empty<string>() : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<string>()
            );

            builder.Property(e => e.Bio).HasColumnName("bio").HasColumnType("TEXT");
            builder.Property(e => e.SpokenLanguages)
                .HasColumnName("spoken_languages")
                .HasColumnType("jsonb")
                .HasConversion(jsonArrayConverter);

            builder.Property(e => e.TopicsOfInterest)
                .HasColumnName("topics_of_interest")
                .HasColumnType("jsonb")
                .HasConversion(jsonArrayConverter);

            builder.Property(e => e.VerifyStatus)
                .HasColumnName("verify_status")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<VerifyStatus>(v, true)
                )
                .HasDefaultValue(VerifyStatus.Pending);
            builder.Property(e => e.DocumentUrl).HasColumnName("document_url").HasMaxLength(500);
            builder.Property(e => e.VerifyReason).HasColumnName("verify_reason").HasColumnType("TEXT");
            builder.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            builder.Property(e => e.ResponseTime).HasColumnName("response_time");
            builder.Property(e => e.TotalExperiences).HasColumnName("total_experiences").HasDefaultValue(0);
            builder.Property(e => e.TotalBookings).HasColumnName("total_bookings").HasDefaultValue(0);
            builder.Property(e => e.TotalReviews).HasColumnName("total_reviews").HasDefaultValue(0);
            builder.Property(e => e.RatingAvg).HasColumnName("rating_avg").HasColumnType("DECIMAL(3,2)");
            builder.Property(e => e.Work).HasColumnName("work").HasMaxLength(200);
            builder.Property(e => e.Education).HasColumnName("education").HasMaxLength(200);
            builder.Property(e => e.FunFact).HasColumnName("fun_fact").HasMaxLength(500);

            builder.Property(e => e.TopicsOfInterest)
                .HasColumnName("topics_of_interest")
                .HasColumnType("jsonb")
                .HasConversion(jsonArrayConverter);
            builder.Property(e => e.DesiredHostingStyle).HasColumnName("desired_hosting_style").HasMaxLength(500);
            builder.Property(e => e.FacebookUrl).HasColumnName("facebook_url").HasMaxLength(500);
            builder.Property(e => e.InstagramUrl).HasColumnName("instagram_url").HasMaxLength(500);
            builder.Property(e => e.LinkedInUrl).HasColumnName("linkedin_url").HasMaxLength(500);
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            builder.HasIndex(e => e.UserId).IsUnique();
            builder.HasIndex(e => e.VerifyStatus);

            builder.HasOne(e => e.User)
                .WithOne(u => u.HostProfile)
                .HasForeignKey<HostProfileEntity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
