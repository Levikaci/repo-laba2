using Microsoft.EntityFrameworkCore;

namespace ChemistryApp2;

public class Topic
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int SortOrder { get; set; }
}

public class ChemicalElement
{
    public int Id { get; set; }
    public int AtomicNumber { get; set; }
    public string Symbol { get; set; } = "";
    public string NameRu { get; set; } = "";
    public decimal? AtomicMass { get; set; }
    public string? Description { get; set; }
    public int GroupNumber { get; set; }
    public int Period { get; set; }
    public string Category { get; set; } = "Неметалл";
    public string Display => $"{AtomicNumber}. {NameRu} ({Symbol})";
}

public class ChemistryDbContext : DbContext
{
    public ChemistryDbContext(DbContextOptions<ChemistryDbContext> options) : base(options) { }
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<ChemicalElement> ChemicalElements { get; set; } = null!;
}