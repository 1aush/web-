namespace NetTask.Utilities
{
    public interface ITokenService
    {
        public string CreateToken(Guid userId, string userName, string userRole, Guid userDepartment);
        public Models.LoginUser ReadToken();
    }
}
