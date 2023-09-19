using API.DTOs;
using API.Entities;
using API.Helpers;
using System.Collections;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        
        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);
        
        Task<AppUser> GetUserBYUsernameAsync(string username);

        Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams);

        Task<MemberDto> GetMemberBYUsernameAsync(string username);

    }
}
