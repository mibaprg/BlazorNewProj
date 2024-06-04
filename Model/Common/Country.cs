﻿using Havit.Data.EntityFrameworkCore.Attributes;
using MB.HBlazorApp.Model.Localizations;
using System.ComponentModel.DataAnnotations;

namespace MB.HBlazorApp.Model.Common;

[Cache]
public class Country : ILocalized<CountryLocalization>
{
	public int Id { get; set; }

	/// <summary>
	/// 2-chars ISO
	/// </summary>
	[Required]
	[MaxLength(2)]
	public string IsoCode { get; set; }

	/// <summary>
	/// 3-chars ISO
	/// </summary>
	[Required]
	[MaxLength(3)]
	public string IsoCode3 { get; set; }

	/// <summary>
	/// CZ: Předčíslí telefonního čísla
	/// </summary>
	[MaxLength(6)]
	public string PhoneCountryCode { get; set; }

	public int UiOrder { get; set; }

	public List<CountryLocalization> Localizations { get; } = new List<CountryLocalization>();
}
