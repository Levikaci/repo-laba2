using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace ChemistryApp2;

public partial class App : Application
{
    public static ChemistryDbContext Db { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var conn = "Host=localhost;Database=1laba_ivan;Username=postgres;Password=sa";
        var options = new DbContextOptionsBuilder<ChemistryDbContext>()
            .UseNpgsql(conn)
            .Options;

        Db = new ChemistryDbContext(options);
        Db.Database.EnsureCreated();

        // Сид тем (если пусто)
        if (!Db.Topics.Any())
        {
            Db.Topics.AddRange(new[]
            {
                new Topic { Title = "Тела и вещества", Content = "Вещества состоят из атомов. Тела имеют форму и объём.", SortOrder = 1 },
                new Topic { Title = "Физические и химические процессы", Content = "Физические: плавление, кипение. Химические: горение, ржавление.", SortOrder = 2 },
                new Topic { Title = "Химические элементы", Content = "118 элементов в таблице Менделеева.", SortOrder = 3 }
            });
            Db.SaveChanges();
        }

        if (!Db.ChemicalElements.Any())
        {
            SeedElements();
        }

        new MainWindow().Show();
    }

    // Метод для сидинга всех элементов (вынесите в отдельный файл при росте)
    private void SeedElements()
    {
        var seeds = new[]
        {
            new ChemicalElement { AtomicNumber = 1, Symbol = "H", NameRu = "Водород", AtomicMass = 1.008m, GroupNumber = 1, Period = 1, Category = "Неметалл", Description = "Самый лёгкий элемент" },
            new ChemicalElement { AtomicNumber = 2, Symbol = "He", NameRu = "Гелий", AtomicMass = 4.0026m, GroupNumber = 18, Period = 1, Category = "Инертный газ", Description = "Инертный газ" },
        };
        Db.ChemicalElements.AddRange(seeds);
        Db.SaveChanges();
    }
}