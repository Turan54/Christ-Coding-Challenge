using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
namespace Christ_Coding_Challenge.Models

{
    public class Article
    {
        public string Id { get; set; } // Eindeutige ID des Artikels
        public string ArticleId { get; set; } // Artikelnummer
        public List<ArticleAttribute> Attributes { get; set; } = new List<ArticleAttribute>(); // Liste der Attribute
    }

    public class ArticleAttribute
    {
        public int ArticleAttributeId { get; set; }  // Primary
        public string Key { get; set; } // Schlüssel für das Attribut
        public string Source { get; set; } // Quelle des Attributs
        public string Value { get; set; } // Wert des Attributs
        public string Label { get; set; } // Bezeichnung des Attributs
        public string? Language { get; set; } // Sprache des Attributs, kann null sein
        public string ArticleId { get; set; } // Fremdschlüssel zur Article
        public Article Article { get; set; } // Navigationseigenschaft zurück zu Article
    }

    public class ArticleDbContext : DbContext
    {
        public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleAttribute> ArticleAttributes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
              "Data Source=.\\SQLEXPRESS;Initial Catalog=MyApplicationDB;Integrated Security=True;Trust Server Certificate=True",
                options => options.CommandTimeout(180)); // Setzt das Command Timeout auf 180 Sekunden
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definiere den Primärschlüssel für ArticleAttribute
            modelBuilder.Entity<ArticleAttribute>().HasKey(aa => aa.ArticleAttributeId);

            // Konfiguriere die 1:n-Beziehung zwischen Article und ArticleAttribute
            modelBuilder.Entity<Article>()
                .HasMany(a => a.Attributes)
                .WithOne(aa => aa.Article)
                .HasForeignKey(aa => aa.ArticleId); // Stelle sicher, dass der Fremdschlüssel auf ArticleId gesetzt ist
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:8080") // Erlaubt Zugriffe vom Frontend
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler =ReferenceHandler.Preserve;
            });
        }

    }
}
