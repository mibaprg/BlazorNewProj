using Havit.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MB.HBlazorApp.Entity;

public class HBlazorAppDbContext : Havit.Data.EntityFrameworkCore.DbContext
{
	/// <summary>
	/// Constructor for unit tests.
	/// </summary>
	internal HBlazorAppDbContext()
	{
		// NOOP
	}

	public HBlazorAppDbContext(DbContextOptions options) : base(options)
	{
		// NOOP
	}

	/// <inheritdoc />
	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		// modelBuilder.HasSequence<int>("XySequence");

		modelBuilder.RegisterModelFromAssembly(typeof(MB.HBlazorApp.Model.Localizations.Language).Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
	}
}
