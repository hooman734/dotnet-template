using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Models.Models;

namespace Api.Utilities
{
    public class HttpRequestUtility
    {
        private readonly UserManager<User> _userManager;
        
        private readonly SignInManager<User> _signInManager;

        public HttpRequestUtility(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> ResolveUser(HttpContext context)
        {
            var user = await _userManager.GetUserAsync(context.User);

            return user;
        }
        
        public bool IsAuthenticated(HttpContext context)
        {
            return _signInManager.IsSignedIn(context.User);
        }
    }
}