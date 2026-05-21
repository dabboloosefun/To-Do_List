public class MemberService : IMemberService
{
    private readonly IRepository<Member> _repository;
    private readonly IMyCollection<Member> _Members;

    public MemberService(IRepository<Member> repository)
    {
        _repository = repository;
        _Members = _repository.Load()!;
    }

    public IEnumerable<Member> GetAllMembers()
    {
        return ConvertToEnumerable(_Members);
    }

    public void AddMember(string name, string password)
    {
        int newId = _Members.Count > 0 ? GetLastMemberId() + 1 : 1;
        var newMember = new Member(newId, name, password);

        _Members.Add(newMember);
        SaveMembers();
    }

    public Member? GetMemberById(int id)
    {
        return _Members.FindBy(id, (Member a, int Id) => {
            if (a.Id == Id) return true;
            return false;
        });
    }

    public void RemoveMember(int id)
    {
        var member = GetMemberById(id);
        if (member != null)
        {
            _Members.Remove(member);
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
        _repository.Save(_Members);
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