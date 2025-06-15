document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form[action="/AppUser/SearchUserPartial"]');
    const searchInput = form.querySelector('input[name="searchTerm"]');
    const resultsContainerId = "search-results-container";
    let resultsContainer = document.getElementById(resultsContainerId);

    if (!resultsContainer) {
        resultsContainer = document.createElement('div');
        resultsContainer.id = resultsContainerId;
        form.parentNode.insertBefore(resultsContainer, form.nextSibling);
    }

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        const searchTerm = searchInput.value;

        fetch(`/AppUser/SearchUserPartial?searchTerm=${encodeURIComponent(searchTerm)}`, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => response.text())
        .then(html => {
            resultsContainer.innerHTML = html;
        })
        .catch(error => {
            resultsContainer.innerHTML = '<div style="color:red;">Error loading results.</div>';
        });
    });
});
@using FriChat.Core.Models.AppUser
@model AppUserIndexPageViewModel
@{
    ViewData["Title"] = "Home page";
}
<link rel="stylesheet" href="~/css/index.css" />
<script src="~/js/searchUser.js"></script>

<form method="get" action="/AppUser/SearchUserPartial">
    <input type="text" name="searchTerm" />
    <button type="submit">Search</button>
</form>
<div id="search-results-container"></div>

<div class="chat-layout">
    <div class="friends-list">
        <h3>Friends</h3>
        <ul>
            @foreach (var friend in Model.FriendsList)
            {
                <li>
                    <div style="display: flex; align-items: center; gap: 0.75rem;">
                        <img src="@friend.FriendProfilePicturePath" alt="Profile" style="width:40px; height:40px; border-radius:50%; object-fit:cover;" />
                        <div>
                            <div style="font-weight: bold;">@friend.FriendUsername</div>
                            <div style="font-size: 0.9em; color: #bbb;">@friend.LastMessage</div>
                        </div>
                    </div>
                </li>
            }
        </ul>
    </div>
    <div class="chat-window">
        <div class="chat-messages">
            <!-- Chat messages will go here -->
            <div><strong>User 1:</strong> Hello!</div>
            <div><strong>You:</strong> Hi there!</div>
        </div>
        <form class="chat-input">
            <input type="text" placeholder="Type your message..." />
            <button type="submit">Send</button>
        </form>
    </div>
</div>
@model IEnumerable<FriChat.Core.Models.AppUser.AppUserSearchResultViewModel>

@if (!Model.Any())
{
    <div>No users found.</div>
}
else
{
    <ul>
        @foreach (var user in Model)
        {
            <li style="display: flex; align-items: center; gap: 0.75rem;">
                <img src="@user.ProfilePicturePath" alt="Profile" style="width:32px; height:32px; border-radius:50%; object-fit:cover;" />
                <span>@user.Username</span>
            </li>
        }
    </ul>
}
using Microsoft.AspNetCore.Mvc;
using FriChat.Core.Models.AppUser;

namespace FriChat.Controllers
{
    public class AppUserController : Controller
    {
        [HttpGet]
        public IActionResult SearchUserPartial(string searchTerm)
        {
            // Replace with your actual search logic
            var results = new List<AppUserSearchResultViewModel>
            {
                new AppUserSearchResultViewModel { Username = "Alice", ProfilePicturePath = "/images/alice.png" },
                new AppUserSearchResultViewModel { Username = "Bob", ProfilePicturePath = "/images/bob.png" }
            }.Where(u => string.IsNullOrEmpty(searchTerm) || u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            return PartialView("_SearchUserPartial", results);
        }
    }
}
namespace FriChat.Core.Models.AppUser
{
    public class AppUserSearchResultViewModel
    {
        public string Username { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
