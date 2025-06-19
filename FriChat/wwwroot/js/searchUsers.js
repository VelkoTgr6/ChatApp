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
                resultsContainer.style.display = 'block';
            })
            .catch(error => {
                resultsContainer.innerHTML = '<div style="color:red;">Error loading results.</div>';
                resultsContainer.style.display = 'block';
            });

        // Friend request button logic
    });

    // Hide resultsContainer when clicking outside
    document.addEventListener('mousedown', function (event) {
        if (
            resultsContainer.style.display === 'block' &&
            !resultsContainer.contains(event.target) &&
            !form.contains(event.target)
        ) {
            resultsContainer.style.display = 'none';
        }
    });
});