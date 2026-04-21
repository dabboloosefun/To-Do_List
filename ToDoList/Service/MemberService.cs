public class MemberService : IMemberService
{
    private readonly IRepository<Member> _repository;
    private readonly IMyCollection<Member> _Members;
    private readonly IMyHashMap<int, Member> _memberIndex;

    public MemberService(IRepository<Member> repository)
    {
        _repository = repository;
        _Members = new MyArray<Member>();
        _memberIndex = new MyHashMap<int, Member>();

        IEnumerable<Member> loaded = _repository.Load();

        foreach (var member in loaded)
        {
            _Members.Add(member);
            _memberIndex.Add(member.Id, member);
        }
    }

    public IEnumerable<Member> GetAllMembers()
    {
        return ConvertToEnumerable(_Members);
    }

    public void AddMember(string name, string password)
    {
        int newId = _Members.Count > 0 ? GetLastMemberId() + 1 : 1;
        var newMember = new Member { Id = newId, Name = name, Password = password };

        _Members.Add(newMember);
        _memberIndex.Add(newMember.Id, newMember);
        SaveMembers();
    }

    public Member? GetMemberById(int id)
    {
        return _memberIndex.FindBy(id);
    }

    public void RemoveMember(int id)
    {
        var member = GetMemberById(id);
        if (member != null)
        {
            _Members.Remove(member);
            _memberIndex.Remove(id);
            SaveMembers();
        }
    }

    public Tuple<bool, Member?> LogIn(string name, string password)
    {
        IMyIterator<Member> iterator = _Members.GetIterator();
        while (iterator.HasNext())
        {
            var member = iterator.Next();
            if (member.Name == name && member.Password == password)
            {
                return new Tuple<bool, Member?>(true, member);
            }
        }
        return new Tuple<bool, Member?>(false, null);
    }

    private int GetLastMemberId()
    {
        int lastId = 0;
        IMyIterator<Member> iterator = _Members.GetIterator();
        while (iterator.HasNext())
        {
            var m = iterator.Next();
            if (m.Id > lastId) lastId = m.Id;
        }
        return lastId;
    }

    private void SaveMembers()
    {
        List<Member> list = new List<Member>();
        IMyIterator<Member> iterator = _Members.GetIterator();
        while (iterator.HasNext())
        {
            list.Add(iterator.Next());
        }
        _repository.Save(list);
    }

    private IEnumerable<Member> ConvertToEnumerable(IMyCollection<Member> collection)
    {
        IMyIterator<Member> iterator = collection.GetIterator();
        while (iterator.HasNext())
        {
            yield return iterator.Next();
        }
    }
}