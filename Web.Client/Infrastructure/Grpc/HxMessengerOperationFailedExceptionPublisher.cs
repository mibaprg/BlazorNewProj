using Havit.Blazor.Grpc.Client.ServerExceptions;
using MB.HBlazorApp.Resources;
using Microsoft.Extensions.Localization;

namespace MB.HBlazorApp.Web.Client.Infrastructure.Grpc;

public class HxMessengerOperationFailedExceptionGrpcClientListener : IOperationFailedExceptionGrpcClientListener
{
	private readonly IHxMessengerService _messenger;
	private readonly IStringLocalizer<Global> _localizer;

	public HxMessengerOperationFailedExceptionGrpcClientListener(IHxMessengerService messenger, IStringLocalizer<Global> localizer)
	{
		_messenger = messenger;
		_localizer = localizer;
	}

	public Task ProcessAsync(string errorMessage)
	{
		_messenger.AddError(_localizer["OperationFailedExceptionMessengerTitle"], errorMessage);

		return Task.CompletedTask;
	}
}
