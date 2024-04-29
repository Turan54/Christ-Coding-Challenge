using Christ_Coding_Challenge.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDb")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder => builder
        .WithOrigins("http://localhost:8080") 
        .AllowAnyHeader()
        .AllowAnyMethod());
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.MapControllers();
app.Run();
