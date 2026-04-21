using System.ComponentModel;
using System.Runtime.CompilerServices;

class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _repository;
    private readonly IMyCollection<TaskItem> _tasks;
    private readonly IMyHashMap<int, TaskItem> _taskIndex;

    public TaskService(IRepository<TaskItem> repository)
    {
        _repository = repository;
        _tasks = new MyArray<TaskItem>();
        _taskIndex = new MyHashMap<int, TaskItem>();

        IEnumerable<TaskItem> loaded = _repository.Load();
        foreach (var task in loaded)
        {
            _tasks.Add(task);
            _taskIndex.Add(task.Id, task);
        }
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
        return _taskIndex.FindBy(id);
    }

    public void AddTask(string description, int priority, IMyIndexable<int>? assignedMembers)
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
        _taskIndex.Add(newTask.Id, newTask);
        SaveTasks();
    }

    public void RemoveTask(int id)
    {
        TaskItem? task = GetTaskById(id);
        if (task != null)
        {
            _tasks.Remove(task);
            _taskIndex.Remove(id);
            SaveTasks();
        }
    }

    public void UpdateTask(TaskItem task)
    {
        TaskItem? existing = GetTaskById(task.Id);
        if (existing != null)
        {
            _tasks.Remove(existing);
            _tasks.Add(task);
            _taskIndex.Add(task.Id, task);
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
        List<TaskItem> list = new List<TaskItem>();
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();
        while (iterator.HasNext())
        {
            list.Add(iterator.Next());
        }
        _repository.Save(list);
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
        if (GetTaskById(dependencyId) == null) return;

        if (task.DependantOn.FindBy<int>(dependencyId, (t, id) => t == id) != 0)
            task.DependantOn.Remove(dependencyId);

        SaveTasks();
    }

    public void AddDependency(TaskItem task, int dependencyId)
    {
        TaskItem? dependency = GetTaskById(dependencyId);
        if (dependency == null || IsCircular(task.Id, dependency)) return;

        task.DependantOn.Add(dependencyId);
        SaveTasks();
    }

    public bool IsCircular(int taskId, TaskItem dependency, HashSet<int>? visited = null)
    {
        visited ??= new HashSet<int>();

        if (!visited.Add(dependency.Id)) return false;

        if (dependency.DependantOn.FindBy<int>(taskId, (id, key) => id == key) != 0)
            return true;

        IMyIterator<int> iterator = dependency.DependantOn.GetIterator();
        while (iterator.HasNext())
        {
            TaskItem? next = GetTaskById(iterator.Next());
            if (next == null) continue;

            if (IsCircular(taskId, next, visited))
                return true;
        }

        return false;
    }

    public bool CanStartTask(TaskItem item)
    {
        int amountDone = 0;
        IMyIterator<int> iterator = item.DependantOn.GetIterator();

        while (iterator.HasNext())
        {
            int taskId = iterator.Next();
            TaskItem? dependency = GetTaskById(taskId);
            if (dependency == null)
            {
                amountDone--;
                continue;
            }

            if (dependency.Status == 1)
            {
                amountDone++;
                continue;
            }

            amountDone--;
        }

        return amountDone >= item.DependantOn.Count;
    }

    public bool ToggleTaskStatus(int id, int status)
    {
        TaskItem? task = GetTaskById(id);

        if (task != null && status < 2)
        {
            if (CanStartTask(task))
            {
                task.Status = status;
                SaveTasks();
                return true;
            }
        }
        else if (task != null)
        {
            task.Status = -1;
            SaveTasks();
        }

        return false;
    }
}