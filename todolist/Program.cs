using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToDoApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllersWithViews();
var app = builder.Build();



app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();

namespace ToDoApi.Models
{
    public class ToDoItem
    {
        public required int  Id { get; set; }
        public required string Task { get; set; }
    }
}

namespace ToDoApi.Controllers
{
    [Route("api/todo")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private static readonly string filePath = "todo.json";

        private List<string> LoadTasks()
        {
            if (!System.IO.File.Exists(filePath))
                return new List<string>();

            var json = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }

        private void SaveTasks(List<string> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, json);
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok(LoadTasks());
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] string newTask)
        {
            var tasks = LoadTasks();
            tasks.Add(newTask);
            SaveTasks(tasks);
            return Ok(tasks);
        }

        [HttpDelete]
        public IActionResult DeleteTask([FromBody] string taskToDelete)
        {
            var tasks = LoadTasks();
            if (!tasks.Remove(taskToDelete))
                return NotFound("Task not found");

            SaveTasks(tasks);
            return Ok(tasks);
        }
    }
}