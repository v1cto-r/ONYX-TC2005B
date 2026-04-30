using TMPro;
using UnityEngine;

namespace MECS
    {
    // Slot UI para un prompt; maneja texto, estado y feedback
    [System.Serializable]
    public class PromptSlot
    {
        // Collider principal donde se escribe el prompt
        public Collider2D promptCollider;
        // Zona para enviar la respuesta
        public Collider2D submitCollider;
        // Zona para cancelar la respuesta
        public Collider2D cancelCollider;
        // Texto visible del prompt incompleto o completado
        public TMP_Text promptLabel;
        // Texto de calidad que aparece al corregir el prompt
        public TMP_Text qualityText;
        // Marca que separa la palabra que se va a reemplazar
        public string blankToken = "_____";

        // Prompt asignado a este slot
        [HideInInspector] public PromptEntry assignedPrompt;
        // Palabra que el jugador deposito en el hueco
        [HideInInspector] public string droppedWord = string.Empty;
        // Referencia al slot de palabra que entrego la palabra
        [HideInInspector] public DraggableWord sourceWord;
        // Controla si el slot ya esta lleno
        [HideInInspector] public bool isFilled;
        // Controla si el slot ya fue enviado
        [HideInInspector] public bool isSubmitted;

        // Resetea por completo el estado del slot
        public void Clear()
        {
            // Limpia los datos internos para que el slot quede vacio
            assignedPrompt = null;
            droppedWord = string.Empty;
            sourceWord = null;
            isFilled = false;
            isSubmitted = false;

            // Vaciar el texto evita que se vea un prompt viejo
            if (promptLabel != null)
            {
                promptLabel.text = string.Empty;
            }
            // Ocultamos cualquier mensaje de calidad que pudiera seguir activo
            HideQuality();
        }

        // Asigna un prompt nuevo y deja el slot listo para usarse
        public void SetPrompt(PromptEntry prompt)
        {
            // Guardamos la referencia del prompt actual
            assignedPrompt = prompt;
            droppedWord = string.Empty;
            sourceWord = null;
            isFilled = false;
            isSubmitted = false;

            // Si hay prompt, mostramos su texto inicial; si no, dejamos vacio el label
            if (promptLabel != null)
            {
                promptLabel.text = prompt != null ? prompt.incompletePrompt : string.Empty;
            }
            // Siempre ocultamos el feedback anterior al cambiar de prompt
            HideQuality();
        }

        // Momento a partir del cual la calidad debe desaparecer sola
        [HideInInspector] public float qualityHideAt = 0f;

        // Muestra el mensaje de calidad con color y tiempo de vida
        public void ShowQuality(string message, Color color, float seconds)
        {
            // Sin texto de calidad no hay nada que mostrar
            if (qualityText == null)
            {
                return;
            }

            // Aplicamos contenido, color y activamos la etiqueta
            qualityText.text = message;
            qualityText.color = color;
            qualityText.gameObject.SetActive(true);
            // Guardamos el instante en el que se ocultara
            qualityHideAt = Time.time + Mathf.Max(0.001f, seconds);
        }

        // Oculta el mensaje de calidad y reinicia su temporizador
        public void HideQuality()
        {
            qualityHideAt = 0f;
            // Si no existe el label, no hay nada mas que limpiar
            if (qualityText == null)
            {
                return;
            }

            // Dejamos el texto vacio y lo desactivamos para que no ocupe espacio visual
            qualityText.text = string.Empty;
            qualityText.gameObject.SetActive(false);
        }

        // Comprueba si el puntero esta sobre la zona del prompt
        public bool ContainsPoint(Vector2 worldPoint)
        {
            return promptCollider != null && promptCollider.OverlapPoint(worldPoint);
        }

        // Comprueba si el puntero esta sobre la zona de enviar
        public bool ContainsSubmitPoint(Vector2 worldPoint)
        {
            return submitCollider != null && submitCollider.OverlapPoint(worldPoint);
        }

        // Comprueba si el puntero esta sobre la zona de cancelar
        public bool ContainsCancelPoint(Vector2 worldPoint)
        {
            return cancelCollider != null && cancelCollider.OverlapPoint(worldPoint);
        }

        // Intenta rellenar el hueco con una palabra concreta
        public bool TryFillWithWord(string word, DraggableWord source = null)
        {
            // No podemos llenar un slot ya enviado, ya lleno, vacio o sin label
            if (isSubmitted || isFilled || string.IsNullOrWhiteSpace(word) || promptLabel == null)
            {
                return false;
            }

            // Tomamos el texto base del prompt actual para saber donde reemplazar el hueco
            string sourceText = assignedPrompt != null && !string.IsNullOrWhiteSpace(assignedPrompt.incompletePrompt)
                ? assignedPrompt.incompletePrompt
                : promptLabel.text;

            // Si el texto base esta vacio, no hay nada que completar
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                return false;
            }

            // Reemplazamos solo la primera aparicion del token de hueco
            string completedText = ReplaceFirstBlank(sourceText, blankToken, word);
            if (completedText == sourceText)
            {
                return false;
            }

            // Guardamos el resultado y marcamos el slot como lleno
            promptLabel.text = completedText;
            droppedWord = word;
            sourceWord = source;
            isFilled = true;
            return true;
        }

        // Cancela el llenado y devuelve la palabra al slot original
        public void CancelFill()
        {
            // Si no hay prompt o ya fue enviado, no se puede cancelar
            if (assignedPrompt == null || isSubmitted)
            {
                return;
            }

            // Si sabemos de donde vino la palabra, la devolvemos
            if (sourceWord != null && !string.IsNullOrWhiteSpace(droppedWord))
            {
                sourceWord.SetWord(droppedWord);
            }

            // Limpiamos el estado interno para que el slot quede libre
            droppedWord = string.Empty;
            sourceWord = null;
            isFilled = false;

            // Restauramos el texto original del prompt
            if (promptLabel != null)
            {
                promptLabel.text = assignedPrompt.incompletePrompt;
            }
        }

        // Reemplaza solo el primer token encontrado dentro del texto
        private static string ReplaceFirstBlank(string sourceText, string token, string replacement)
        {
            // Si falta texto o token, devolvemos lo recibido sin cambios
            if (string.IsNullOrEmpty(sourceText) || string.IsNullOrEmpty(token))
            {
                return sourceText;
            }

            // Buscamos la primera aparicion para no tocar mas huecos de los necesarios
            int tokenIndex = sourceText.IndexOf(token, System.StringComparison.Ordinal);
            if (tokenIndex < 0)
            {
                return sourceText;
            }

            // Reconstruimos el texto con la palabra colocada en ese punto
            return sourceText.Substring(0, tokenIndex)
                + replacement
                + sourceText.Substring(tokenIndex + token.Length);
        }
    }
}
