using System.Collections.Concurrent;
using System.Xml;
using CommunityToolkit.Common;

public class Options
{
    private Member _member { get; set; }
    private ITaskService _taskService { get; set; }
    private IMemberService _memberService { get; set; }

    public Options(Member member, ITaskService taskService, IMemberService memberService)
    {
        _member = member;
        _taskService = taskService;
        _memberService = memberService;
    }

    public void DisplayTasksTruncated(IEnumerable<TaskItem> tasks)
    {
        int headerLength = $"Currently logged in as {_member.Name} [{_member.Id}]".Length + 2;
        
        int left = ((Console.WindowWidth - headerLength) / 2) + headerLength;
        int top = 1;
        int maxWidth = Console.WindowWidth - left - 1;

        Format.TrueClear();
        Console.SetCursorPosition(left, top);

        Console.WriteLine("To Do".PadLeft("To Do".Length + (maxWidth - "To Do".Length) / 2, '=').PadRight(maxWidth, '='));
        foreach (var task in tasks)
        {
            
            if (task.Status == -1) 
            {
                top++;
                Console.SetCursorPosition(left, top);
                string taskStr = $"{(_taskService.CanStartTask(task) ? "" : "LOCKED")} [{task.Id}] [{task.Priority switch
                {
                    -1 => "Low",
                    0 => "Medium",
                    1 => "High",
                    _ => "None"
                }}] {task.Description}";
                Console.WriteLine(StringExtensions.Truncate(taskStr, maxWidth));
            }
        }

        top += 2;
        Console.SetCursorPosition(left, top);
        Console.WriteLine("In Progress".PadLeft("In Progress".Length + (maxWidth - "In Progress".Length) / 2, '=').PadRight(maxWidth, '='));
        foreach (var task in tasks)
        {
            if (task.Status == 0)
            {
                top++;
                Console.SetCursorPosition(left, top);
                string taskStr = $"[{task.Id}] [{task.Priority switch
                {
                    -1 => "Low",
                    0 => "Medium",
                    1 => "High",
                    _ => "None"
                }}] {task.Description}";
                Console.WriteLine(StringExtensions.Truncate(taskStr, maxWidth));
            }
        }

        top += 2;
        Console.SetCursorPosition(left, top);
        Console.WriteLine("Done".PadLeft("Done".Length + (maxWidth - "Done".Length) / 2, '=').PadRight(maxWidth, '='));
        foreach (var task in tasks)
        {
            if (task.Status == 1)
            {
                top++;
                Console.SetCursorPosition(left, top);
                string taskStr = $"[{task.Id}] [{task.Priority switch
                {
                    -1 => "Low",
                    0 => "Medium",
                    1 => "High",
                    _ => "None"
                }}] {task.Description}";
                Console.WriteLine(StringExtensions.Truncate(taskStr, maxWidth));
            }
        }
    }

    public void DisplayTasks(IEnumerable<TaskItem> tasks)
    {
        int left = 1;
        int top = 1;
        int maxWidth = Console.WindowWidth - 2;

        Format.TrueClear();
        Console.SetCursorPosition(left, top);

        Console.WriteLine("To Do".PadLeft("To Do".Length + (maxWidth - "To Do".Length) / 2, '=').PadRight(maxWidth, '='));
        foreach (var task in tasks)
        {
            
            if (task.Status == -1) 
            {
                top++;
                Console.SetCursorPosition(left, top);
                string taskStr = $"{(_taskService.CanStartTask(task) ? "" : "LOCKED")} [{task.Id}] [{task.Priority switch
                {
                    -1 => "Low",
                    0 => "Medium",
                    1 => "High",
                    _ => "None"
                }}] {task.Description}";
                Format.WriteCentered(StringExtensions.Truncate(taskStr, maxWidth));
            }
        }

        top += 2;
        Console.SetCursorPosition(left, top);
        Console.WriteLine("In Progress".PadLeft("In Progress".Length + (maxWidth - "In Progress".Length) / 2, '=').PadRight(maxWidth, '='));
        foreach (var task in tasks)
        {
            if (task.Status == 0)
            {
                top++;
                Console.SetCursorPosition(left, top);
                string taskStr = $"[{task.Id}] [{task.Priority switch
                {
                    -1 => "Low",
                    0 => "Medium",
                    1 => "High",
                    _ => "None"
                }}] {task.Description}";
                Console.WriteLine(StringExtensions.Truncate(taskStr, maxWidth));
            }
        }

        top += 2;
        Console.SetCursorPosition(left, top);
        Console.WriteLine("Done".PadLeft("Done".Length + (maxWidth - "Done".Length) / 2, '=').PadRight(maxWidth, '='));
        foreach (var task in tasks)
        {
            if (task.Status == 1)
            {
                top++;
                Console.SetCursorPosition(left, top);
                string taskStr = $"[{task.Id}] [{task.Priority switch
                {
                    -1 => "Low",
                    0 => "Medium",
                    1 => "High",
                    _ => "None"
                }}] {task.Description}";
                Console.WriteLine(StringExtensions.Truncate(taskStr, maxWidth));
            }
        }
    }

    public string? DisplayOptions()
    {
        Format.WriteTitle($"Currently logged in as {_member.Name} [{_member.Id}]");

        Format.WriteList([
            "Add Task", "Remove Task", "Toggle Task Status", "Manage Task Assignment", "Manage Task Dependency", "Filter", "Show Dependencies", "Exit"
            ], (Console.WindowHeight - 8) / 2);

        return Format.PromptReadKey("Select an option: ");
    }

    public void ShowDependenciesOption()
    {
        Format.TrueClear();
        DisplayTasksTruncated(_taskService.GetAllTasks());
        Format.WriteTitle("DEPENDANCY GRAPH");
        Format.CentreCursor();

        string? taskIdstr = Format.Prompt("Enter task Id: ");
        if (int.TryParse(taskIdstr, out int taskId))
        {
            DependencyGraph(taskId);
        }
        Format.Pad(1);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public void DependencyGraph(int taskId, int depth = 0)
    {
        TaskItem? task = _taskService.GetTaskById(taskId);
        if (task == null) return;

        for (int i = 0; i < depth; i++)
        {
            Console.Write("\t");
        }

        if(depth == 0)
        {
            Format.TrueClear();
            Console.SetCursorPosition(1,1);
            Console.WriteLine($"-[{task.Id}] {task.Description}");
        }

        else Console.WriteLine($"|-{task.Description}");
        for (int i = 0; i < task.DependantOn.Count(); i++)
        {
            DependencyGraph(task.DependantOn[i], depth + 1);
        }
    }

    public void AddTaskOption(Member member)
    {
        bool validPriority = false;

        Format.TrueClear();
        Format.WriteTitle("DESCRIPTION");
        Format.CentreCursor();
    
        string description = Format.Prompt("Enter task description: ");

        while (!validPriority)
        {
            Format.TrueClear();
            Format.WriteTitle("STATUS OPTIONS");
            Format.WriteList([
                "Low", "Medium", "High"
            ], (Console.WindowHeight - 3) / 2);

            Format.Pad(1);

            string priorityStr = Format.PromptReadKey("Select task priority: ");
            
            if (int.TryParse(priorityStr, out int priority))
            {
                if (priority < 4 && priority > 0)
                {
                    Format.TrueClear();
                    Format.WriteTitle("AUTO ASSIGNMENT");
                    Format.CentreCursor();
                    
                    string assignYourself = Format.PromptReadKey("Would you like to assign yourself? (y/n): ");
                    _taskService.AddTask(description, priority -2, assignYourself.ToLower().Contains('y') ? new List<int> {member.Id} : null);
                    validPriority = true;
                }
            }
        } 
    }

    public void RemoveTaskOption()
    {
        Format.TrueClear();
        DisplayTasksTruncated(_taskService.GetAllTasks());
        Format.WriteTitle("REMOVE");
        Format.CentreCursor();
        
        string removeIdStr = Format.Prompt("Enter task Id: ");
        if (int.TryParse(removeIdStr, out int removeId)) {
            _taskService.RemoveTask(removeId);
        }
    }

    public void ToggleTaskStatusOption()
    {
        Format.TrueClear();
        DisplayTasksTruncated(_taskService.GetAllTasks());
        Format.WriteTitle("TOGGLE STATUS");
        Format.CentreCursor();
        
        string toggleIdStr = Format.Prompt("Enter task Id: ");
        
        Format.TrueClear();
        Format.WriteTitle("STATUS OPTIONS");
        Format.WriteList([
            "To Do", "In Progress", "Done"
        ], (Console.WindowHeight - 3) / 2);
        Format.Pad(1);

        string? statusOption = Format.PromptReadKey("Select task status: ");
        if (int.TryParse(toggleIdStr, out int toggleId)) {
            _taskService.ToggleTaskStatus(toggleId, statusOption switch {
                "1" => -1,
                "2" => 0,
                "3" => 1,
                _ => 2
            });
        }
    }

    public void TaskAssignmentOption()
    {
        Format.TrueClear();
        DisplayTasksTruncated(_taskService.GetAllTasks());
        Format.WriteTitle("MEMBER ASSIGNMENT");
        Format.CentreCursor();
        
        string taskIdstr = Format.Prompt("Enter task Id: ");
        if (int.TryParse(taskIdstr, out int taskId))
        {
            TaskItem? task = _taskService.GetTaskById(taskId);

            Format.TrueClear();
            Format.WriteTitle("MEMBER ASSIGNMENT");
            Format.CentreCursor();
            
            if (task != null && int.TryParse(Format.Prompt("How many members would you like to assign? "), out int memberAmount))
            {
                for (int i = 0; i < memberAmount; i++)
                {
                    Format.TrueClear();
                    Format.WriteTitle("MEMBER ASSIGNMENT");
                    Format.CentreCursor();
                    
                    if (int.TryParse(Format.Prompt($"Enter member {i + 1} Id: "), out int memberIdOut))
                    {
                        if (_memberService.GetMemberById(memberIdOut) != null && !task.AssignedMembers.Contains(memberIdOut)) task.AssignedMembers.Add(memberIdOut);
                    }
                }
                _taskService.UpdateTask(task);
            }
        }
    }
    
    public void TaskDependancyOption()
    {
        Format.TrueClear();
        DisplayTasksTruncated(_taskService.GetAllTasks());
        Format.WriteTitle("TASK DEPENDENCY");
        Format.CentreCursor();
        
        string taskIdstr = Format.Prompt("Enter task Id: ");

        if (int.TryParse(taskIdstr, out int taskId))
        {
            TaskItem? task = _taskService.GetTaskById(taskId);
            if (task == null) return;

            Format.TrueClear();
            Format.WriteTitle("TASK DEPENDENCY");
            Format.CentreCursor();
            
            string dependencyIdstr = Format.Prompt("Enter dependency Id: ");

            if (int.TryParse(dependencyIdstr, out int dependencyId))
            {
                _taskService.AddDependency(task, dependencyId);
            }
        }
    }

    public void FilterOption()
    {
        Format.TrueClear();
        Format.WriteTitle("FILTER OPTIONS");

        Format.WriteList([
            "Filter by Priority", "Filter by Status", "Filter by Date"
        ], (Console.WindowHeight - 3) / 2);
        Format.Pad(1);

        string filterOption = Format.PromptReadKey("Select filter: ");

        switch (filterOption)
        {
            case "1":
                Format.TrueClear();
                Format.WriteTitle("FILTER OPTIONS");

                Format.WriteList([
                    "Low", "Medium", "High"
                ], (Console.WindowHeight - 3) / 2);
                Format.Pad(1);

                string priorityStr = Format.PromptReadKey("Select priority: ");
                if (int.TryParse(priorityStr, out int priorityFilter))
                {
                    int priorityValue = priorityFilter switch
                    {
                        1 => -1,
                        2 => 0,
                        3 => 1,
                        _ => -99
                    };
                    if (priorityValue != -99)
                    {
                        DisplayTasks(_taskService.GetTasksByPriority(priorityValue));
                        Format.Prompt("Press Enter to continue...");
                    }
                }
                break;

            case "2":
                Format.TrueClear();
                Format.WriteTitle("FILTER BY STATUS");

                Format.WriteList([
                    "To Do", "In Progress", "Done"
                ], (Console.WindowHeight - 3) / 2);
                Format.Pad(1);

                string statusStr = Format.PromptReadKey("Select status: ");
                if (int.TryParse(statusStr, out int statusFilter))
                {
                    int statusValue = statusFilter switch
                    {
                        1 => -1,
                        2 => 0,
                        3 => 1,
                        _ => -99
                    };
                    if (statusValue != -99)
                    {
                        DisplayTasks(_taskService.GetTasksByStatus(statusValue));
                        Format.Prompt("Press Enter to continue...");
                    }
                }
                break;

            case "3":
                Format.TrueClear();
                Format.WriteTitle("FILTER BY DATE");
                Format.CentreCursor();

                string dateStr = Format.Prompt("Enter date (yyyy-MM-dd): ");
                if (DateTime.TryParse(dateStr, out DateTime filterDate))
                {
                    DisplayTasks(_taskService.GetTasksByDate(filterDate));
                    Format.Prompt("Press Enter to continue...");
                }
                else
                {
                    Console.WriteLine("Invalid date format.");
                    Format.Prompt("Press Enter to continue...");
                }
                break;

            default:
                Console.WriteLine("Invalid filter option.");
                break;
        }

        
    }
}