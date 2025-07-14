using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointOfSale.Database.AppDbContextModels
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleDetail> SaleDetails { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Role> Roles { get; set; }  // ✅ Added for Role table support

        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuPermission> MenuPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductCode).HasName("PK__product__AE1A8CC57CBF433D");
                entity.ToTable("product");

                entity.Property(e => e.ProductCode).HasMaxLength(50).HasColumnName("product_code");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.InActive).HasDefaultValue(false).HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt).HasColumnType("datetime").HasColumnName("modified_at");
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
                entity.Property(e => e.ProductName).HasMaxLength(100).HasColumnName("product_name");
                entity.Property(e => e.StockQuantity).HasDefaultValue(0).HasColumnName("stock_quantity");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.SaleId).HasName("PK__sale__E1EB00B277A95327");
                entity.ToTable("sale");

                entity.HasIndex(e => e.VoucherCode, "UQ__sale__21731069B41EBD53").IsUnique();

                entity.Property(e => e.SaleId).HasColumnName("sale_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.InActive).HasDefaultValue(false).HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt).HasColumnType("datetime").HasColumnName("modified_at");
                entity.Property(e => e.SaleDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("sale_date");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)").HasColumnName("total_amount");
                entity.Property(e => e.VoucherCode).HasMaxLength(50).HasColumnName("voucher_code");
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.HasKey(e => e.SaleDetailId).HasName("PK__sale_det__4D671D835D70052E");
                entity.ToTable("sale_details");

                entity.Property(e => e.SaleDetailId).HasColumnName("sale_detail_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.InActive).HasDefaultValue(false).HasColumnName("in_active");
                entity.Property(e => e.ModifiedAt).HasColumnType("datetime").HasColumnName("modified_at");
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
                entity.Property(e => e.ProductCode).HasMaxLength(50).HasColumnName("product_code");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.VoucherCode).HasMaxLength(50).HasColumnName("voucher_code");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__staff__1963DD9CC8272A77");
                entity.ToTable("staff");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");
                entity.Property(e => e.Username).HasMaxLength(100).HasColumnName("username");
                entity.Property(e => e.PasswordHash).HasMaxLength(256).HasColumnName("password_hash");
                entity.Property(e => e.FullName).HasMaxLength(100).HasColumnName("full_name");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
                entity.Property(e => e.HireDate).HasColumnName("hire_date");
                entity.Property(e => e.Designation).HasMaxLength(50).HasColumnName("designation");
                entity.Property(e => e.InActive).HasDefaultValue(false).HasColumnName("in_active");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.ModifiedAt).HasColumnType("datetime").HasColumnName("modified_at");
                entity.Property<int>("RoleId").HasColumnName("role_id"); // ✅ Add RoleId field manually (no FK)
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasColumnName("RoleId");
                entity.Property(e => e.RoleName).HasColumnName("RoleName").HasMaxLength(50);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.HasKey(e => e.MenuId);

                entity.Property(e => e.MenuId).HasColumnName("MenuId");
                entity.Property(e => e.Name)
                      .HasColumnName("Name")
                      .HasMaxLength(100)
                      .IsRequired();
                entity.Property(e => e.Endpoint)
                      .HasColumnName("Endpoint")
                      .HasMaxLength(200)
                      .IsRequired();
            });

            modelBuilder.Entity<MenuPermission>(entity =>
            {
                entity.ToTable("MenuPermission");

                entity.HasKey(e => e.MenuPermissionId);

                entity.Property(e => e.MenuPermissionId).HasColumnName("MenuPermissionId");
                entity.Property(e => e.RoleId).HasColumnName("RoleId");
                entity.Property(e => e.MenuId).HasColumnName("MenuId");
                entity.Property(e => e.CanAccess).HasColumnName("CanAccess").HasDefaultValue(false);

                // Foreign key relationships
                entity.HasOne(d => d.Role)
                      .WithMany()
                      .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.Menu)
                      .WithMany(p => p.MenuPermissions)
                      .HasForeignKey(d => d.MenuId);
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
