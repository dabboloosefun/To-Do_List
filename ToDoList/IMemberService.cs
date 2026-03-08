public interface IMemberService {
    IEnumerable<Member> GetAllMembers();
    void AddMember(string name, string password);
    void RemoveMember(int id);
    Tuple<bool, Member?> LogIn(string name, string password);
}