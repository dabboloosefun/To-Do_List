class Program {
    static void Main(string[] args){
    // Dependency injection: wiring up our components
        string filePath = "tasks.json";
        IRepository<TaskItem> repository = new JsonRepository<TaskItem>(filePath);
        ITaskService service = new TaskService(repository);
        ITaskView view = new ConsoleTaskView(service);
        // Run the view
        view.Run();
    }
}