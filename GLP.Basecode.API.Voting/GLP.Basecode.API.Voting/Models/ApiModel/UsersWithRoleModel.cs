namespace GLP.Basecode.API.Voting.Models.CustomModel
{
    public class UsersWithRoleModel
    {
        public short RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public long UserId { get; set; }
    }
}
