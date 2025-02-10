using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nasty.Core.SuperExtension;
using NastyService;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddControllers().AddControllersAsServices();

builder.Host.AddNastyHost(args, AutofacModule.GetModules(), null);
builder.Services.AddNasty();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseNasty();

app.MapRazorPages();

app.MapControllers();

app.Run();
