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
public class FakeRoleDataSource : FakeDataSource<MB.HBlazorApp.Model.Security.Role>, MB.HBlazorApp.DataLayer.DataSources.Security.IRoleDataSource
{
	public FakeRoleDataSource(params MB.HBlazorApp.Model.Security.Role[] data)
		: this((IEnumerable<MB.HBlazorApp.Model.Security.Role>)data)
	{			
	}

	public FakeRoleDataSource(IEnumerable<MB.HBlazorApp.Model.Security.Role> data, ISoftDeleteManager softDeleteManager = null)
		: base(data, softDeleteManager)
	{
	}
}
