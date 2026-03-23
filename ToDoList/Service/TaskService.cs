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

    public void AddTask(string description, int priority, List<int>? assignedMembers, List<int> dependantOn) {
        int newId = _tasks.Count > 0 ? _tasks[_tasks.Count - 1].Id + 1 :
        1;
        var newTask = new TaskItem(assignedMembers, dependantOn) { Id = newId, Description =
        description, Status = -1, Priority = priority};
        _tasks.Add(newTask);
        _repository.Save(_tasks);
    }
    public void RemoveTask(int id) {
        TaskItem? task = GetTaskById(id);
        if (task != null) {
            _tasks.Remove(task);
            _repository.Save(_tasks);
        }
    }

    public bool DependanciesDone(TaskItem item)
    {
        int amountDone = 0;
        foreach (int taskId in item.DependantOn)
        {
            TaskItem? dependancy = GetTaskById(taskId);
            if (dependancy == null)
            {
                amountDone--;
                continue;
            }

            if (dependancy.Status == 2)
            {
                amountDone++;
                continue;
            }

            amountDone--;
        }
        return amountDone >= item.DependantOn.Count ? true : false;
    }

    public bool ToggleTaskStatus(int id, int status) {
        TaskItem? task = GetTaskById(id);

        if (task != null && status < 2) {
            if (DependanciesDone(task)) 
            {
                task.Status = status;
                _repository.Save(_tasks);
                return true;
            }
        }
        
        return false;
    }
} 