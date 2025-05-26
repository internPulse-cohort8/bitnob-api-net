using InternPulse4.Models;
using InternPulse4.Models.UserModel;

namespace InternPulse4.Core.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponse>> GetUser(int id);
        Task<BaseResponse<ICollection<UserResponse>>> GetAllUsers();
        Task<BaseResponse> RemoveUser(int id);
        Task<BaseResponse> UpdateUser(int id, UserRequest request);
        Task<BaseResponse<UserResponse>> Login(UserRequest.LoginRequestModel model);
        Task<BaseResponse<UserResponse>> CreateUser(UserRequest request);
    }
}
