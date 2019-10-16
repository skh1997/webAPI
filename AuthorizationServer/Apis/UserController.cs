using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizationServer.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthorizationServer.Apis
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController: Controller
    {
        private ILogger<UserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(
            ILogger<UserController> logger, 
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var claims = User.Claims.ToList();
            var users = await _userManager.Users.ToListAsync();
            var userVms = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return Ok(userVms);
        }
    }
}
