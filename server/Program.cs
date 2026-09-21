using Microsoft.EntityFrameworkCore;
using ChoreApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ChoreAppDbContext>(opt => opt.UseSqlite("Data Source=todo.db"));
builder.Services.AddOpenApi();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ChoreAppDbContext>();
	db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

var chores = app.MapGroup("/chores");

chores.MapGet("/", async (ChoreAppDbContext db) =>
	await db.Chores.ToListAsync());

chores.MapGet("/{id}", async (int id, ChoreAppDbContext db) =>
	await db.Chores.FindAsync(id) is Chore chore
		? Results.Ok(chore)
		: Results.NotFound());

chores.MapPost("/", async (Chore chore, ChoreAppDbContext db) =>
{
	db.Chores.Add(chore);
	await db.SaveChangesAsync();

	return Results.Created($"/chores/{chore.Id}", chore);
});

chores.MapPatch("/{id}", async (int id, ChorePatchDto input, ChoreAppDbContext db) =>
{
	var chore = await db.Chores.FindAsync(id);
	if (chore is null) return Results.NotFound();

	if (input.Name is not null) chore.Name = input.Name;
	if (input.Description is not null) chore.Description = input.Description;
	if (input.IsComplete is not null) chore.IsComplete = input.IsComplete.Value;
	if (input.CompletedAt is not null) chore.CompletedAt = input.CompletedAt;
	if (input.IsMandatory is not null) chore.IsMandatory = input.IsMandatory.Value;
	if (input.XpReward is not null) chore.XpReward = input.XpReward.Value;
	if (input.AssignedUserId is not null) chore.AssignedUserId = input.AssignedUserId;

	await db.SaveChangesAsync();
	return Results.NoContent();
});

chores.MapDelete("/{id}", async (int id, ChoreAppDbContext db) =>
{
	if (await db.Chores.FindAsync(id) is not Chore chore) return Results.NotFound();
	db.Chores.Remove(chore);
	await db.SaveChangesAsync();
	return Results.Ok(chore);
});

var households = app.MapGroup("/households");
households.MapGet("/", async (ChoreAppDbContext db) => await db.Households.ToListAsync());
households.MapPost("/", async (Household household, ChoreAppDbContext db) =>
{
	db.Households.Add(household);
	await db.SaveChangesAsync();
	return Results.Created($"/households/{household.Id}", household);
});

households.MapPatch("/{id}", async (int id, HouseholdPatchDto input, ChoreAppDbContext db) =>
{
	var household = await db.Households.FindAsync(id);
	if (household is null) return Results.NotFound();
	if (input.Name is not null) household.Name = input.Name;
	await db.SaveChangesAsync();
	return Results.NoContent();
});

var users = app.MapGroup("/users");
users.MapGet("/", async (ChoreAppDbContext db) => await db.Users.ToListAsync());
users.MapPost("/", async (User user, ChoreAppDbContext db) =>
{
	db.Users.Add(user);
	await db.SaveChangesAsync();
	return Results.Created($"/users/{user.Id}", user);
});

users.MapPatch("/{id}", async (int id, UserPatchDto input, ChoreAppDbContext db) =>
{
	var user = await db.Users.FindAsync(id);
	if (user is null) return Results.NotFound();

	if (input.Name is not null) user.Name = input.Name;
	if (input.Xp is not null) user.Xp = input.Xp.Value;
	if (input.Streak is not null) user.Streak = input.Streak.Value;
	if (input.LastChoreDayCompletedAt is not null)
		user.LastChoreDayCompletedAt = input.LastChoreDayCompletedAt;
	if (input.HouseholdId is not null) user.HouseholdId = input.HouseholdId.Value;

	await db.SaveChangesAsync();
	return Results.NoContent();
});

app.Run();
