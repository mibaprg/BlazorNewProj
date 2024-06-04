﻿using MB.HBlazorApp.Services.Mailing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MB.HBlazorApp.Web.Server.Infrastructure.ConfigurationExtensions;

public static class MailingConfig
{
	public static void AddCustomizedMailing(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<MailingOptions>(configuration.GetSection("AppSettings:MailingOptions"));
	}
}
