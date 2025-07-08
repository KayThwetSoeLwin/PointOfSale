using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PointOfSale.Database.AppDbContextModels
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleDetail> SaleDetails { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }

        // ✅ Removed OnConfiguring method — no hardcoded connection anymore
        // It will be configured in Program.cs via Dependency Injection

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductCode).HasName("PK__product__AE1A8CC57CBF433D");
                entity.ToTable("product");

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(50)
                    .HasColumnName("product_code");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.InActive)
                    .HasDefaultValue(false)
                    .HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");
                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("product_name");
                entity.Property(e => e.StockQuantity)
                    .HasDefaultValue(0)
                    .HasColumnName("stock_quantity");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.SaleId).HasName("PK__sale__E1EB00B277A95327");
                entity.ToTable("sale");

                entity.HasIndex(e => e.VoucherCode, "UQ__sale__21731069B41EBD53").IsUnique();

                entity.Property(e => e.SaleId).HasColumnName("sale_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.InActive)
                    .HasDefaultValue(false)
                    .HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
                entity.Property(e => e.SaleDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("sale_date");
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_amount");
                entity.Property(e => e.VoucherCode)
                    .HasMaxLength(50)
                    .HasColumnName("voucher_code");
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.HasKey(e => e.SaleDetailId).HasName("PK__sale_det__4D671D835D70052E");
                entity.ToTable("sale_details");

                entity.Property(e => e.SaleDetailId).HasColumnName("sale_detail_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.InActive)
                    .HasDefaultValue(false)
                    .HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");
                entity.Property(e => e.ProductCode)
                    .HasMaxLength(50)
                    .HasColumnName("product_code");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.VoucherCode)
                    .HasMaxLength(50)
                    .HasColumnName("voucher_code");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__staff__1963DD9CC8272A77");
                entity.ToTable("staff");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.Designation)
                    .HasMaxLength(50)
                    .HasColumnName("designation");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");
                entity.Property(e => e.HireDate).HasColumnName("hire_date");
                entity.Property(e => e.InActive)
                    .HasDefaultValue(false)
                    .HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256)
                    .HasColumnName("password_hash");
                entity.Property(e => e.Username)
                    .HasMaxLength(100)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
