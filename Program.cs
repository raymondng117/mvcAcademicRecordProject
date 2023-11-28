using LabAssignment6.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string? dbConnstr = builder.Configuration.GetConnectionString("StudentRecord");
if (dbConnstr != null)
{
    builder.Services.AddDbContext<StudentrecordContext>(
        options => options.UseMySQL(dbConnstr));
}
else
{
    throw new Exception("no connection string obtained");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
