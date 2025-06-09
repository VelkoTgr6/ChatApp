using FriChat.Core.Contracts;
using FriChat.Core.Models.AppUser;
using FriChat.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FriChat.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly IRepository repository;
        private readonly IAppUserService appUserService;

        public IndexModel(
            ILogger<IndexModel> _logger,
            IRepository _repository,
            IAppUserService _appUserService)
        {
            logger = _logger;
            repository = _repository;
            appUserService = _appUserService;
        }

        [BindProperty]
        public string ReturnUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
        public IEnumerable<FriendsFormViewModed> FriendsList { get; set; } = Enumerable.Empty<FriendsFormViewModed>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public IEnumerable<UserSearchFormViewModel> SearchResults { get; set; } = Enumerable.Empty<UserSearchFormViewModel>();
        public async Task<int> GetUserIdAsync()
        {
            return await repository.GetUserIdByIdentityIdAsync(User.GetId().ToString());
        }

        public async Task OnGetAsync(string returnUrl)
        {
            ReturnUrl = returnUrl ??= Url.Content("~/");
            UserId = await GetUserIdAsync();
            FriendsList = await appUserService.GetFriendsListAsync(UserId);
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchResults = await appUserService.SearchUsersAsync(SearchTerm,UserId);
            }
            else
            {
                SearchResults = Enumerable.Empty<UserSearchFormViewModel>();
            }
        }

        public IActionResult OnPost(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Your post logic here
            return RedirectToPage("./Index", new { returnUrl });
        }
    }
}
