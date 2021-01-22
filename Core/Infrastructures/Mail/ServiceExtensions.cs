using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TGS.Core.Infrastructures.Mail
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMail(this IServiceCollection services, IConfiguration configuration,
            string configSectionName = "EmailSetting")
        {
            services.Configure<EmailConfiguration>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailConfiguration>>().Value);

            services.AddTransient<IEmailManager, EmailManager>();
            services.AddTransient(sp =>
            {
                var settings = sp.GetRequiredService<EmailConfiguration>();

                return new SmtpClient
                {
                    Host = settings.SmtpServer,
                    Port = settings.Port,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        settings.FromMail,
                        settings.Password
                    ),
                    EnableSsl = true
                };
            });

            return services;
        }
    }
}