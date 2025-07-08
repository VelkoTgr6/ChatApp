using FriChat.Core.Contracts;
using FriChat.Core.Models.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FriChat.Infrastructure.Enums;

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
            model.FriendRequestsCount = await appUserService.GetUserFriendRequestsCount(model.UserId);

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
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, error = "Invalid friend ID." });
                return BadRequest("Invalid friend ID.");
            }
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, error = "User not found." });
                return NotFound("User not found.");
            }
            var result = await appUserService.ConfirmFriendRequestAsync(userId, friendId);
            if (result > 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });
                return RedirectToAction(nameof(Index));
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Failed to confirm friend request." });
            return BadRequest("Failed to confirm friend request.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineFriendRequest(int friendId)
        {
            if (friendId <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, error = "Invalid friend ID." });
                return BadRequest("Invalid friend ID.");
            }
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, error = "User not found." });
                return NotFound("User not found.");
            }
            var result = await appUserService.DeclineFriendRequestAsync(userId, friendId);
            if (result > 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });
                return RedirectToAction(nameof(Index));
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Failed to reject friend request." });
            return BadRequest("Failed to reject friend request.");
        }

        [HttpGet]
        public async Task<IActionResult> GetConversationPartial(int friendId)
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
            var conversationId = await appUserService.GetConversationIdAsync(userId, friendId);

            var conversation = await appUserService.GetConversationAsync(userId, friendId, conversationId);

            if (conversation == null)
            {
                return NotFound("Conversation not found.");
            }

            return PartialView("_UsersConversationPartial", conversation);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMessagesForConversationPartial(int friendId, int conversationId)
        {
            if (friendId <= 0 || conversationId <= 0)
            {
                return BadRequest("Invalid friend ID or conversation ID.");
            }
            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
            {
                return NotFound("User not found.");
            }
            var messages = await appUserService.GetUserMessagesForConversationAsync(userId, friendId, conversationId);
            return PartialView("_MessagesPartial", messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int friendId, IFormFile? messageContent, string? textContent, int conversationId, MessageType messageType)
        {
            if (friendId <= 0 || conversationId <= 0)
                return Json(new { success = false, error = "Invalid friend or conversation ID." });

            var userId = await appUserService.GetUserIdAsync(User.GetId());
            if (userId <= 0)
                return Json(new { success = false, error = "User not found." });

            int result = 0;

            if (messageType == MessageType.Text || messageType == MessageType.Link)
            {
                if (string.IsNullOrWhiteSpace(textContent))
                    return Json(new { success = false, error = "Message content is required." });

                // Convert text to a stream and wrap as IFormFile for compatibility
                var bytes = System.Text.Encoding.UTF8.GetBytes(textContent);
                using var stream = new MemoryStream(bytes);
                var formFile = new FormFile(stream, 0, bytes.Length, "textContent", "message.txt");
                result = await appUserService.CreateMessageAsync(userId, friendId, formFile, messageType, conversationId);
            }
            else
            {
                if (messageContent == null || messageContent.Length == 0)
                    return Json(new { success = false, error = "File is required for this message type." });

                result = await appUserService.CreateMessageAsync(userId, friendId, messageContent, messageType, conversationId);
            }

            var messages = await appUserService.GetUserMessagesForConversationAsync(userId, friendId, conversationId);

            if (result > 0)
            {
                var html = await RenderViewAsync("_MessagesPartial", messages, true);
                return Json(new { success = true, html });
            }

            return Json(new { success = false, error = "Failed to create message." });
        }

        private async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = false)
        {
            var actionContext = this.ControllerContext;
            var serviceProvider = HttpContext.RequestServices;
            var viewEngine = (ICompositeViewEngine)serviceProvider.GetService(typeof(ICompositeViewEngine));
            var tempDataProvider = (ITempDataProvider)serviceProvider.GetService(typeof(ITempDataProvider));
            var viewResult = viewEngine.FindView(actionContext, viewName, !partial);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"View '{viewName}' not found.");
            }

            var viewDictionary = new ViewDataDictionary<TModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: ModelState)
            {
                Model = model
            };

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
