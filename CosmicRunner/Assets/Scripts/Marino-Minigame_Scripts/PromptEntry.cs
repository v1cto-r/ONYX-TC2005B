using UnityEngine;

namespace MECS
{
    // Calidad posible de una respuesta de prompt
    public enum PromptQuality
    {
        Meh,
        Ok,
        Good
    }

    // Datos de un prompt individual con respuestas aceptadas
    [System.Serializable]
    public class PromptEntry
    {
        // Texto con el hueco que ve el jugador
        [TextArea(2, 4)] public string incompletePrompt;
        
        // Respuestas que cuentan como excelente
        [Tooltip("Best answers for this prompt")]
        public string[] goodAnswers = new string[] { };

        // Respuestas que cuentan como aceptables
        [Tooltip("Acceptable answers for this prompt")]
        public string[] okAnswers = new string[] { };

        // Respuestas que cuentan como bajas pero validas
        [Tooltip("Mediocre answers for this prompt")]
        public string[] mehAnswers = new string[] { };
    }
}
