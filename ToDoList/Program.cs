class Program {
    static void Main(string[] args){
        Format.TrueClear();
        Format.WriteTitle("SELECT YOUR COLLECTION TYPE");

        Format.WriteList([
            "Array", "LinkedList", "BinaryTree", "HashMap"
        ], (Console.WindowHeight - 4) / 2);

        Console.WriteLine();

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

        IRepository<TaskItem> taskRepository = new JsonRepository<TaskItem>("./Data/tasks.json", option);
        IRepository<Member> memberRepository = new JsonRepository<Member>("./Data/members.json", option);

        ITaskService taskService = new TaskService(taskRepository);
        IMemberService memberService = new MemberService(memberRepository);
        ITaskView view = new ConsoleTaskView(taskService, memberService);
        // Run the view
        view.Run();
    }
}