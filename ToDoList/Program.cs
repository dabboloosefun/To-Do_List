class Program {
    static void Main(string[] args){
    // Dependency injection: wiring up our components
        string TfilePath = "Data/tasks.json";
        string MfilePath = "Data/members.json";
        IRepository<TaskItem> taskRepository = new JsonRepository<TaskItem>(TfilePath);
        IRepository<Member> memberRepository = new JsonRepository<Member>(MfilePath);
        ITaskService taskService = new TaskService(taskRepository);
        IMemberService memberService = new MemberService(memberRepository);
        ITaskView view = new ConsoleTaskView(taskService, memberService);
        // Run the view
        view.Run();
    }
}