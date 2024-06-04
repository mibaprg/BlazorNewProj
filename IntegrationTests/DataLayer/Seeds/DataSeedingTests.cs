using Havit.Data.Patterns.DataSeeds;
using MB.HBlazorApp.DataLayer.Seeds.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using MB.HBlazorApp.TestHelpers;

namespace MB.HBlazorApp.IntegrationTests.DataLayer.Seeds;

[TestClass]
public class DataSeedingTests : IntegrationTestBase
{
	protected override bool UseLocalDb => true;
	protected override bool DeleteDbData => true; // default, but to be sure :D

	[TestMethod]
	public async Task DataSeeds_CoreProfile()
	{
		// arrange
		var seedRunner = ServiceProvider.GetRequiredService<IDataSeedRunner>();

		// act
		await seedRunner.SeedDataAsync<CoreProfile>();

		// assert
		// no exception
	}
}
