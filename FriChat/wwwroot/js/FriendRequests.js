document.addEventListener('DOMContentLoaded', function () {
    const friendRequestBtn = document.getElementById('friendRequestBtn');
    const friendRequestPartialContainer = document.getElementById('friendRequestPartialContainer');

    if (friendRequestBtn && friendRequestPartialContainer) {
        friendRequestBtn.addEventListener('click', function (e) {
            e.preventDefault();

            if (friendRequestPartialContainer.style.display === 'none' || friendRequestPartialContainer.style.display === '') {
                fetch('/AppUser/GetFriendRequestsPartial', {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                })
                    .then(response => response.text())
                    .then(html => {
                        friendRequestPartialContainer.innerHTML = html;
                        friendRequestPartialContainer.style.display = 'block';
                        attachFriendRequestHandlers();
                    });
            } else {
                friendRequestPartialContainer.style.display = 'none';
            }
        });

        document.addEventListener('mousedown', function (event) {
            if (
                friendRequestPartialContainer.style.display === 'block' &&
                !friendRequestPartialContainer.contains(event.target) &&
                !friendRequestBtn.contains(event.target)
            ) {
                friendRequestPartialContainer.style.display = 'none';
            }
        });
    }

    function attachFriendRequestHandlers() {
        const forms = document.querySelectorAll('.friend-request-form');

        forms.forEach(form => {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                const action = form.getAttribute('action');
                const userId = form.dataset.userid;
                const formData = new FormData(form);

                // Grab the anti-forgery token
                const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
                if (tokenInput) {
                    formData.append('__RequestVerificationToken', tokenInput.value);
                }

                fetch(action, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest' // Optional, useful if your controller checks this
                    }
                })
                    .then(res => res.json())
                    .then(result => {
                        if (result.success) {
                            const item = document.getElementById(`friend-request-${userId}`);
                            if (item) item.remove();
                        }
                    })
                    .catch(error => {
                        console.error('Error confirming/declining friend request', error);
                    });
            });
        });
    }

});
