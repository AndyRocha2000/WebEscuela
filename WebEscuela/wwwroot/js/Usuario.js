// Para el modal de agregar
function actualizarCamposPorRol() {
    const rol = document.getElementById('rolSelector').value;
    const campoCarrera = document.getElementById('campoCarrera');
    const campoTitulo = document.getElementById('campoTitulo');

    campoCarrera.classList.add('d-none');
    campoTitulo.classList.add('d-none');

    if (rol === "3") { // Docente
        campoTitulo.classList.remove('d-none');
    } else if (rol === "2") { // Alumno
        campoCarrera.classList.remove('d-none');
    }
}

// Para el modal de editar
function actualizarCamposPorRolEditar() {
    const rol = document.getElementById('editRolId').value;
    const campoCarrera = document.getElementById('editCampoCarrera');
    const campoTitulo = document.getElementById('editCampoTitulo');

    campoCarrera.classList.add('d-none');
    campoTitulo.classList.add('d-none');

    if (rol === "3") {
        campoTitulo.classList.remove('d-none');
    } else if (rol === "2") {
        campoCarrera.classList.remove('d-none');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    actualizarCamposPorRol();

    const rolEditSelect = document.getElementById('editRolId');
    if (rolEditSelect) {
        rolEditSelect.addEventListener('change', actualizarCamposPorRolEditar);
    }

    var modalEditar = document.getElementById('modalEditar');

    if (modalEditar) {
        modalEditar.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;

            var id = button.getAttribute('data-id');
            var correo = button.getAttribute('data-correo');
            var dni = button.getAttribute('data-dni');
            var rol = button.getAttribute('data-rol');
            var nombreCompleto = button.getAttribute('data-nombre-completo') || '';
            var carreraId = button.getAttribute('data-carrera-id') || '';
            var titulo = button.getAttribute('data-titulo') || '';

            modalEditar.querySelector('#editId').value = id;
            modalEditar.querySelector('#editCorreoElectronico').value = correo;
            modalEditar.querySelector('#editRolId').value = rol;
            modalEditar.querySelector('#editNombreCompleto').value = nombreCompleto;
            modalEditar.querySelector('#editContrasenia').value = '';
            modalEditar.querySelector('#editTitulo').value = titulo;
            modalEditar.querySelector('#editCarreraId').value = carreraId;

            // Mostrar/ocultar campos según el rol
            actualizarCamposPorRolEditar();
        });
    }
});
