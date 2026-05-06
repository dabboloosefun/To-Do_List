class Program {
    static void Main(string[] args){
        // Dependency injection: wiring up our components
        Format.WriteTitle("SELECT YOUR COLLECTION TYPE");

        Format.WriteList([
            "Array", "LinkedList", "BinaryTree", "HashMap", "Manage Task Dependency"
        ], (Console.WindowHeight - 4) / 2);

        int option = 0;
        while (option <= 0)
        {
            string input = Format.PromptReadKey("Select an option:");

            try
            {
                int parsed = int.Parse(input);
                if (parsed > 0 && parsed < 5)
                {
                    option = parsed;
                }
            }
            catch (FormatException)
            {
                continue;
            }
        }
        

        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string dataPath = Path.Combine(basePath, "Data");

        Directory.CreateDirectory(dataPath);

        string TfilePath = Path.Combine(dataPath, "tasks.json");
        string MfilePath = Path.Combine(dataPath, "members.json");

        IRepository<TaskItem> taskRepository = new JsonRepository<TaskItem>(TfilePath, option);
        IRepository<Member> memberRepository = new JsonRepository<Member>(MfilePath, option);

        ITaskService taskService = new TaskService(taskRepository);
        IMemberService memberService = new MemberService(memberRepository);
        ITaskView view = new ConsoleTaskView(taskService, memberService);
        // Run the view
        view.Run();
    }
}