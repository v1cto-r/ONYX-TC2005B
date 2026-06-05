document.addEventListener("DOMContentLoaded", function () {
    const mensajeTiendaModal = document.getElementById("mensajeTiendaModal");

    // Modal resultado
    if (mensajeTiendaModal) {
        const modal = new bootstrap.Modal(mensajeTiendaModal);
        modal.show();
    }
});