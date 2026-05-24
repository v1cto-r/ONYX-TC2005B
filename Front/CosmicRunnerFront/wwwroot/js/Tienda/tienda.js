document.addEventListener("DOMContentLoaded", function () {
    const detalleModalElement = document.getElementById("detalleProductoModal");
    const confirmarModalElement = document.getElementById("confirmarCompraModal");
    const modalProductoImagen = document.getElementById("modalProductoImagen");
    const modalProductoCategoria = document.getElementById("modalProductoCategoria");
    const modalProductoNombre = document.getElementById("modalProductoNombre");
    const modalProductoDescripcion = document.getElementById("modalProductoDescripcion");
    const modalProductoPrecio = document.getElementById("modalProductoPrecio");
    const modalCreditosActuales = document.getElementById("modalCreditosActuales");
    const modalCreditosRestantes = document.getElementById("modalCreditosRestantes");
    const modalCompraMensaje = document.getElementById("modalCompraMensaje");
    const btnAbrirConfirmacion = document.getElementById("btnAbrirConfirmacion");
    const confirmProductoNombre = document.getElementById("confirmProductoNombre");
    const confirmProductoPrecio = document.getElementById("confirmProductoPrecio");
    const confirmProductoId = document.getElementById("confirmProductoId");
    const mensajeTiendaModalElement = document.getElementById("mensajeTiendaModal");

    let productoSeleccionado = null;

    // Abrimos el pop up final
    if (mensajeTiendaModalElement) {
        const mensajeModal = new bootstrap.Modal(mensajeTiendaModalElement);
        mensajeModal.show();
    }

    if (!detalleModalElement || !confirmarModalElement) {
        return;
    }

    function formatearCreditos(cantidad) {
        return new Intl.NumberFormat("es-MX").format(cantidad) + " créditos";
    }

    detalleModalElement.addEventListener("show.bs.modal", function (event) {
        const boton = event.relatedTarget;

        if (!boton) {
            return;
        }

        const precio = parseInt(boton.getAttribute("data-producto-precio"));
        const creditos = parseInt(boton.getAttribute("data-creditos"));
        const creditosRestantes = creditos - precio;

        // Guardamos el producto seleccionado
        productoSeleccionado = {
            id: boton.getAttribute("data-producto-id"),
            nombre: boton.getAttribute("data-producto-nombre"),
            descripcion: boton.getAttribute("data-producto-descripcion"),
            categoria: boton.getAttribute("data-producto-categoria"),
            precio: precio,
            imagen: boton.getAttribute("data-producto-imagen"),
            creditos: creditos,
            creditosRestantes: creditosRestantes
        };

        // Llenamos la informacion del producto
        modalProductoImagen.src = productoSeleccionado.imagen;
        modalProductoImagen.alt = productoSeleccionado.nombre;
        modalProductoCategoria.textContent = productoSeleccionado.categoria;
        modalProductoNombre.textContent = productoSeleccionado.nombre;
        modalProductoDescripcion.textContent = productoSeleccionado.descripcion;
        modalProductoPrecio.textContent = formatearCreditos(productoSeleccionado.precio);
        modalCreditosActuales.textContent = formatearCreditos(productoSeleccionado.creditos);
        modalCreditosRestantes.textContent = formatearCreditos(productoSeleccionado.creditosRestantes);

        // Si no tiene el dinero no deja comprar y muestra un mensaje
        if (productoSeleccionado.creditosRestantes < 0) {
            modalCompraMensaje.classList.remove("d-none");
            btnAbrirConfirmacion.disabled = true;
        } else {
            modalCompraMensaje.classList.add("d-none");
            btnAbrirConfirmacion.disabled = false;
        }
    });

    btnAbrirConfirmacion.addEventListener("click", function () {
        if (!productoSeleccionado) {
            return;
        }

        // Mandamos los datos de confirmacion
        confirmProductoNombre.textContent = productoSeleccionado.nombre;
        confirmProductoPrecio.textContent = formatearCreditos(productoSeleccionado.precio);
        confirmProductoId.value = productoSeleccionado.id;
        const detalleModal = bootstrap.Modal.getInstance(detalleModalElement);
        detalleModal.hide();

        setTimeout(function () {
            const confirmarModal = new bootstrap.Modal(confirmarModalElement);
            confirmarModal.show();
        }, 250);
    });
});