using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using PlanQuinquenal.Infrastructure.Repositories;
using Quartz;
using Quartz.Impl;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;

namespace PlanQuinquenal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(x =>
           x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<IRepositoryMantenedores, MantenedoresRepository>();
            services.AddTransient<IRepositoryLogin, LoginRepository>();
            services.AddTransient<IRepositoryPermisos, PermisosRepository>();
            services.AddTransient<IRepositoryPerfil, PerfilRepository>();
            services.AddTransient<IRepositoryUnidadNeg, UnidadNegRepository>();
            services.AddTransient<IRepositoryProyecto, ProyectoRepository>();
            services.AddTransient<HashService>();
            services.AddTransient<Constantes>();


            services.AddEndpointsApiExplorer();

            //Conexion
            services.AddDbContext<PlanQuinquenalContext>(options => 
              options.UseSqlServer(Configuration.GetConnectionString("PlanQuinquenal"))
          );
            //------------------------------------- JOB ----------------------------------------
            // Configuración de Quartz.NET
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            var job = JobBuilder.Create<JobInfoHIS>()
                .WithIdentity("MyJob", "MyJobGroup")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("MyTrigger", "MyTriggerGroup")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(15, 6)) // Se ejecutará todos los días a las 7 am
                .Build();

            scheduler.ScheduleJob(job, trigger).Wait();
            //-------------------------------------------------------------------------------------

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                    ClockSkew = TimeSpan.Zero
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Servicios Rest - DavisPeru", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddCors(opciones =>
            {
                var urlList = Configuration.GetSection("AllowedOrigin").GetChildren().Select(c => c.Value)
                    .ToArray();
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(urlList).AllowAnyMethod().AllowAnyHeader();
                });

            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
            });
        }
    }
}
