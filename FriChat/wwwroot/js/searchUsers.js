document.addEventListener('DOMContentLoaded', function () {
    // Existing search user partial script...
    const form = document.querySelector('form[action="/AppUser/SearchUserPartial"]');
    const searchInput = form.querySelector('input[name="searchTerm"]');
    const resultsContainer = document.getElementById('searchResults');

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

        // Friend request button logic
    });

    // Friend request button logic
    const friendRequestBtn = document.getElementById('friendRequestBtn');
    const friendRequestContainer = document.getElementById('friendRequestPartialContainer');
    let friendRequestVisible = false;

    friendRequestBtn.addEventListener('click', function () {
        if (!friendRequestVisible) {
            fetch('/AppUser/FriendRequestPartial', {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => response.text())
                .then(html => {
                    friendRequestContainer.innerHTML = html;
                    friendRequestContainer.style.display = 'block';
                    friendRequestVisible = true;
                })
                .catch(error => {
                    friendRequestContainer.innerHTML = '<div style="color:red;">Error loading friend requests.</div>';
                    friendRequestContainer.style.display = 'block';
                    friendRequestVisible = true;
                });
        } else {
            friendRequestContainer.style.display = 'none';
            friendRequestVisible = false;
        }
    });

    // Hide friend request partial when clicking outside
    document.addEventListener('click', function (event) {
        if (friendRequestVisible && !friendRequestBtn.contains(event.target) && !friendRequestContainer.contains(event.target)) {
            friendRequestContainer.style.display = 'none';
            friendRequestVisible = false;
        }
    });
});