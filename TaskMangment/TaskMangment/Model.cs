namespace TaskMangment;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class MyTask
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}

public class TaskContext : IdentityDbContext<IdentityUser>
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    public DbSet<MyTask> Tasks { get; set; }
}
