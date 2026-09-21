namespace ChoreApp.Models;

public class User
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public int Xp { get; set; }
	public int Streak { get; set; }
	public DateTime? LastChoreDayCompletedAt { get; set; }
	public int HouseholdId { get; set; }
}
