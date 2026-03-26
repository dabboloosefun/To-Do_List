using System.Diagnostics;
using System.Xml;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _taskService;
    private readonly IMemberService _memberService;
    private Options options;

    public ConsoleTaskView(ITaskService taskService, IMemberService memberService)
    {
        _taskService = taskService;
        _memberService = memberService;
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
            options = new Options(member, _taskService, _memberService);

            options.DisplayTasks(_taskService.GetAllTasks());
            string? option =  options.DisplayOptions(member);

            switch (option) {
                case "1":
                    options.AddTaskOption(member);
                    break;

                case "2":
                    options.RemoveTaskOption();
                    break;

                case "3":
                    options.ToggleTaskStatusOption();
                    break;

                case "4":
                    options.TaskAssignmentOption();
                    break;

                case "5":
                    options.TaskDependancyOption();
                    break;

                case "6":
                    options.FilterOption();
                    break;

                case "7":
                    string taskIdstr = Prompt("Task Id: ");
                    if (int.TryParse(taskIdstr, out int taskId))
                    {
                        options.ShowDependenciesOption(taskId);
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;

                case "8":
                    return;

                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}