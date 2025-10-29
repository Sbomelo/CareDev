document.addEventListener('DOMContentLoaded', () => {
    console.log('confirmModal: initialized');

    const modalEl = document.getElementById('confirmModal');
    const modal = new bootstrap.Modal(modalEl);
    const titleEl = document.getElementById('confirmModalTitle');
    const messageEl = document.getElementById('confirmModalMessage');
    const confirmBtn = document.getElementById('confirmModalConfirmBtn');

    document.querySelectorAll('form[data-confirm-on-submit="true"]').forEach(form => {
        form.addEventListener('submit', e => {
            e.preventDefault();

            const title = form.dataset.confirmTitle || 'Confirm';
            const message = form.dataset.confirmMessage || 'Are you sure?';
            const btnText = form.dataset.confirmText || 'Confirm';
            const btnClass = form.dataset.confirmClass || 'btn-primary';

            // Reset previous color classes
            confirmBtn.className = 'btn rounded-pill px-4';
            // Apply new color
            confirmBtn.classList.add(btnClass);

            titleEl.textContent = title;
            messageEl.textContent = message;
            confirmBtn.textContent = btnText;

            // Show modal
            modal.show();

            // Confirm event handler
            const handler = () => {
                modal.hide();
                form.removeEventListener('submit', e => e.preventDefault());
                confirmBtn.removeEventListener('click', handler);
                form.submit();
            };

            confirmBtn.addEventListener('click', handler);
        });
    });
});
