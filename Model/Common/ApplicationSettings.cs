using System.ComponentModel.DataAnnotations.Schema;

namespace MB.HBlazorApp.Model.Common;

public class ApplicationSettings
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	public enum Entry
	{
		Current = -1
	}
}
