using MB.HBlazorApp.Contracts.Infrastructure;
using MB.HBlazorApp.Resources;
using Microsoft.AspNetCore.Components;

namespace MB.HBlazorApp.Web.Client;

public partial class Routes
{
	[Inject] protected IFluentValidationDefaultMessagesLocalizer ValidationMessagesLocalizer { get; set; }

	protected override void OnInitialized()
	{
		// we cannot use IStringLocalizer in application startup class (locks the culture to the invariant one)
		FluentValidationLocalizationHelper.RegisterDefaultValidationMessages(this.ValidationMessagesLocalizer);
	}
}