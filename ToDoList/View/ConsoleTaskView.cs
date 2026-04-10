using System.Diagnostics;
using System.Xml;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _taskService;
    private readonly IMemberService _memberService;
    private Options? options;

    public ConsoleTaskView(ITaskService taskService, IMemberService memberService)
    {
        _taskService = taskService;
        _memberService = memberService;
    }

    public void Run()
    {
        bool LoggedIn = false;
        Member? member = null;

        while (!LoggedIn && member == null)
        {
            Format.TrueClear();
            Format.CentreCursor();
            Console.Write("LOG IN");
            Format.Pad(2);

            string name = Format.Prompt("Name: ");
            string password = Format.Prompt("Password: ", mask: true);

            Format.Pad();
            if (name == null || password == null) continue;

            Tuple<bool, Member?> result = _memberService.LogIn(name, password);
            LoggedIn = result.Item1;
            member = result.Item2;
        }

        while (LoggedIn && member != null)
        {
            options = new Options(member, _taskService, _memberService);
            Format.Pad(3);
            options.DisplayTasksTruncated(_taskService.GetAllTasks());
            string? option = options.DisplayOptions();

            switch (option)
            {
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
                    options.ShowDependenciesOption();
                    break;

                case "8":
                    Console.ResetColor();
                    Format.TrueClear();
                    return;

                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}