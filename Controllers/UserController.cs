using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.Models;
using PennyMonster.Services;
using System.Security.Claims;

namespace PennyMonster.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            var result = await userService.SyncUserAsync();
            return Ok(result);
        }
    }
}