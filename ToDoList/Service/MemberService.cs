public class MemberService : IMemberService {
    private readonly IRepository<Member> _repository;
    private readonly List<Member> _Members;
    public MemberService(IRepository<Member> repository) {
        _repository = repository;
        _Members = _repository.Load();
    }
    public IEnumerable<Member> GetAllMembers() => _Members;
    public void AddMember(string name, string password) {
        int newId = _Members.Count > 0 ? _Members[_Members.Count - 1].Id + 1 :
        1;
        var newMember = new Member { Id = newId, Name =
        name, Password = password};
        _Members.Add(newMember);
        _repository.Save(_Members);
    }

    public Member? GetMemberById(int id)
    {
        foreach (Member m in _Members)
        {
            if (m.Id == id)
            {
                return m;
            }
        }
        return null;
    }
    public void RemoveMember(int id) {
        var Member = _Members.Find(m => m.Id == id);
        if (Member != null) {
            _Members.Remove(Member);
            _repository.Save(_Members);
        }
    }
    public Tuple<bool, Member?> LogIn(string name, string password) {
        Console.WriteLine(_Members);
        var Member = _Members.Find(m => m.Name == name);
        Console.WriteLine(Member);
        if (Member != null && Member.Password == password) {
            return new Tuple<bool, Member?>(true, Member);
        }
        return new Tuple<bool, Member?>(false, null);
    }
} 