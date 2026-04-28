async function pasteFromClipboard(event) {
  event.preventDefault();

  try {
    if (!navigator.clipboard) {
      alert("Tu navegador no soporta pegar desde el portapapeles.");
      return;
    }

    const text = await navigator.clipboard.readText();

    if (text) {
      const textarea = document.getElementById("promptContent");
      textarea.focus();

      const prefix =
        textarea.value && textarea.selectionStart === textarea.value.length
          ? "\n"
          : "";

      textarea.setRangeText(
        prefix + text,
        textarea.selectionStart,
        textarea.selectionEnd,
        "end",
      );
    }
  } catch (err) {
    console.error("Error al leer el portapapeles: ", err);
    alert("Debes dar permiso para acceder al portapapeles.");
  }
}

function readFileContent(event) {
  const fileInput = event.target;
  const file = fileInput.files[0];

  if (!file) return;

  const reader = new FileReader();
  reader.onload = function (e) {
    const text = e.target.result;
    const textarea = document.getElementById("promptContent");

    textarea.focus();
    const prefix =
      textarea.value && textarea.selectionStart === textarea.value.length
        ? "\n"
        : "";

    textarea.setRangeText(
      prefix + text,
      textarea.selectionStart,
      textarea.selectionEnd,
      "end",
    );

    fileInput.value = "";
  };

  reader.readAsText(file);
}
