using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.DTO;
using TaskTracker.Models;
using TaskTracker.Repositories;
using TaskTracker.Services;


namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly IAuthenticateService _authenticateService;


        public UserController(IUserRepository userRepository,IAuthenticateService authenticateService)
        {
            _userRepository = userRepository;
            _authenticateService = authenticateService;
          
        }

         [HttpPost("register")]
         public async Task<ActionResult> CreateUser(User user)
         {
             try
             {
                 var createdUser = await _userRepository.CreateUserAsync(user);
                 return Ok(createdUser);
             }
             catch (Exception ex)
             {
                 return StatusCode(500, ex.Message);
             }
         }

        [HttpPost("login")]
      
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _userRepository.ValidateUserCredentials(loginDto.UserName, loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _authenticateService.GenerateJwtToken(user);
            

            return Ok(new { Token = token });
        }

        [HttpGet("gettasks")]
        [Authorize]
        public async Task<ActionResult> GetTasks(int id)
        {
            try

            {
                var taskdetails=await _userRepository.GetTaskDetailsAsync(id);
                return Ok(taskdetails);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    
        [HttpPost("createtasks")]
        [Authorize]
        public async Task<ActionResult> CreateTasks(TaskCreationDto taskCreation)
        {
            try
            {
                var createdTasks=await _userRepository.CreateTaskAsync(taskCreation);
                return Ok(createdTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("updatetask")]
        [Authorize]
        public async Task<ActionResult> UpdatePatchTasks([FromRoute] int id, [FromBody] JsonPatchDocument taskDocument)
        {
            var updatedTask = await _userRepository.UpdateEmployeePatchAsync(id, taskDocument);
            if (updatedTask == null)
            {
                return NotFound();
            }
            return Ok(updatedTask);
        }

        [HttpPatch("updatestatus")]
        [Authorize]
        public async Task<ActionResult> UpdateTaskStatus([FromRoute] int taskId, [FromBody] string newStatus)
        {
            try
            {
                bool updated = await _userRepository.UpdateTaskStatusAsync(taskId, newStatus);

                if (updated)
                {
                    return Ok("Task status updated successfully");
                }
                else
                {
                    return NotFound("Task not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("deletetask")]
       
        public async Task<ActionResult> DeleteTasks(int taskId)
        {
            try
            {
                bool deletedTask=await _userRepository.DeleteTaskAsync(taskId);
                if(!deletedTask) 
                {
                    return NotFound();
                }
                return Ok("Task Deleted");
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("statistics")]
       
        public async Task<ActionResult> GetTaskStatistics(int id)
        {
            try
            {
                var statistics = await _userRepository.GetTaskStatisticsAsync(id);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


       



    }
}
