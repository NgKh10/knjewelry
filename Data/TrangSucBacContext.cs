using Microsoft.EntityFrameworkCore;
using knjewelry.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace knjewelry.Data
{
    public class TrangSucBacContext : DbContext
    {
        public TrangSucBacContext(DbContextOptions<TrangSucBacContext> options)
            : base(options)
        {
        }

        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<ChatLieu> ChatLieus { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<BienThe> BienThes { get; set; }
        public DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<MaGiamGia> MaGiamGias { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LoaiSanPham self-referencing
            modelBuilder.Entity<LoaiSanPham>()
                .HasOne(l => l.LoaiCha)
                .WithMany(l => l.LoaiCon)
                .HasForeignKey(l => l.id_loai_cha)
                .OnDelete(DeleteBehavior.Restrict);

            // HoaDon - computed column
            modelBuilder.Entity<HoaDon>()
                .Property(h => h.ma_hoa_don)
                .HasComputedColumnSql("('HD' + RIGHT('000000' + CAST(id_hoa_don AS VARCHAR), 6))");

            // Unique constraint for GioHang
            modelBuilder.Entity<GioHang>()
                .HasIndex(g => new { g.ma_phien, g.id_nguoi_dung })
                .IsUnique();

            // Indexes
            modelBuilder.Entity<SanPham>().HasIndex(p => p.id_loai_sp);
            modelBuilder.Entity<SanPham>().HasIndex(p => p.trang_thai);
            modelBuilder.Entity<HoaDon>().HasIndex(h => h.trang_thai);
            modelBuilder.Entity<GioHang>().HasIndex(g => g.ma_phien);
            modelBuilder.Entity<GioHang>().HasIndex(g => g.id_nguoi_dung);
            modelBuilder.Entity<ChiTietGioHang>().HasIndex(c => c.id_gio_hang);
        }
    }
}