using AudioService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AudioService.Data;

public class BooksContext : DbContext
{
	public BooksContext(DbContextOptions options) : base(options)
	{
	}

	public DbSet<BookEntity> Books { get; set; }
	public DbSet<ReviewEntity> Reviews { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<BookEntity>()
			.HasMany(b => b.Reviews)
			.WithOne(r => r.BookEntity)
			.OnDelete(DeleteBehavior.Cascade);
		
		//Separate authors with commas to avoid creating new table
		modelBuilder.Entity<BookEntity>()
			.Property(e => e.Authors)
			.HasConversion(
				v => string.Join(',', v),
				v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
			.Metadata.SetValueComparer(new ValueComparer<string[]>(
				(c1, c2) => c1.SequenceEqual(c2),
				c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c.ToArray()));;
	}
}