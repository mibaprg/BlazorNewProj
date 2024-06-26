﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Attributes;

namespace MB.HBlazorApp.DataLayer.DataSources.Localizations.Fakes;

[Fake]
[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public class FakeLanguageDataSource : FakeDataSource<MB.HBlazorApp.Model.Localizations.Language>, MB.HBlazorApp.DataLayer.DataSources.Localizations.ILanguageDataSource
{
	public FakeLanguageDataSource(params MB.HBlazorApp.Model.Localizations.Language[] data)
		: this((IEnumerable<MB.HBlazorApp.Model.Localizations.Language>)data)
	{			
	}

	public FakeLanguageDataSource(IEnumerable<MB.HBlazorApp.Model.Localizations.Language> data, ISoftDeleteManager softDeleteManager = null)
		: base(data, softDeleteManager)
	{
	}
}
