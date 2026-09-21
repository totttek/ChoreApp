namespace ChoreApp.Models;

public class ChorePatchDto
{
	public string? Name { get; set; }
	public string? Description { get; set; }
	public bool? IsComplete { get; set; }
	public DateTime? CompletedAt { get; set; }
	public bool? IsMandatory { get; set; }
	public int? XpReward { get; set; }
	public int? AssignedUserId { get; set; }
}
