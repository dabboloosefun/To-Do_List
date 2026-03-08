
using System.ComponentModel.Design;
using System.Diagnostics;

public class ConsoleTaskView : ITaskView {
    private readonly ITaskService _taskService;
    private readonly IMemberService _memberService;

    public ConsoleTaskView(ITaskService taskService, IMemberService memberService) {
        _taskService = taskService;
        _memberService = memberService;
    }
    
    void DisplayTasks(IEnumerable<TaskItem> tasks) {
        Console.Clear();
        Console.WriteLine("==== ToDo List ====");
        foreach (var task in tasks)
        {
            if (task.Status == -1) Console.WriteLine($"To Do: [{task.Id}] [{task.Priority switch
            {
                -1 => "Low",
                0 => "Medium",
                1 => "High",
                _ => "None"
            }}] {task.Description}");
        }
        Console.WriteLine("==== In Progress List ====");
        foreach (var task in tasks)
        {
            if (task.Status == 0) Console.WriteLine($"In Progress: [{task.Id}] [{task.Priority switch
            {
                -1 => "Low",
                0 => "Medium",
                1 => "High",
                _ => "None"
            }}] {task.Description}");
        }
        Console.WriteLine("==== Done List ====");
        foreach (var task in tasks)
        {
            if (task.Status == 1) Console.WriteLine($"Done: [{task.Id}] [{task.Priority switch
            {
                -1 => "Low",
                0 => "Medium",
                1 => "High",
                _ => "None"
            }}] {task.Description}");
        }
    }

    string Prompt(string prompt) {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    public void Run() {
        bool LoggedIn = false;
        Member? member = null;
        bool failedOnce = false;
        while (!LoggedIn)
        {
            Console.Clear();
            if (failedOnce) Console.WriteLine("Incorrect account details, please try again.");
            Console.WriteLine("Please log in to your company account");
            string name = Prompt("Enter your account name: "); 
            string password = Prompt("Enter your password: ");
            var result = _memberService.LogIn(name, password);
            member = result.Item2;
            LoggedIn = result.Item1;
            failedOnce = true;
        }
        while (LoggedIn) {
            DisplayTasks(_taskService.GetAllTasks());
            Console.WriteLine($"\n[{member!.Name}]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Remove Task");
            Console.WriteLine("3. Toggle Task State");
            Console.WriteLine("4. Exit");
            string option=Prompt("Select an option: ");
            switch (option) {
                case "1":
                    bool validPriority = false;
                    bool looped = false;

                    string description = Prompt ("Enter task description: ");

                    while (!validPriority)
                    {
                        Console.WriteLine("1. Low");
                        Console.WriteLine("2. Medium");
                        Console.WriteLine("3. High");
                        if (looped) Console.WriteLine("Invalid priority choice. Please enter 1 2 or 3.");
                        string priorityStr = Prompt ("Choose task priority: ");
                        if (int.TryParse(priorityStr, out int priority))
                        {
                            if (priority > 3 || priority < 1)
                            {
                                looped = true;
                            }
                            if (priority < 4 && priority > 0)
                            {
                                _taskService.AddTask(description, priority -2);
                                validPriority = true;
                            }
                        }
                    }
                    break;
                case "2":
                    string removeIdStr = Prompt ("Enter task id to remove: ");
                    if (int.TryParse(removeIdStr, out int removeId)) {
                        _taskService.RemoveTask(removeId);
                    }
                    break;
                case "3":
                    string toggleIdStr = Prompt ("Enter task id to toggle: ");
                    Console.WriteLine("1. To Do");
                    Console.WriteLine("2. In Progress");
                    Console.WriteLine("3. Done");
                    string statusOption =Prompt("Select an option: ");
                    if (int.TryParse(toggleIdStr, out int toggleId)) {
                        _taskService.ToggleTaskStatus(toggleId, statusOption switch {
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