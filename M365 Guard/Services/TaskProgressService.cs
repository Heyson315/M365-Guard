using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace M365_Guard.Services;

/// <summary>
/// Manages parallel task execution with progress reporting
/// Allows user interaction while long-running operations execute in background
/// </summary>
public class TaskProgressService
{
    private readonly List<BackgroundTask> _activeTasks = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Run an async operation and track its progress
    /// </summary>
    public async Task RunTaskAsync(string taskName, Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        var task = new BackgroundTask
        {
            Id = Guid.NewGuid(),
            Name = taskName,
            StartTime = DateTime.UtcNow,
            Status = TaskStatus.Running
        };

        lock (_lockObject)
        {
            _activeTasks.Add(task);
        }

        try
        {
            ShowTaskStarted(task);
            await operation(cancellationToken);
            task.Status = TaskStatus.Completed;
            ShowTaskCompleted(task);
        }
        catch (OperationCanceledException)
        {
            task.Status = TaskStatus.Cancelled;
            ShowTaskCancelled(task);
        }
        catch (Exception ex)
        {
            task.Status = TaskStatus.Failed;
            task.ErrorMessage = ex.Message;
            ShowTaskFailed(task);
        }
        finally
        {
            lock (_lockObject)
            {
                _activeTasks.Remove(task);
            }
        }
    }

    /// <summary>
    /// Run multiple tasks in parallel
    /// </summary>
    public async Task RunParallelTasksAsync(params (string name, Func<CancellationToken, Task> operation)[] tasks)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var parallelTasks = tasks.Select(t => RunTaskAsync(t.name, t.operation, cancellationTokenSource.Token)).ToList();

        try
        {
            await Task.WhenAll(parallelTasks);
            Console.WriteLine("\n✅ All tasks completed successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ One or more tasks failed: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Show active tasks status
    /// </summary>
    public void ShowStatus()
    {
        lock (_lockObject)
        {
            if (!_activeTasks.Any())
            {
                Console.WriteLine("✅ No active tasks\n");
                return;
            }

            Console.WriteLine("\n═══════════════════════════════════════════════════════");
            Console.WriteLine("   📊 Active Tasks");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");

            foreach (var task in _activeTasks)
            {
                var elapsed = DateTime.UtcNow - task.StartTime;
                var icon = task.Status switch
                {
                    TaskStatus.Running => "⏳",
                    TaskStatus.Completed => "✅",
                    TaskStatus.Failed => "❌",
                    TaskStatus.Cancelled => "⏹️",
                    _ => "❓"
                };

                Console.WriteLine($"{icon} {task.Name}");
                Console.WriteLine($"   Status: {task.Status}");
                Console.WriteLine($"   Duration: {elapsed.TotalSeconds:F1}s");

                if (!string.IsNullOrEmpty(task.ErrorMessage))
                    Console.WriteLine($"   Error: {task.ErrorMessage}");

                Console.WriteLine();
            }

            Console.WriteLine("═══════════════════════════════════════════════════════\n");
        }
    }

    private void ShowTaskStarted(BackgroundTask task)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⏳ Started: {task.Name}");
        Console.ResetColor();
    }

    private void ShowTaskCompleted(BackgroundTask task)
    {
        var elapsed = (DateTime.UtcNow - task.StartTime).TotalSeconds;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Completed: {task.Name} ({elapsed:F1}s)");
        Console.ResetColor();
    }

    private void ShowTaskFailed(BackgroundTask task)
    {
        var elapsed = (DateTime.UtcNow - task.StartTime).TotalSeconds;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ Failed: {task.Name} ({elapsed:F1}s)");
        if (!string.IsNullOrEmpty(task.ErrorMessage))
            Console.WriteLine($"   Error: {task.ErrorMessage}");
        Console.ResetColor();
    }

    private void ShowTaskCancelled(BackgroundTask task)
    {
        var elapsed = (DateTime.UtcNow - task.StartTime).TotalSeconds;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⏹️  Cancelled: {task.Name} ({elapsed:F1}s)");
        Console.ResetColor();
    }
}

/// <summary>
/// Background task tracking
/// </summary>
public class BackgroundTask
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public TaskStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Task execution status
/// </summary>
public enum TaskStatus
{
    Running,
    Completed,
    Failed,
    Cancelled
}
