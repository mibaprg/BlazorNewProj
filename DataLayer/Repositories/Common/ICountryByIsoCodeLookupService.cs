
using MB.HBlazorApp.Model.Common;

namespace MB.HBlazorApp.DataLayer.Repositories.Common;

public interface ICountryByIsoCodeLookupService
{
	Country GetCountryByIsoCode(string isoCode);
}
