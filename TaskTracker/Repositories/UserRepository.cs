﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.DTO;
using TaskTracker.Models;

namespace TaskTracker.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var existingEmail = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingEmail != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var existingUsername = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (existingUsername != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }


        public LoginDto ValidateUserCredentials(string username, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user == null)
            {
                return null;
            }

            var loginDto = new LoginDto
            {

                UserName = user.UserName,
                Password = user.Password,

            };

            return loginDto;
        }



          public async Task<TaskCreationDto> CreateTaskAsync(TaskCreationDto taskCreation)
          {
              var taskDetail = new TaskDetail
              {
                  UserId =taskCreation.UserId,
                  Title = taskCreation.Title,
                  Description = taskCreation.Description,
                  DueDate = taskCreation.DueDate,
                  DueTime = taskCreation.DueTime,
              };

              await _dbContext.TaskDetails.AddAsync(taskDetail);
              await _dbContext.SaveChangesAsync();

              return taskCreation;
          }


        public async Task<IEnumerable<TaskDetail>> GetTaskDetailsAsync(int userId)
        {
            return await _dbContext.TaskDetails.Where(x => x.UserId == userId).ToListAsync();
        }


       public  async Task<TaskDetail> UpdateEmployeePatchAsync(int id, JsonPatchDocument taskdetail)
        {
            var tasks=await _dbContext.TaskDetails.FirstOrDefaultAsync(x=>x.Id == id);
   
            if (tasks == null)
            {
                return tasks;
            }
            taskdetail.ApplyTo(tasks);
            await _dbContext.SaveChangesAsync();

            return tasks;
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus)
        {
           
                var existingTask = await _dbContext.TaskDetails.FirstOrDefaultAsync(x=>x.Id==taskId);

                if (existingTask == null)
                {
                    return false; 
                }

                existingTask.Status = newStatus;

                _dbContext.TaskDetails.Update(existingTask);
                await _dbContext.SaveChangesAsync();
                return true; 
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var deletedTask= await _dbContext.TaskDetails.FirstOrDefaultAsync(x=>x.Id==taskId);
            if (deletedTask==null) 
            {
                return false;
            }
            else
            {
                _dbContext.TaskDetails.Remove(deletedTask);
                _dbContext.SaveChanges();
                return true;
            }
        }

       public async Task<TaskStatisticsDto> GetTaskStatisticsAsync(int userId)
        {
            var totalTasks = await _dbContext.TaskDetails.Where(x => x.UserId == userId).CountAsync();

            var completedTasks = await _dbContext.TaskDetails.Where(x => x.UserId == userId && x.Status == "Completed").CountAsync();

            return new TaskStatisticsDto
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,   
            };
        }

       

    }
}

