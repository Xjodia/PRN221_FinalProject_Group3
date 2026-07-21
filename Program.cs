using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Services.Implement;
using PRN221_FinalProject_Group3.Services.Email;
using PRN221_FinalProject_Group3.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "StoryNest.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<EmailVerificationDao>();
builder.Services.AddScoped<NovelDao>();
builder.Services.AddScoped<ChapterDao>();
builder.Services.AddScoped<DashboardDao>();
builder.Services.AddScoped<ProfileDao>();
builder.Services.AddScoped<SystemDao>();
builder.Services.AddScoped<LibraryDao>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.Configure<MailOptions>(options =>
{
    builder.Configuration.GetSection("Mail").Bind(options);
    options.Host = builder.Configuration["MAIL_HOST"] ?? options.Host;
    if (int.TryParse(builder.Configuration["MAIL_PORT"], out var mailPort))
    {
        options.Port = mailPort;
    }
    options.Username = builder.Configuration["MAIL_USERNAME"] ?? options.Username;
    options.Password = builder.Configuration["MAIL_PASSWORD"] ?? options.Password;
    options.FromAddress = builder.Configuration["MAIL_FROM"] ?? options.FromAddress;
    options.FromName = builder.Configuration["MAIL_FROM_NAME"] ?? options.FromName;
});
builder.Services.AddScoped<INovelService, NovelService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
