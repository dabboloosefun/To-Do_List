class TaskService : ITaskService {
    private readonly IRepository<TaskItem> _repository;
    private readonly List<TaskItem> _tasks;
    public TaskService(IRepository<TaskItem> repository) {
        _repository = repository;
        _tasks = _repository.Load();
    }
    public IEnumerable<TaskItem> GetAllTasks() => _tasks;
    public TaskItem? GetTaskById(int id)
    {
        foreach (TaskItem task in _tasks)
        {
            if (task.Id == id)
            {
                return task;
            }
        }
        
        return null;
    }

    public void UpdateTask(TaskItem task)
    {
        int taskIdx = -1;

        for (int i = 0; i < _tasks.Count(); i++)
        {
            if (_tasks[i].Id == task.Id)
            {
                taskIdx = i;
            }
        }
        if (taskIdx > -1)
        {
            _tasks[taskIdx] = task;
            _repository.Save(_tasks);
        }
    }

    public void AddTask(string description, int priority, List<int>? assignedMembers) {
        int newId = _tasks.Count > 0 ? _tasks[_tasks.Count - 1].Id + 1 :
        1;
        var newTask = new TaskItem(assignedMembers) { Id = newId, Description =
        description, Status = -1, Priority = priority};
        _tasks.Add(newTask);
        _repository.Save(_tasks);
    }
    public void RemoveTask(int id) {
        var task = _tasks.Find(t => t.Id == id);
        if (task != null) {
            _tasks.Remove(task);
            _repository.Save(_tasks);
        }
    }
    public void ToggleTaskStatus(int id, int status) {
        var task = _tasks.Find(t => t.Id == id);
        if (task != null && status < 2) {
            task.Status = status;
            _repository.Save(_tasks);
        }
    }
} 