using System.Diagnostics;
using System.Xml;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _taskService;
    private readonly IMemberService _memberService;

    public ConsoleTaskView(ITaskService taskService, IMemberService memberService)
    {
        _taskService = taskService;
        _memberService = memberService;
    }

    void DisplayTasks(IEnumerable<TaskItem> tasks)
    {
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

    string Prompt(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    string? PromptReadKey(string prompt)
    {
        Console.WriteLine(prompt);
        return Console.ReadKey().ToString();
    }

    public string? DisplayOptions(Member member)
    {
        Console.WriteLine($"Currently logged in as {member.Name} [{member.Id}]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. Remove Task");
        Console.WriteLine("3. Toggle Task State");
        Console.WriteLine("4. Manage Task Assignment");
        Console.WriteLine("5. Manage Task Dependancy");
        Console.WriteLine("6. Exit");
        return Console.ReadKey().ToString();
    }

    public void AddTaskOption(Member member)
    {
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
            string assignYourself = Prompt("Would you like to assign yourself? (y/n): ");
            if (int.TryParse(priorityStr, out int priority))
            {
                if (priority > 3 || priority < 1)
                {
                    looped = true;
                }
                if (priority < 4 && priority > 0)
                {
                    _taskService.AddTask(description, priority -2, assignYourself.ToLower().Contains('y') ? new List<int> {member.Id} : null, null);
                    validPriority = true;
                }
            }
        } 
    }

    public void RemoveTaskOption()
    {
        string removeIdStr = Prompt ("Enter task id to remove: ");
        if (int.TryParse(removeIdStr, out int removeId)) {
            _taskService.RemoveTask(removeId);
        }
    }

    public void ToggleTaskStatusOption()
    {
        string toggleIdStr = Prompt ("ENTER TASK ID: ");
        Console.WriteLine("[1] TO DO");
        Console.WriteLine("[2] IN PROGRESS");
        Console.WriteLine("[3] DONE");
        string? statusOption = PromptReadKey("SELECT AN OPTION");
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
        string taskIdstr = Prompt("Please enter a task ID: ");
        if (int.TryParse(taskIdstr, out int taskId))
        {
            TaskItem? task = _taskService.GetTaskById(taskId);
            if (task != null && int.TryParse(Prompt("How many members would you like to assign? "), out int memberAmount))
            {
                for (int i = 0; i < memberAmount; i++)
                {
                    if (int.TryParse(Prompt("Input member id: "), out int memberIdOut))
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
        throw new NotImplementedException();    
    }

    public void Run() {
        bool LoggedIn = false;
        Member? member = null;
        
        while(!LoggedIn && member == null)
        {
            Console.WriteLine("Log in to continue:");
            string name = Prompt("Name: ");
            string password = Prompt("Password: ");

            Tuple<bool, Member?> result = _memberService.LogIn(name, password);
            LoggedIn = result.Item1;
            member = result.Item2;
        }

        while (LoggedIn && member != null) {

            DisplayTasks(_taskService.GetAllTasks());
            string? option =  DisplayOptions(member);

            switch (option) {
                case "1":
                    AddTaskOption(member);
                    break;

                case "2":
                    RemoveTaskOption();
                    break;

                case "3":
                    ToggleTaskStatusOption();
                    break;

                case "4":
                    TaskAssignmentOption();
                    break;

                case "5":
                    Console.WriteLine("\nFilter Options:");
                    Console.WriteLine("1. Filter by Priority");
                    Console.WriteLine("2. Filter by Status");
                    Console.WriteLine("3. Filter by Date");
                    string filterOption = Prompt("Select filter: ");

                    switch (filterOption)
                    {
                        case "1":
                            Console.WriteLine("1. Low");
                            Console.WriteLine("2. Medium");
                            Console.WriteLine("3. High");
                            string priorityStr = Prompt("Select priority: ");
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
                                    Prompt("Press Enter to continue...");
                                }
                            }
                            break;

                        case "2":
                            Console.WriteLine("1. To Do");
                            Console.WriteLine("2. In Progress");
                            Console.WriteLine("3. Done");
                            string statusStr = Prompt("Select status: ");
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
                                    Prompt("Press Enter to continue...");
                                }
                            }
                            break;

                        case "3":
                            string dateStr = Prompt("Enter date (yyyy-MM-dd): ");
                            if (DateTime.TryParse(dateStr, out DateTime filterDate))
                            {
                                DisplayTasks(_taskService.GetTasksByDate(filterDate));
                                Prompt("Press Enter to continue...");
                            }
                            else
                            {
                                Console.WriteLine("Invalid date format.");
                                Prompt("Press Enter to continue...");
                            }
                            break;

                        default:
                            Console.WriteLine("Invalid filter option.");
                            break;
                    }
                    break;

                case "6":
                    TaskDependancyOption();
                    break;


                case "7":
                    return;

                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}