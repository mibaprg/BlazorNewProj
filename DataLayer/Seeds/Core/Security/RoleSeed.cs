﻿using Havit.Data.Patterns.DataSeeds;
using MB.HBlazorApp.Model.Security;
using MB.HBlazorApp.Primitives.Security;

namespace MB.HBlazorApp.DataLayer.Seeds.Core.Security;

public class RoleSeed : DataSeed<CoreProfile>
{
	public override void SeedData()
	{
		var roles = Enum.GetValues<RoleEntry>().Select(entry => new Role { Id = (int)entry, Name = entry.ToString() }).ToArray();

		Seed(For(roles).PairBy(r => r.Id));
	}
}
