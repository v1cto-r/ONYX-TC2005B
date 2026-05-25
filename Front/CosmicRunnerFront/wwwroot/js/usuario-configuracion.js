(function () {
    const form = document.getElementById("configForm");
    if (!form) return;

    const saveModal = bootstrap.Modal.getOrCreateInstance(document.getElementById("confirmSaveModal"));
    const discardModal = bootstrap.Modal.getOrCreateInstance(document.getElementById("confirmDiscardModal"));
    const aboutModal = bootstrap.Modal.getOrCreateInstance(document.getElementById("aboutMeModal"));

    const aboutEditor = document.getElementById("aboutEditor");
    const biografiaInput = document.getElementById("Usuario_Biografia");
    const habilidadSelect = document.getElementById("habilidadSelect");
    const habilidadesInput = document.getElementById("Habilidades");
    const skillsGrid = form.querySelector(".config-skills-grid");

    const requiredFields = [
        ["#Nombre", "errorNombre"],
        ["#Apellido", "errorApellido"],
        ["#Puesto", "errorPuesto"],
        ["#Telefono", "errorTelefono"],
        ["#Correo", "errorCorreo"],
        ["#Departamento", "errorDepartamento"],
        ["#Tema", "errorTema"]
    ];

    function setError(id, message) {
        const node = document.getElementById(id);
        if (node) node.textContent = message || "";
    }

    function clearErrors() {
        requiredFields.forEach(function (item) { setError(item[1], ""); });
        setError("errorBiografia", "");
        setError("errorHabilidades", "");
    }

    function getSkills() {
        return Array.from(skillsGrid.querySelectorAll(".skill-chip"))
            .map(function (chip) { return (chip.textContent || "").trim(); })
            .filter(Boolean);
    }

    function syncSkillsHiddenInput() {
        habilidadesInput.value = getSkills().join("|");
    }

    function renderSkills(skills) {
        skillsGrid.innerHTML = "";
        skills.forEach(function (skill) {
            const chip = document.createElement("span");
            chip.className = "skill-chip";
            chip.textContent = skill;
            skillsGrid.appendChild(chip);
        });
        habilidadesInput.value = skills.join("|");
    }

    function snapshot() {
        return {
            Nombre: form.querySelector("#Nombre").value,
            Apellido: form.querySelector("#Apellido").value,
            Puesto: form.querySelector("#Puesto").value,
            Telefono: form.querySelector("#Telefono").value,
            Correo: form.querySelector("#Correo").value,
            Departamento: form.querySelector("#Departamento").value,
            Tema: form.querySelector("#Tema").value,
            Biografia: biografiaInput.value,
            Habilidades: getSkills()
        };
    }

    function restore(data) {
        form.querySelector("#Nombre").value = data.Nombre;
        form.querySelector("#Apellido").value = data.Apellido;
        form.querySelector("#Puesto").value = data.Puesto;
        form.querySelector("#Telefono").value = data.Telefono;
        form.querySelector("#Correo").value = data.Correo;
        form.querySelector("#Departamento").value = data.Departamento;
        form.querySelector("#Tema").value = data.Tema;
        biografiaInput.value = data.Biografia;
        aboutEditor.value = data.Biografia;
        renderSkills(data.Habilidades);
        clearErrors();
    }

    function validate() {
        let ok = true;
        clearErrors();

        requiredFields.forEach(function (item) {
            const input = form.querySelector(item[0]);
            if (!input || !input.value.trim()) {
                setError(item[1], "Este campo es obligatorio.");
                ok = false;
            }
        });

        if (!biografiaInput.value.trim()) {
            setError("errorBiografia", "Este campo es obligatorio.");
            ok = false;
        }

        if (getSkills().length === 0) {
            setError("errorHabilidades", "Este campo es obligatorio.");
            ok = false;
        }

        return ok;
    }

    let lastSaved = snapshot();

    document.getElementById("addSkillButton").addEventListener("click", function () {
        const selected = (habilidadSelect.value || "").trim();
        if (!selected) return;

        const skills = getSkills();
        if (!skills.includes(selected)) {
            const chip = document.createElement("span");
            chip.className = "skill-chip skill-chip-pending";
            chip.setAttribute("data-pending", "true");
            chip.textContent = selected;
            skillsGrid.appendChild(chip);
            syncSkillsHiddenInput();
        }
        habilidadSelect.value = "";
    });

    document.getElementById("saveAboutButton").addEventListener("click", function () {
        biografiaInput.value = aboutEditor.value.trim();
        if (biografiaInput.value) setError("errorBiografia", "");
        aboutModal.hide();
    });

    document.getElementById("aboutMeModal").addEventListener("show.bs.modal", function () {
        aboutEditor.value = biografiaInput.value;
    });

    document.getElementById("saveChangesButton").addEventListener("click", function () {
        if (validate()) saveModal.show();
    });

    document.getElementById("confirmSaveButton").addEventListener("click", function () {
        saveModal.hide();
        form.submit();
    });

    document.getElementById("discardChangesButton").addEventListener("click", function () {
        discardModal.show();
    });

    document.getElementById("confirmDiscardButton").addEventListener("click", function () {
        restore(lastSaved);
        discardModal.hide();
    });

    requiredFields.forEach(function (item) {
        const input = form.querySelector(item[0]);
        if (!input) return;
        input.addEventListener("input", function () {
            if (input.value.trim()) setError(item[1], "");
        });
    });

    document.getElementById("Departamento").addEventListener("change", function () {
        setError("errorDepartamento", "");
    });

    document.getElementById("Tema").addEventListener("change", function () {
        setError("errorTema", "");
    });

})();
