
using System.Diagnostics;

public class ConsoleTaskView : ITaskView {
    private readonly ITaskService _service;

    public ConsoleTaskView(ITaskService service) {
        _service = service;
    }
    
    void DisplayTasks(IEnumerable<TaskItem> tasks) {
        Console.Clear();
        Console.WriteLine("==== ToDo List ====");
        foreach (var task in tasks)
        {
            if (task.Status == -1) Console.WriteLine($"To Do: {task.Description}, {task.Id}");
        }
        Console.WriteLine("==== In Progress List ====");
        foreach (var task in tasks)
        {
            if (task.Status == 0) Console.WriteLine($"In Progress: {task.Description}, {task.Id}");
        }
        Console.WriteLine("==== Done List ====");
        foreach (var task in tasks)
        {
            if (task.Status == 1) Console.WriteLine($"Done: {task.Description}, {task.Id}");
        }
    }

    string Prompt(string prompt) {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    public void Run() {
        while (true) {
            DisplayTasks(_service.GetAllTasks());
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Remove Task");
            Console.WriteLine("3. Toggle Task State");
            Console.WriteLine("4. Exit");
            string option=Prompt("Select an option: ");
            switch (option) {
                case "1":
                    string description = Prompt ("Enter task description: ");
                        _service.AddTask(description);
                    break;
                case "2":
                    string removeIdStr = Prompt ("Enter task id to remove: ");
                    if (int.TryParse(removeIdStr, out int removeId)) {
                        _service.RemoveTask(removeId);
                    }
                    break;
                case "3":
                    string toggleIdStr = Prompt ("Enter task id to toggle: ");
                    Console.WriteLine("1. To Do");
                    Console.WriteLine("2. In Progress");
                    Console.WriteLine("3. Done");
                    string statusOption =Prompt("Select an option: ");
                    if (int.TryParse(toggleIdStr, out int toggleId)) {
                        _service.ToggleTaskStatus(toggleId, statusOption switch {
                            "1" => -1,
                            "2" => 0,
                            "3" => 1,
                            _ => 2                            
                        });
                    }
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
} 