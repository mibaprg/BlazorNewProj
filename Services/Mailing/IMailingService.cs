using MimeKit;

namespace MB.HBlazorApp.Services.Mailing;

public interface IMailingService
{
	Task VerifyHealthAsync(CancellationToken cancellationToken = default);

	Task SendAsync(MimeMessage mailMessage, CancellationToken cancellationToken = default);
}
