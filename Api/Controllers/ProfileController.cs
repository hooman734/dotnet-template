using System.Net.Sockets;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserLogic _userLogic;

        public ProfileController(UserManager<User> userManager, IUserLogic userLogic)
        {
            _userManager = userManager;
            _userLogic = userLogic;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            return View("index", user);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> ProfileHandler(User dto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            await _userLogic.Update(user.Id, dto);
            
            return RedirectToAction("GetProfile");
        }
    }
}