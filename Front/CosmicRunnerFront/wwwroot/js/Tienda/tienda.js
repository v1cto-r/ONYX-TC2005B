document.addEventListener("DOMContentLoaded", function () {
    const detalleModalElement = document.getElementById("detalleProductoModal");
    const confirmarModalElement = document.getElementById("confirmarCompraModal");
    const modalProductoCarouselInner = document.getElementById("modalProductoCarouselInner");
    const modalProductoCarousel = document.getElementById("modalProductoCarousel");
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
    let carouselInstance = null;

    // Abrimos el pop up final
    if (mensajeTiendaModalElement) {
        const mensajeModal = new bootstrap.Modal(mensajeTiendaModalElement);
        mensajeModal.show();
    }

    if (!detalleModalElement || !confirmarModalElement) {
        return;
    }

    function formatearCreditos(cantidad) {
        return new Intl.NumberFormat("en-US").format(cantidad) + " créditos";
    }

    function obtenerImagenesProducto(boton) {
        const imagenesTexto = boton.getAttribute("data-producto-imagenes");
        const imagenPrincipal = boton.getAttribute("data-producto-imagen");

        const imagenes = (imagenesTexto || imagenPrincipal || "")
            .split("|")
            .map(imagen => imagen.trim())
            .filter(imagen => imagen !== "");

        if (imagenes.length === 0 && imagenPrincipal) {
            imagenes.push(imagenPrincipal);
        }

        return imagenes;
    }

    function llenarCarrusel(imagenes, nombreProducto) {
        modalProductoCarouselInner.innerHTML = "";

        imagenes.forEach(function (imagen, index) {
            const item = document.createElement("div");
            item.className = index === 0 ? "carousel-item active" : "carousel-item";

            const img = document.createElement("img");
            img.src = imagen;
            img.alt = nombreProducto;

            item.appendChild(img);
            modalProductoCarouselInner.appendChild(item);
        });

        const controlesCarousel = modalProductoCarousel.querySelectorAll(".carousel-control-prev, .carousel-control-next");
        const mostrarControles = imagenes.length > 1;

        controlesCarousel.forEach(function (control) {
            control.classList.toggle("d-none", !mostrarControles);
        });

        if (carouselInstance) {
            carouselInstance.dispose();
        }

        carouselInstance = new bootstrap.Carousel(modalProductoCarousel, {
            interval: false,
            ride: false,
            touch: true
        });

        carouselInstance.to(0);
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
            imagenes: obtenerImagenesProducto(boton),
            creditos: creditos,
            creditosRestantes: creditosRestantes
        };

        // Llenamos las imagenes del producto en el carrusel
        llenarCarrusel(productoSeleccionado.imagenes, productoSeleccionado.nombre);

        // Llenamos la informacion del producto
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