using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PropertyRentals.Models;

public partial class PropertyRentalDbContext : DbContext
{
    public PropertyRentalDbContext()
    {
    }

    public PropertyRentalDbContext(DbContextOptions<PropertyRentalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Apartment> Apartments { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-VGGP11IP\\SQLEXPRESS;Initial Catalog=PropertyRentalDB;User=sa;Password=Andrea38;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.Property(e => e.ApartmentCode).HasMaxLength(20);
            entity.Property(e => e.PropertyCode).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(10);


            entity.HasOne(d => d.PropertyCodeNavigation).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.PropertyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_Properties");

            entity.HasOne(d => d.Status).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_Status1");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");

            entity.HasOne(d => d.Apartment).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Apartments");

            entity.HasOne(d => d.Manager).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Managers");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Status");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Tenants");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.EventId).HasColumnName("EventID");

            entity.HasOne(d => d.Apartment).WithMany(p => p.Events)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Apartments");

            entity.HasOne(d => d.Status).WithMany(p => p.Events)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Status");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Managers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Managers_Users");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.MessageDateTime).HasColumnType("datetime");
            entity.Property(e => e.ReceiverUserId).HasColumnName("ReceiverUserID");
            entity.Property(e => e.SenderUserId).HasColumnName("SenderUserID");
            entity.Property(e => e.Subject).HasMaxLength(90);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Apartments");

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.MessageReceiverUsers)
                .HasForeignKey(d => d.ReceiverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users1");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.MessageSenderUsers)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users");

            entity.HasOne(d => d.Status).WithMany(p => p.Messages)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Status");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Owners)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Owners_Users");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.Property(e => e.PhotoLink).HasMaxLength(200);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Photos)
                .HasForeignKey(d => d.ApartmentId)
                .HasConstraintName("FK_Photos_Apartments");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.PropertyCode);

            entity.Property(e => e.PropertyCode).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(20);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.Name).HasMaxLength(70);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.State).HasMaxLength(20);
            entity.Property(e => e.ZipCode).HasMaxLength(6);

            entity.HasOne(d => d.Manager).WithMany(p => p.Properties)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Properties_Managers");

            entity.HasOne(d => d.Owner).WithMany(p => p.Properties)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Properties_Owners");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasOne(d => d.Apartment).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentals_Apartments");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentals_Tenants");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Tenants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tenants_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserType).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
