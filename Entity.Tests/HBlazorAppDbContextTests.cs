using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MB.HBlazorApp.Entity.Tests;

[TestClass]
public class HBlazorAppDbContextTests
{
	[TestMethod]
	public void HBlazorAppDbContext_CheckModelConventions()
	{
		// Arrange
		DbContextOptions<HBlazorAppDbContext> options = new DbContextOptionsBuilder<HBlazorAppDbContext>()
			.UseInMemoryDatabase(nameof(HBlazorAppDbContext))
			.Options;
		HBlazorAppDbContext dbContext = new HBlazorAppDbContext(options);

		// Act
		Havit.Data.EntityFrameworkCore.ModelValidation.ModelValidator modelValidator = new Havit.Data.EntityFrameworkCore.ModelValidation.ModelValidator();
		string errors = modelValidator.Validate(dbContext);

		// Assert
		if (!String.IsNullOrEmpty(errors))
		{
			Assert.Fail(errors);
		}
	}
}
