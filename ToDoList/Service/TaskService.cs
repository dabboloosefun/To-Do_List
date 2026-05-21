using System.ComponentModel;
using System.Runtime.CompilerServices;

class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _repository;
    private readonly IMyCollection<TaskItem> _tasks;

    public TaskService(IRepository<TaskItem> repository)
    {
        _repository = repository;
        _tasks = _repository.Load()!;
    }

    public IEnumerable<TaskItem> GetAllTasks()
    {
        return ConvertToEnumerable(_tasks);
    }

    public IEnumerable<TaskItem> GetTasksByPriority(int priority)
    {
        return ConvertToEnumerable(_tasks.Filter(t => t.Priority == priority));
    }

    public IEnumerable<TaskItem> GetTasksByStatus(int status)
    {
        return ConvertToEnumerable(_tasks.Filter(t => t.Status == status));
    }

    public IEnumerable<TaskItem> GetTasksByDate(DateTime date)
    {
        return ConvertToEnumerable(_tasks.Filter(t => t.CreationDate.Date == date.Date));
    }

    public TaskItem? GetTaskById(int id)
    {
        return _tasks.FindBy(id, (TaskItem a, int Id) => {
            if (a.Id == Id) return true;
            return false;
        });
    }

    public void AddTask(string description, int priority, IMyCollection<int>? assignedMembers)
    {
        int newId = _tasks.Count > 0 ? GetLastTaskId() + 1 : 1;

        TaskItem newTask = new TaskItem(assignedMembers)
        {
            Id = newId,
            Description = description,
            Status = -1,
            Priority = priority
        };

        _tasks.Add(newTask);
        SaveTasks();
    }

    public void RemoveTask(int id)
    {
        TaskItem? task = GetTaskById(id);
        if (task == null) return;

        _tasks.Remove(task);

        IMyIterator<TaskItem> iterator = _tasks.GetIterator();
        while (iterator.HasNext())
        {
            var t = iterator.Next();
            t.DependantOn.Remove(id);
        }

        SaveTasks();
    }

    public void UpdateTask(TaskItem task)
    {
        TaskItem? existing = GetTaskById(task.Id);
        if (existing != null)
        {
            _tasks.Remove(existing);
            _tasks.Add(task);
            SaveTasks();
        }
    }

    private int GetLastTaskId()
    {
        int lastId = 0;
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();
        while (iterator.HasNext())
        {
            var t = iterator.Next();
            if (t.Id > lastId) lastId = t.Id;
        }
        return lastId;
    }

    private void SaveTasks()
    {
        _repository.Save(_tasks);
    }

    private IEnumerable<TaskItem> ConvertToEnumerable(IMyCollection<TaskItem> collection)
    {
        IMyIterator<TaskItem> iterator = collection.GetIterator();
        while (iterator.HasNext())
        {
            yield return iterator.Next();
        }
    }

    public void RemoveDependency(TaskItem task, int dependencyId)
    {
        if (task == null) return;
        task.DependantOn.Remove(dependencyId);
        SaveTasks();
    }

    public void AddDependency(TaskItem task, int dependencyId)
    {
        if (task == null) return;
        if (task.Id == dependencyId) return;

        var existing = task.DependantOn.ToArray();
        foreach (var d in existing) if (d == dependencyId) return;

        TaskItem? dependency = GetTaskById(dependencyId);
        if (dependency == null) return;

        if (IsCircular(task.Id, dependency.Id)) return;

        task.DependantOn.Add(dependencyId);
        SaveTasks();
    }

    public bool IsCircular(int startTaskId, int currentTaskId, HashSet<int>? visited = null)
    {
        visited ??= new HashSet<int>();

        if (!visited.Add(currentTaskId)) return false;

        TaskItem? current = GetTaskById(currentTaskId);
        if (current == null) return false;

        var deps = current.DependantOn.ToArray();
        foreach (var depId in deps)
        {
            if (depId == startTaskId) return true;
            if (IsCircular(startTaskId, depId, visited)) return true;
        }

        return false;
    }

    public bool IsCircular(int taskId, TaskItem next, HashSet<int>? visited = null)
    {
        if (next == null) return false;
        return IsCircular(taskId, next.Id, visited);
    }

    public bool CanStartTask(TaskItem item)
    {
        if (item == null) return false;
        var deps = item.DependantOn.ToArray();
        foreach (var depId in deps)
        {
            TaskItem? dependency = GetTaskById(depId);
            if (dependency == null) return false;
            if (dependency.Status != 1) return false;
        }
        return true;
    }

    public bool ToggleTaskStatus(int id, int status)
    {
        TaskItem? task = GetTaskById(id);
        if (task == null) return false;

        if (status == 1)
        {
            if (!CanStartTask(task)) return false;
            task.Status = 1;
            SaveTasks();
            return true;
        }

        if (status == 0 || status == -1)
        {
            task.Status = status;
            SaveTasks();
            return true;
        }

        return false;
    }
}