class TaskService : ITaskService {
    private readonly ITaskRepository _repository;
    private readonly List<TaskItem> _tasks;
    public TaskService(ITaskRepository repository) {
        _repository = repository;
        _tasks = _repository.LoadTasks();
    }
    public IEnumerable<TaskItem> GetAllTasks() => _tasks;
    public void AddTask(string description) {
        int newId = _tasks.Count > 0 ? _tasks[_tasks.Count - 1].Id + 1 :
        1;
        var newTask = new TaskItem { Id = newId, Description =
        description, Status = -1 };
        _tasks.Add(newTask);
        _repository.SaveTasks(_tasks);
    }
    public void RemoveTask(int id) {
        var task = _tasks.Find(t => t.Id == id);
        if (task != null) {
            _tasks.Remove(task);
            _repository.SaveTasks(_tasks);
        }
    }
    public void ToggleTaskStatus(int id, int status) {
        var task = _tasks.Find(t => t.Id == id);
        if (task != null && status < 2) {
            task.Status = status;
            _repository.SaveTasks(_tasks);
        }
    }
} 