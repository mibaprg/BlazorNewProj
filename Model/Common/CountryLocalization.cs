using MB.HBlazorApp.Model.Localizations;
using System.ComponentModel.DataAnnotations;

namespace MB.HBlazorApp.Model.Common;

public class CountryLocalization : ILocalization<Country>
{
	public int Id { get; set; }

	public Country Parent { get; set; }
	public int ParentId { get; set; }

	public Language Language { get; set; }
	public int LanguageId { get; set; }

	[Required]
	[MaxLength(50)]
	public string Name { get; set; }
}
