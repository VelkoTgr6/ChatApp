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
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            var model = new AppUserIndexPageViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/"),
            };

            model.UserId = await appUserService.GetUserIdAsync(User.GetId());
            model.FriendsList = await appUserService.GetFriendsListAsync(model.UserId);

            logger.LogInformation(
                "Index method called with UserId: {UserId}, ReturnUrl: {ReturnUrl}, SearchTerm: {SearchTerm}",
                model.UserId, model.ReturnUrl, model.SearchTerm);

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchUserPartial(string searchTerm)
        {
            logger.LogInformation("SearchUserPartial called with searchTerm: {searchTerm}", searchTerm);
            var userId = await appUserService.GetUserIdAsync(User.GetId());

            var model = new UserSearchFormViewModel
            {
                UserId = userId,
                SearchTerm = searchTerm,
            };

            ViewBag.hasSearched = true;

            if (userId <= 0)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View("Index", new AppUserIndexPageViewModel
                {
                    ReturnUrl = Url.Content("~/"),
                    UserId = 0,
                    FriendsList = Enumerable.Empty<FriendsFormViewModed>()
                });
            }
            var searchResults = await appUserService.SearchUsersAsync(searchTerm, userId);


            return PartialView("_SearchUserPartial", searchResults);
        }

        [HttpGet]
        public async Task<IActionResult> SendFriendRequest()
        {
            var model = new AddFriendFormModel
            {
                UserId = await appUserService.GetUserIdAsync(User.GetId())
            };
            return View(model);
        }

        //add model error handling 
        // ||
        // VV

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int friendId)
        {
            if (friendId <= 0)
            {
                return BadRequest("Invalid friend ID.");
            }
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                return NotFound("User not found.");
            }
            var result = await appUserService.AddFriendRequestToUserAsync(userId, friendId);
            if (result > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Failed to add friend.");
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendRequestsPartial()
        {
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                return NotFound("User not found.");
            }
            var friendRequests = await appUserService.GetUserFriendRequestsAsync(userId);
            return PartialView("_FriendRequestsPartial", friendRequests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmFriendRequest(int friendId)
        {
            if (friendId <= 0)
            {
                return BadRequest("Invalid friend ID.");
            }
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                return NotFound("User not found.");
            }
            var result = await appUserService.ConfirmFriendRequestAsync(userId, friendId);
            if (result > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Failed to confirm friend request.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineFriendRequest(int friendId)
        {
            if (friendId <= 0)
            {
                return BadRequest("Invalid friend ID.");
            }
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                return NotFound("User not found.");
            }
            var result = await appUserService.DeclineFriendRequestAsync(userId, friendId);
            if (result > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Failed to reject friend request.");
        }
    }
}
