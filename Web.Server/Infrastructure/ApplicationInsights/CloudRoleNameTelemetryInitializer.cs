﻿using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace MB.HBlazorApp.Web.Server.Infrastructure.ApplicationInsights;

public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
{
	public void Initialize(ITelemetry telemetry)
	{
		telemetry.Context.Cloud.RoleName = "Web.Server";
	}
}
