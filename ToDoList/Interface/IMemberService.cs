public interface IMemberService {
    IEnumerable<Member> GetAllMembers();
    Member? GetMemberById(int id);
    void AddMember(string name, string password);
    void RemoveMember(int id);
    Tuple<bool, Member?> LogIn(string name, string password);
}