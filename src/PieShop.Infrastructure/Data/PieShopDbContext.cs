using Microsoft.EntityFrameworkCore;
using PieShop.Core.Models;
using PieShop.Core.StateMachine;

namespace PieShop.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for PostgreSQL
/// </summary>
public class PieShopDbContext : DbContext
{
    public PieShopDbContext(DbContextOptions<PieShopDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<StateTransition> StateHistory { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            
            entity.HasKey(o => o.Id);
            
            entity.Property(o => o.Id)
                .HasColumnName("id");
            
            entity.Property(o => o.PieType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("pie_type");
            
            entity.Property(o => o.CurrentState)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("current_state");
            
            entity.Property(o => o.PickerJobId)
                .HasMaxLength(255)
                .HasColumnName("picker_job_id");
            
            entity.Property(o => o.BakerJobId)
                .HasMaxLength(255)
                .HasColumnName("baker_job_id");
            
            entity.Property(o => o.DeliveryId)
                .HasMaxLength(255)
                .HasColumnName("delivery_id");
            
            entity.Property(o => o.EstimatedDelivery)
                .HasColumnName("estimated_delivery");
            
            entity.Property(o => o.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");
            
            entity.Property(o => o.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");

            // Configure Customer value object (owned entity)
            entity.OwnsOne(o => o.Customer, customer =>
            {
                customer.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("customer_name");
                
                customer.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("customer_email");
                
                customer.Property(c => c.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("customer_phone");
            });

            // Configure DeliveryAddress value object (owned entity)
            entity.OwnsOne(o => o.DeliveryAddress, address =>
            {
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("delivery_street");
                
                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("delivery_city");
                
                address.Property(a => a.State)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("delivery_state");
                
                address.Property(a => a.Zip)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("delivery_zip");
            });

            // Configure relationship with StateTransition
            entity.HasMany(o => o.History)
                .WithOne()
                .HasForeignKey(st => st.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes
            entity.HasIndex(o => o.CurrentState)
                .HasDatabaseName("idx_orders_state");
            
            entity.HasIndex(o => o.CreatedAt)
                .HasDatabaseName("idx_orders_created");
        });

        // Configure StateTransition entity
        modelBuilder.Entity<StateTransition>(entity =>
        {
            entity.ToTable("state_history");
            
            entity.HasKey(st => st.Id);
            
            entity.Property(st => st.Id)
                .HasColumnName("id");
            
            entity.Property(st => st.OrderId)
                .IsRequired()
                .HasColumnName("order_id");
            
            entity.Property(st => st.FromState)
                .HasConversion<string>()
                .HasColumnName("from_state");
            
            entity.Property(st => st.ToState)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("to_state");
            
            entity.Property(st => st.Timestamp)
                .IsRequired()
                .HasColumnName("timestamp");
            
            entity.Property(st => st.Notes)
                .HasColumnName("notes");
            
            entity.Property(st => st.ErrorMessage)
                .HasColumnName("error_message");

            // Add index
            entity.HasIndex(st => new { st.OrderId, st.Timestamp })
                .HasDatabaseName("idx_history_order");
        });
    }
}
