namespace ChoreApp.Models;

public class Chore
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool IsComplete { get; set; }
	public DateTime? CompletedAt { get; set; }
	public bool IsMandatory { get; set; }
	public int XpReward { get; set; }
	public int HouseholdId { get; set; }
	public int? AssignedUserId { get; set; }
}
