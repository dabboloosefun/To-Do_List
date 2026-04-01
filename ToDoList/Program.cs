class Program {
    static void Main(string[] args){
        // Dependency injection: wiring up our components
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string dataPath = Path.Combine(basePath, "Data");

        Directory.CreateDirectory(dataPath);

        string TfilePath = Path.Combine(dataPath, "tasks.json");
        string MfilePath = Path.Combine(dataPath, "members.json");
        IRepository<TaskItem> taskRepository = new JsonRepository<TaskItem>(TfilePath);
        IRepository<Member> memberRepository = new JsonRepository<Member>(MfilePath);
        ITaskService taskService = new TaskService(taskRepository);
        IMemberService memberService = new MemberService(memberRepository);
        ITaskView view = new ConsoleTaskView(taskService, memberService);
        // Run the view
        view.Run();
    }
}