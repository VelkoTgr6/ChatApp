using FriChat.Core.Contracts;
using FriChat.Core.Models.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FriChat.Controllers
{
    [Authorize(Roles = "User")]
    public class AppUserController : Controller
    {
        private readonly ILogger<AppUserController> logger;
        private readonly IAppUserService appUserService;

        public AppUserController(
            ILogger<AppUserController> _logger,
            IAppUserService _appUserService)
        {
            logger = _logger;
            appUserService = _appUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl = null, string searchTerm = null)
        {
            var model = new AppUserIndexPageViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/"),
                SearchTerm = searchTerm
            };

            model.UserId = await appUserService.GetUserIdAsync(User.GetId());
            model.FriendsList = await appUserService.GetFriendsListAsync(model.UserId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model.SearchResults = await appUserService.SearchUsersAsync(searchTerm, model.UserId);
                searchTerm = string.Empty;
            }
            else
            {
                model.SearchResults = Enumerable.Empty<UserSearchFormViewModel>();
            }

            return View(model);
        }
    }
}
