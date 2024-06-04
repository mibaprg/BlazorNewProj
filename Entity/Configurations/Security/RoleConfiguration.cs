using MB.HBlazorApp.Model.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MB.HBlazorApp.Entity.Configurations.Security;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.Property(a => a.Id).ValueGeneratedNever();
	}
}
