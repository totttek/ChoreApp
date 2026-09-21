namespace ChoreApp.Models;

public class UserPatchDto
{
	public string? Name { get; set; }
	public int? Xp { get; set; }
	public int? Streak { get; set; }
	public DateTime? LastChoreDayCompletedAt { get; set; }
	public int? HouseholdId { get; set; }
}
