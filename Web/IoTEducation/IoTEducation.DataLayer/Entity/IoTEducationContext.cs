using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace IoTEducation.DataLayer.Entity
{
    public partial class IoTEducationContext : DbContext
    {
        public IoTEducationContext()
        {
        }

        public IoTEducationContext(DbContextOptions<IoTEducationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<IotDevice> IotDevices { get; set; }
        public virtual DbSet<IotDeviceDatum> IotDeviceData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                var config = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("appsettings.json", true, true)
                                       .Build();

                if (config is not null)
                {
                    optionsBuilder.UseSqlServer(config.GetConnectionString("DataConnection"));
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<IotDevice>(entity =>
            {
                entity.ToTable("IOT_Device");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(5)
                    .UseCollation("Turkish_CI_AS");

                entity.Property(e => e.LastAccessDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .UseCollation("Turkish_CI_AS");
            });

            modelBuilder.Entity<IotDeviceDatum>(entity =>
            {
                entity.ToTable("IOT_DeviceData");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DeviceDate).HasColumnType("datetime");

                entity.Property(e => e.DeviceId).HasColumnName("DeviceID");

                entity.Property(e => e.Humidity).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Latitude).HasColumnType("decimal(12, 8)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(12, 8)");

                entity.Property(e => e.RawData)
                    .IsRequired()
                    .HasMaxLength(500)
                    .UseCollation("Turkish_CI_AS");

                entity.Property(e => e.Temperature).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.IotDeviceData)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IOT_DeviceData_IOT_Device");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
