using knjewelry.Data;
using knjewelry.Repository;
using Jewelry.Common.Auth;
using Jewelry.Common.Helpers;
using Jewelry.Repository;
using Jewelry.Repository.EFCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// MVC + API Controllers (gộp cả 2 phần trong 1 app)
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Tránh vòng lặp tuần hoàn khi serialize (cần cho API)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();

// Session (dùng cho phần giao diện User - MVC)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

// CORS (cần cho React admin gọi API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// DbContext - phần User MVC (TrangSucBacContext)
builder.Services.AddDbContext<TrangSucBacContext>(options =>
    options.UseSqlServer(connectionString));

// DbContext - phần API Jewelry (AppDbContext)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories - phần User MVC
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<knjewelry.Repository.ISanPhamRepository, knjewelry.Repository.SanPhamRepository>();
builder.Services.AddScoped<knjewelry.Repository.IDonHangRepository, knjewelry.Repository.DonHangRepository>();
builder.Services.AddScoped<knjewelry.Repository.ITaiKhoanRepository, knjewelry.Repository.TaiKhoanRepository>();
builder.Services.AddScoped<knjewelry.Repository.IGioHangRepository, knjewelry.Repository.GioHangRepository>();

// Services - phần User MVC
builder.Services.AddScoped<knjewelry.Services.ISanPhamService, knjewelry.Services.SanPhamService>();
builder.Services.AddScoped<knjewelry.Services.IGioHangService, knjewelry.Services.GioHangService>();
builder.Services.AddScoped<knjewelry.Services.IDonHangService, knjewelry.Services.DonHangService>();
builder.Services.AddScoped<knjewelry.Services.ITaiKhoanService, knjewelry.Services.TaiKhoanService>();

// Repositories - phần API Jewelry
// Dùng fully-qualified name để tránh xung đột với knjewelry.Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<Jewelry.Repository.EFCore.ISanPhamRepository, Jewelry.Repository.EFCore.SanPhamRepository>();
builder.Services.AddScoped<ILoaiSanPhamRepository, LoaiSanPhamRepository>();
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddScoped<IChiTietHoaDonRepository, ChiTietHoaDonRepository>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IChatLieuRepository, ChatLieuRepository>();
builder.Services.AddScoped<IBienTheRepository, BienTheRepository>();
builder.Services.AddScoped<IHinhAnhSanPhamRepository, HinhAnhSanPhamRepository>();
builder.Services.AddScoped<IMaGiamGiaRepository, MaGiamGiaRepository>();

// =====================================================
// Services - phần API Jewelry
// (dùng fully-qualified name để tránh xung đột với MVC services)
// =====================================================
builder.Services.AddScoped<Jewelry.Services.ILoaiSanPhamService, Jewelry.Services.LoaiSanPhamService>();
builder.Services.AddScoped<Jewelry.Services.ISanPhamService, Jewelry.Services.SanPhamService>();
builder.Services.AddScoped<Jewelry.Services.IHoaDonService, Jewelry.Services.HoaDonService>();
builder.Services.AddScoped<Jewelry.Services.INguoiDungService, Jewelry.Services.NguoiDungService>();
builder.Services.AddScoped<Jewelry.Services.IChatLieuService, Jewelry.Services.ChatLieuService>();
builder.Services.AddScoped<Jewelry.Services.IBienTheService, Jewelry.Services.BienTheService>();
builder.Services.AddScoped<Jewelry.Services.IHinhAnhSanPhamService, Jewelry.Services.HinhAnhSanPhamService>();
builder.Services.AddScoped<Jewelry.Services.IMaGiamGiaService, Jewelry.Services.MaGiamGiaService>();
builder.Services.AddScoped<Jewelry.Services.Authen.INguoiDungService, Jewelry.Services.Authen.NguoiDungService>();
builder.Services.AddHostedService<Jewelry.Services.DiscountExpiryService>();

// Helpers (dùng cho API - tạo JWT token, xử lý file)
builder.Services.AddScoped<TokenHelper>();
builder.Services.AddScoped<FileHelper>();

// JWT Authentication (dùng cho React admin)
var jwtKey = builder.Configuration["Jwt:SecretKey"]
    ?? "YourSecretKeyForAuthenticationShouldBeLongEnough";
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    })
    // Cookie Auth cho phần giao diện User (MVC) — bền vững hơn Session
    .AddCookie("MvcCookie", options =>
    {
        options.LoginPath  = "/TaiKhoan/DangNhap";
        options.LogoutPath = "/TaiKhoan/DangXuat";
        options.Cookie.Name     = "KN.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan  = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Ngăn browser cache index.html của admin SPA
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin"))
    {
        var ext = Path.GetExtension(context.Request.Path.Value);
        if (string.IsNullOrEmpty(ext) || ext.Equals(".html", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
        }
    }
    await next();
});

// React Admin SPA: mọi request /admin/* không phải file tĩnh → trả về index.html
app.MapFallbackToFile("/admin/{**path}", "admin/index.html");

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles(); //cho phép truy cập wwwroot
app.UseRouting();
app.UseSession(); // cho phép dùng session
app.UseAuthentication(); 
app.UseAuthorization(); // phân quyền 

// MVC routes (phần giao diện User)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
