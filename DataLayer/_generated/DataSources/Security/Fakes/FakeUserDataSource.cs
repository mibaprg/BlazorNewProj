﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Attributes;

namespace MB.HBlazorApp.DataLayer.DataSources.Security.Fakes;

[Fake]
[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public class FakeUserDataSource : FakeDataSource<MB.HBlazorApp.Model.Security.User>, MB.HBlazorApp.DataLayer.DataSources.Security.IUserDataSource
{
	public FakeUserDataSource(params MB.HBlazorApp.Model.Security.User[] data)
		: this((IEnumerable<MB.HBlazorApp.Model.Security.User>)data)
	{			
	}

	public FakeUserDataSource(IEnumerable<MB.HBlazorApp.Model.Security.User> data, ISoftDeleteManager softDeleteManager = null)
		: base(data, softDeleteManager)
	{
	}
}
