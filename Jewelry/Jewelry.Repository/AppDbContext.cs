using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository
{
    /// <summary>
    /// Entity Framework Core DbContext for SQL Server - TrangSucBac Database
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Danh mục & Sản phẩm
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<ChatLieu> ChatLieus { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<BienThe> BienThes { get; set; }
        public DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }

        // Người dùng
        public DbSet<NguoiDung> NguoiDungs { get; set; }

        // Mã giảm giá
        public DbSet<MaGiamGia> MaGiamGias { get; set; }

        // Đơn hàng
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        // Giỏ hàng
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

        // Nhật ký
        public DbSet<AdminLog> AdminLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure LoaiSanPham entity
            modelBuilder.Entity<LoaiSanPham>(entity =>
            {

                entity.HasKey(e => e.id_loai_sp);
                entity.Property(e => e.id_loai_sp)
                     .ValueGeneratedOnAdd();

                entity.Property(e => e.ten_loai)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.mo_ta)
                    .HasMaxLength(500);    
            });

            // Configure ChatLieu entity
            modelBuilder.Entity<ChatLieu>(entity =>
            {
                entity.HasKey(e => e.id_chat_lieu);
                entity.Property(e => e.ten_chat_lieu).HasMaxLength(100).IsRequired();
                entity.Property(e => e.do_tinh_khiet).HasMaxLength(50);
                entity.Property(e => e.mo_ta).HasMaxLength(500);
            });

            // Configure SanPham entity
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.id_san_pham);
                entity.Property(e => e.ten_sp).HasMaxLength(200).IsRequired();
                entity.Property(e => e.gia).HasPrecision(18, 0);
                entity.Property(e => e.gia_khuyen_mai).HasPrecision(18, 0);
                entity.Property(e => e.trong_luong).HasPrecision(8, 2);
            });

            // Configure BienThe entity
            modelBuilder.Entity<BienThe>(entity =>
            {
                entity.HasKey(e => e.id_bien_the);
                entity.Property(e => e.kich_co).HasMaxLength(20);
                entity.Property(e => e.mau_sac).HasMaxLength(50);
                entity.Property(e => e.gia_them).HasPrecision(18, 0);
            });

            // Configure HinhAnhSanPham entity
            modelBuilder.Entity<HinhAnhSanPham>(entity =>
            {
                entity.HasKey(e => e.id_hinh_anh);
                entity.Property(e => e.duong_dan).HasMaxLength(500).IsRequired();
            });

            // Configure NguoiDung entity
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasKey(e => e.id_nguoi_dung);
                entity.Property(e => e.ho_ten).HasMaxLength(150).IsRequired();
                entity.Property(e => e.email).HasMaxLength(150).IsRequired();
                entity.HasIndex(e => e.email).IsUnique();
                entity.Property(e => e.ten_dang_nhap).HasMaxLength(150).IsRequired();
                entity.Property(e => e.mat_khau).HasMaxLength(255).IsRequired();
                entity.Property(e => e.so_dien_thoai).HasMaxLength(15);
                entity.Property(e => e.dia_chi).HasMaxLength(300);
                entity.Property(e => e.vai_tro).HasMaxLength(20);
            });

            // Configure MaGiamGia entity
            modelBuilder.Entity<MaGiamGia>(entity =>
            {
                entity.HasKey(e => e.id_ma_giam_gia);
                entity.Property(e => e.ma_code).HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.ma_code).IsUnique();
                entity.Property(e => e.mo_ta).HasMaxLength(300);
                entity.Property(e => e.loai_giam).HasMaxLength(10).IsRequired();
                entity.Property(e => e.gia_tri).HasPrecision(10, 2);
                entity.Property(e => e.giam_toi_da).HasPrecision(18, 0);
                entity.Property(e => e.don_hang_toi_thieu).HasPrecision(18, 0);
            });

            // Configure HoaDon entity
            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.id_hoa_don);
                entity.Property(e => e.ho_ten).HasMaxLength(150).IsRequired();
                entity.Property(e => e.email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.so_dien_thoai).HasMaxLength(15).IsRequired();
                entity.Property(e => e.tinh_thanh_pho).HasMaxLength(100).IsRequired();
                entity.Property(e => e.phuong_xa).HasMaxLength(100).IsRequired();
                entity.Property(e => e.dia_chi_cu_the).HasMaxLength(200).IsRequired();
                entity.Property(e => e.phuong_thuc_tt).HasMaxLength(50);
                entity.Property(e => e.tien_hang).HasPrecision(18, 0);
                entity.Property(e => e.tien_giam).HasPrecision(18, 0);
                entity.Property(e => e.phi_van_chuyen).HasPrecision(18, 0);
                entity.Property(e => e.tong_tien).HasPrecision(18, 0);
                entity.Property(e => e.trang_thai).HasMaxLength(30);
                entity.Property(e => e.ghi_chu).HasMaxLength(500);
            });

            // Configure ChiTietHoaDon entity
            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                entity.HasKey(e => e.id_chi_tiet);
                entity.Property(e => e.ten_sp_luu).HasMaxLength(200).IsRequired();
                entity.Property(e => e.chat_lieu_luu).HasMaxLength(100);
                entity.Property(e => e.mau_sac_luu).HasMaxLength(50);
                entity.Property(e => e.kich_co_luu).HasMaxLength(20);
                entity.Property(e => e.don_gia).HasPrecision(18, 0);
            });

            // Configure GioHang entity
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasKey(e => e.id_gio_hang);
                entity.Property(e => e.ma_phien).HasMaxLength(100).IsRequired();
            });

            // Configure ChiTietGioHang entity
            modelBuilder.Entity<ChiTietGioHang>(entity =>
            {
                entity.HasKey(e => e.id_chi_tiet_gh);
                entity.Property(e => e.don_gia).HasPrecision(18, 0);
            });

            // Configure AdminLog entity
            modelBuilder.Entity<AdminLog>(entity =>
            {
                entity.HasKey(e => e.id_log);
                entity.Property(e => e.ten_bang).HasMaxLength(100).IsRequired();
                entity.Property(e => e.hanh_dong).HasMaxLength(20).IsRequired();   
                entity.Property(e => e.noi_dung).HasMaxLength(int.MaxValue);
            });
        }
    }
}