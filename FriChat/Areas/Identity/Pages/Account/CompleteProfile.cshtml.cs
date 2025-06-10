using FriChat.Infrastructure.Data.Common;
using FriChat.Infrastructure.Data.Models;
using FriChat.Infrastructure.Enums;
using FriChat.Infrastructure.Services.CloudinaryServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FriChat.Areas.Identity.Pages.Account
{
    public class CompleteProfileModel : PageModel
    {
        private readonly IRepository repository;
        private readonly ILogger<CompleteProfileModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ICloudinary cloudinary;

        public CompleteProfileModel(
            IRepository _repository,
            ILogger<CompleteProfileModel> _logger,
            UserManager<IdentityUser> _userManager,
            ICloudinary _cloudinary
            )
        {
            repository = _repository;
            logger = _logger;
            userManager = _userManager;
            cloudinary = _cloudinary;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "First name is required.")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Profile Picture")]
            public IFormFile? ProfilePictureFile { get; set; }

            //[Display(Name = "Profile Picture")]
            //[DataType(DataType.ImageUrl)]
            //[Url(ErrorMessage = "Please enter a valid URL for the profile picture.")]
            //public string? ProfilePictureUrl { get; set; }

            [Required(ErrorMessage = "Date of Birth is required.")]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "Gender is required")]
            public Gender Gender { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ??= Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = userManager.GetUserId(User);

            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            var user = await userManager.FindByIdAsync(userId);

            string profilePictureUrl ="images/profiles/default.jpg";

            if (Input.ProfilePictureFile != null && Input.ProfilePictureFile.Length > 0)
            {
                var uploadedUrl = await cloudinary.UploadImageAsync(Input.ProfilePictureFile);
                if (!string.IsNullOrEmpty(uploadedUrl))
                    profilePictureUrl = uploadedUrl; 
            }

            if (await repository.UsernameExistAsync(Input.Username))
            {
                ModelState.AddModelError(string.Empty, "Username already taken.");
                return Page();
            }

            if (await repository.EmailExistAsync(user.Email))
            {
                ModelState.AddModelError(string.Empty, "Email already exist.");
                return Page();
            }

            var appUser = new AppUser
            {
                UserName = Input.Username,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                DateOfBirth = DateTime.SpecifyKind(Input.DateOfBirth, DateTimeKind.Utc),
                Gender = Input.Gender,
                Email = user.Email,
                IdentityUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                ProfilePicturePath = profilePictureUrl ?? "images/profiles/default.jpg", // Default profile picture if not provided
            };
            await userManager.AddToRoleAsync(user, "User");

            await repository.AddAsync(appUser); // Save changes to the database
            await repository.SaveChangesAsync();

            // Redirect to the return URL or home page
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
    }
}
