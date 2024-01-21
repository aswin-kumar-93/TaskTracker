using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using TaskTracker.DTO;
using TaskTracker.Models;

namespace TaskTracker.Repositories
{
    public interface IUserRepository
    {

        Task<User> CreateUserAsync(User user);

        LoginDto ValidateUserCredentials(string username, string password);

        Task<TaskCreationDto> CreateTaskAsync(TaskCreationDto taskCreation);

        Task<IEnumerable<TaskDetail>> GetTaskDetailsAsync(int userId);


        Task<TaskDetail> UpdateEmployeePatchAsync(int id, JsonPatchDocument taskdetail);

        Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus);

        Task<bool> DeleteTaskAsync(int taskId);

        Task<TaskStatisticsDto> GetTaskStatisticsAsync(int userId);




    }
}
