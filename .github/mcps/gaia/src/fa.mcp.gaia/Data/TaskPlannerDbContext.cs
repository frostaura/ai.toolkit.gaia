using FrostAura.MCP.Gaia.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// Entity Framework database context for Task data with hierarchical structure
/// </summary>
public class TaskPlannerDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the TaskPlannerDbContext
    /// </summary>
    /// <param name="options">DbContext options</param>
    /// <param name="configuration">Configuration instance</param>
    public TaskPlannerDbContext(DbContextOptions<TaskPlannerDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Project plans DbSet
    /// </summary>
    public DbSet<ProjectPlan> Plans { get; set; }

    /// <summary>
    /// Task items DbSet
    /// </summary>
    public DbSet<TaskItem> Tasks { get; set; }

    /// <summary>
    /// Memories DbSet
    /// </summary>
    public DbSet<Memory> Memories { get; set; }

    /// <summary>
    /// Configures the database connection and model
    /// </summary>
    /// <param name="optionsBuilder">Options builder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                                   "Data Source=/Users/deanmartin/Desktop/Projects/FrostAura/ai.toolkit.gaia/.gaia/Gaia.TaskPlanner.db";
            optionsBuilder.UseSqlite(connectionString);
        }
    }

    /// <summary>
    /// Configures entity relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ProjectPlan entity
        modelBuilder.Entity<ProjectPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.AiAgentBuildContext).IsRequired();
            entity.Property(e => e.CreatorIdentity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Configure relationship with TaskItem
            entity.HasMany(e => e.Tasks)
                  .WithOne(e => e.Plan)
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TaskItem entity
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.PlanId).IsRequired().HasMaxLength(36);
            entity.Property(e => e.ParentTaskId).HasMaxLength(36);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.AcceptanceCriteria).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.EstimateHours).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CompletedAt);

            // Configure JSON columns for Tags and Groups
            entity.Property(e => e.TagsJson).HasColumnName("Tags");
            entity.Property(e => e.GroupsJson).HasColumnName("Groups");

            // Configure self-referencing relationship for parent/child tasks
            entity.HasOne(e => e.ParentTask)
                  .WithMany(e => e.ChildTasks)
                  .HasForeignKey(e => e.ParentTaskId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with ProjectPlan
            entity.HasOne(e => e.Plan)
                  .WithMany(e => e.Tasks)
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Memory entity
        modelBuilder.Entity<Memory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.LastAccessedAt);
            entity.Property(e => e.AccessCount).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.Priority).IsRequired().HasDefaultValue(3);

            // Configure JSON column for Tags
            entity.Property(e => e.TagsJson).HasColumnName("Tags");
        });

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Adds a new project plan
    /// </summary>
    /// <param name="plan">Project plan to add</param>
    public async Task AddPlanAsync(ProjectPlan plan)
    {
        if (await Plans.AnyAsync(p => p.Id == plan.Id))
        {
            throw new InvalidOperationException($"A plan with ID '{plan.Id}' already exists.");
        }

        Plans.Add(plan);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Adds a new Task item
    /// </summary>
    /// <param name="task">Task to add</param>
    public async Task AddTaskAsync(TaskItem task)
    {
        // Verify plan exists
        if (!await Plans.AnyAsync(p => p.Id == task.PlanId))
        {
            throw new ArgumentException($"Plan with ID '{task.PlanId}' not found.");
        }

        // Check if task ID already exists
        if (await Tasks.AnyAsync(t => t.Id == task.Id))
        {
            throw new InvalidOperationException($"A task with ID '{task.Id}' already exists.");
        }

        // Verify parent task exists if specified
        if (!string.IsNullOrEmpty(task.ParentTaskId))
        {
            if (!await Tasks.AnyAsync(t => t.Id == task.ParentTaskId))
            {
                throw new ArgumentException($"Parent task with ID '{task.ParentTaskId}' not found.");
            }
        }

        Tasks.Add(task);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Gets a project plan by ID with hierarchical Tasks populated
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>Project plan with Tasks or null if not found</returns>
    public async Task<ProjectPlan?> GetPlanByIdAsync(string planId)
    {
        var plan = await Plans.Include(p => p.Tasks)
                              .FirstOrDefaultAsync(p => p.Id == planId);

        if (plan != null)
        {
            // Build hierarchical structure for the Tasks property
            var flatTasks = plan.Tasks.ToList();
            plan.Tasks = BuildTaskHierarchy(flatTasks);
        }

        return plan;
    }

    /// <summary>
    /// Gets all project plans with hierarchical Tasks populated
    /// </summary>
    /// <returns>List of project plans with Tasks</returns>
    public async Task<List<ProjectPlan>> GetAllPlansAsync()
    {
        var plans = await Plans.Include(p => p.Tasks).ToListAsync();

        // Build hierarchical structure for each plan's tasks
        foreach (var plan in plans)
        {
            var flatTasks = plan.Tasks.ToList();
            plan.Tasks = BuildTaskHierarchy(flatTasks);
        }

        return plans;
    }

    /// <summary>
    /// Gets Task items for a specific plan
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>List of Task items for the plan</returns>
    public async Task<List<TaskItem>> GetTasksByPlanIdAsync(string planId)
    {
        return await Tasks.Where(t => t.PlanId == planId).ToListAsync();
    }

    /// <summary>
    /// Gets a Task item by ID
    /// </summary>
    /// <param name="taskId">ID of the Task item</param>
    /// <returns>Task item or null if not found</returns>
    public async Task<TaskItem?> GetTaskByIdAsync(string taskId)
    {
        return await Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
    }

    /// <summary>
    /// Updates a task in the database
    /// </summary>
    /// <param name="task">Updated task item</param>
    public async Task UpdateTaskAsync(TaskItem task)
    {
        var existingTask = await Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        if (existingTask == null)
        {
            throw new ArgumentException($"Task with ID '{task.Id}' not found.");
        }

        // Update the task properties
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.AcceptanceCriteria = task.AcceptanceCriteria;
        existingTask.Status = task.Status;
        existingTask.Tags = new List<string>(task.Tags);
        existingTask.Groups = new List<string>(task.Groups);
        existingTask.EstimateHours = task.EstimateHours;
        existingTask.UpdatedAt = task.UpdatedAt;
        existingTask.CompletedAt = task.CompletedAt;

        await SaveChangesAsync();
    }

    /// <summary>
    /// Builds hierarchical structure for Tasks
    /// </summary>
    /// <param name="flatTasks">Flat list of Tasks</param>
    /// <returns>List of root Tasks with children populated</returns>
    private List<TaskItem> BuildTaskHierarchy(List<TaskItem> flatTasks)
    {
        var taskDict = flatTasks.ToDictionary(t => t.Id, t =>
            new TaskItem
            {
                Id = t.Id,
                PlanId = t.PlanId,
                ParentTaskId = t.ParentTaskId,
                Title = t.Title,
                Description = t.Description,
                AcceptanceCriteria = t.AcceptanceCriteria,
                Status = t.Status,
                Tags = new List<string>(t.Tags),
                Groups = new List<string>(t.Groups),
                EstimateHours = t.EstimateHours,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CompletedAt = t.CompletedAt,
                Children = new List<TaskItem>()
            });

        var rootTasks = new List<TaskItem>();

        foreach (var task in taskDict.Values)
        {
            if (string.IsNullOrEmpty(task.ParentTaskId))
            {
                rootTasks.Add(task);
            }
            else
            {
                if (taskDict.TryGetValue(task.ParentTaskId, out var parent))
                {
                    parent.Children.Add(task);
                }
                else
                {
                    // Parent not found, treat as root task
                    rootTasks.Add(task);
                }
            }
        }

        return rootTasks;
    }
}
