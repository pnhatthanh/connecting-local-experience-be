using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
            builder.Property(e => e.Bio).HasColumnName("bio").HasColumnType("TEXT");
            builder.Property(e => e.SpokenLanguages)
                .HasColumnName("spoken_languages")
                .HasColumnType("jsonb");
            builder.Property(e => e.Location).HasColumnName("location").HasMaxLength(255);
            builder.Property(e => e.HostingSince).HasColumnName("hosting_since");
            builder.Property(e => e.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
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
            builder.Property(e => e.ResponseTime).HasColumnName("response_time").HasMaxLength(50);
            builder.Property(e => e.TotalExperiences).HasColumnName("total_experiences").HasDefaultValue(0);
            builder.Property(e => e.TotalBookings).HasColumnName("total_bookings").HasDefaultValue(0);
            builder.Property(e => e.TotalReviews).HasColumnName("total_reviews").HasDefaultValue(0);
            builder.Property(e => e.RatingAvg).HasColumnName("rating_avg").HasColumnType("DECIMAL(3,2)");
            builder.Property(e => e.Work).HasColumnName("work").HasMaxLength(200);
            builder.Property(e => e.Education).HasColumnName("education").HasMaxLength(200);
            builder.Property(e => e.FunFact).HasColumnName("fun_fact").HasMaxLength(500);
            builder.Property(e => e.TopicsOfInterest)
                .HasColumnName("topics_of_interest")
                .HasColumnType("jsonb");
            builder.Property(e => e.FacebookUrl).HasColumnName("facebook_url").HasMaxLength(500);
            builder.Property(e => e.InstagramUrl).HasColumnName("instagram_url").HasMaxLength(500);
            builder.Property(e => e.LinkedInUrl).HasColumnName("linkedin_url").HasMaxLength(500);
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            builder.HasIndex(e => e.UserId).IsUnique();
            builder.HasIndex(e => e.VerifyStatus);
            
            // Relationship
            builder.HasOne(e => e.User)
                .WithOne(u => u.HostProfile)
                .HasForeignKey<HostProfileEntity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
