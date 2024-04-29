using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
namespace Christ_Coding_Challenge.Models

{
    public class Article
    {
        public string Id { get; set; } 
        public string ArticleId { get; set; } 
        public List<ArticleAttribute> Attributes { get; set; } = new List<ArticleAttribute>(); 
    }

    public class ArticleAttribute
    {
        public int ArticleAttributeId { get; set; }  
        public string Key { get; set; } 
        public string Source { get; set; } 
        public string Value { get; set; } 
        public string Label { get; set; } 
        public string? Language { get; set; } 
        public string ArticleId { get; set; } 
        public Article Article { get; set; }
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
                options => options.CommandTimeout(180)); 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<ArticleAttribute>().HasKey(aa => aa.ArticleAttributeId);

            
            modelBuilder.Entity<Article>()
                .HasMany(a => a.Attributes)
                .WithOne(aa => aa.Article)
                .HasForeignKey(aa => aa.ArticleId); 
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:8080") 
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
