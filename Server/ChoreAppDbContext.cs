using ChoreApp.Models;
using Microsoft.EntityFrameworkCore;

class ChoreAppDbContext : DbContext
{
	public ChoreAppDbContext(DbContextOptions<ChoreAppDbContext> options)
		: base(options) { }

	public DbSet<Chore> Chores => Set<Chore>();
	public DbSet<Household> Households => Set<Household>();
	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Household>()
			.HasMany<User>()
			.WithOne()
			.HasForeignKey(user => user.HouseholdId)
			.OnDelete(DeleteBehavior.SetNull);

		modelBuilder.Entity<Household>()
			.HasMany<Chore>()
			.WithOne()
			.HasForeignKey(chore => chore.HouseholdId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Chore>()
			.HasOne<User>()
			.WithMany()
			.HasForeignKey(chore => chore.AssignedUserId)
			.OnDelete(DeleteBehavior.SetNull);
	}
}
