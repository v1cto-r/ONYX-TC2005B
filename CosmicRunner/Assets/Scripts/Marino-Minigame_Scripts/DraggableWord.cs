using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MECS
{
    // Contenedor de palabra draggable con campos publicos simples
    public class DraggableWord : MonoBehaviour
    {
        // Referencia al texto que muestra la palabra en pantalla
        [SerializeField] private TMP_Text wordLabel;
        // Objeto que se mueve mientras arrastramos la palabra
        [SerializeField] private Transform dragTarget;
        // Si la palabra vuelve a su sitio al soltarla
        [SerializeField] private bool snapBackOnRelease = true;
        // Camara usada para convertir la posicion del puntero
        [SerializeField] private Camera worldCamera;
        // Si se toma el texto inicial como palabra valida
        [SerializeField] private bool initializeFromLabelText = false;

        // Accion de entrada para arrastrar
        private InputAction dragAction;
        // Accion de entrada para leer la posicion del puntero
        private InputAction pointerAction;
        // Collider propio para detectar el inicio del drag
        private Collider2D ownCollider;
        // Marca si la palabra esta siendo arrastrada
        private bool isDragging;
        // Guarda la posicion original antes de arrastrar
        private Vector3 dragStartPosition;
        // Guarda el desplazamiento entre el puntero y la palabra
        private Vector3 dragOffset;

        // Indica si este slot ya tiene una palabra
        public bool HasWord;
        // Texto actual almacenado en el slot
        public string CurrentWord = string.Empty;

        // Prepara referencias basicas y busca componentes faltantes
        private void Awake()
        {
            // Si no asignamos el texto, lo buscamos en los hijos
            if (wordLabel == null)
            {
                wordLabel = GetComponentInChildren<TMP_Text>();
            }

            // Si no hay target, usamos el propio transform
            if (dragTarget == null)
            {
                dragTarget = transform;
            }

            // Si no hay camara, usamos la principal
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            // Guardamos el collider para saber si se inicio el drag sobre esta palabra
            ownCollider = GetComponent<Collider2D>();

            // Buscamos las acciones de entrada una vez al iniciar
            ResolveActions();
        }

        // Activa la logica de drag cuando el objeto entra en escena
        private void OnEnable()
        {
            ResolveActions();

            // Si el slot debe leer el texto inicial, lo copia al estado interno
            if (initializeFromLabelText)
            {
                SyncWordFromLabelIfNeeded();
            }

            // Habilita la accion de arrastre
            if (dragAction != null)
            {
                dragAction.Enable();
            }

            // Habilita la accion del puntero
            if (pointerAction != null)
            {
                pointerAction.Enable();
            }
        }

        // Desactiva las acciones y limpia el estado de arrastre
        private void OnDisable()
        {
            // Evita que el input siga activo cuando el objeto se deshabilita
            if (dragAction != null)
            {
                dragAction.Disable();
            }

            // Igual para la lectura del puntero
            if (pointerAction != null)
            {
                pointerAction.Disable();
            }

            // Resetea el arrastre por seguridad
            isDragging = false;
        }

        // Actualiza el arrastre segun el estado actual de las entradas
        private void Update()
        {
            // Sin input o sin camara no podemos calcular el drag
            if (dragAction == null || pointerAction == null || worldCamera == null)
            {
                return;
            }

            // Convierte la posicion actual del puntero a coordenadas del mundo
            Vector3 pointerWorld = GetPointerWorldPosition();

            // Si se presiono el boton y aun no se esta arrastrando, intentamos empezar
            if (!isDragging && dragAction.WasPressedThisFrame())
            {
                TryBeginDrag(pointerWorld);
            }

            // Mientras el boton siga presionado, la palabra sigue al puntero
            if (isDragging && dragAction.IsPressed())
            {
                dragTarget.position = pointerWorld + dragOffset;
            }

            // Al soltar, cerramos el drag y evaluamos el drop
            if (isDragging && dragAction.WasReleasedThisFrame())
            {
                EndDrag();
            }
        }

        // Coloca una nueva palabra en el slot y notifica el cambio
        public void SetWord(string word)
        {
            CurrentWord = word;
            HasWord = !string.IsNullOrWhiteSpace(word);

            // Actualiza el texto visible del slot
            if (wordLabel != null)
            {
                wordLabel.text = HasWord ? word : string.Empty;
            }

            // Aviso global para refrescar contadores de palabras
            if (PromptsControl.Instance != null)
            {
                PromptsControl.Instance.NotifyWordSlotsChanged();
            }
        }

        // Limpia el slot dejando la palabra vacia
        public void ClearWord()
        {
            SetWord(string.Empty);
        }

        // Intenta iniciar el arrastre solo si el puntero realmente toco esta palabra
        private void TryBeginDrag(Vector3 pointerWorld)
        {
            // Primero validamos que el slot pueda arrastrarse
            if (!CanStartDragging(out string blockedReason))
            {
                return;
            }

            // Detecta todos los colliders que estan debajo del puntero
            Collider2D[] hits = Physics2D.OverlapPointAll(pointerWorld);
            if (hits == null || hits.Length == 0)
            {
                return;
            }

            // Confirmamos que alguno de los hits pertenezca a este objeto
            bool ownsAnyHit = false;
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i];
                bool isOwnHit = hit == ownCollider || hit.transform.IsChildOf(transform);

                if (isOwnHit)
                {
                    ownsAnyHit = true;
                }
            }

            // Si el puntero no toco esta palabra, no arrancamos el drag
            if (!ownsAnyHit)
            {
                return;
            }

            // Guardamos el punto de inicio para poder volver luego si hace falta
            isDragging = true;
            dragStartPosition = dragTarget.position;
            dragOffset = dragTarget.position - pointerWorld;
        }

        // Verifica si esta palabra puede comenzar a moverse
        private bool CanStartDragging(out string blockedReason)
        {
            // No se puede arrastrar un slot vacio
            if (!HasWord)
            {
                blockedReason = "HasWord is false (slot is empty).";
                return false;
            }

            // Necesitamos referencias validas para mover y leer el puntero
            if (dragTarget == null || ownCollider == null || worldCamera == null)
            {
                blockedReason = "dragTarget, collider, or camera is missing.";
                return false;
            }

            // Si no existe la accion, el input esta mal configurado
            if (dragAction == null)
            {
                blockedReason = $"Drag action 'Drag' was not found.";
                return false;
            }

            // Si el boton no esta presionado, no iniciamos el arrastre
            if (!dragAction.IsPressed())
            {
                blockedReason = "Drag action is not currently pressed.";
                return false;
            }

            // Todo ok: se puede arrancar el drag
            blockedReason = string.Empty;
            return true;
        }

        // Convierte la posicion del puntero desde pantalla a mundo
        private Vector3 GetPointerWorldPosition()
        {
            Vector2 pointerPosition = pointerAction.ReadValue<Vector2>();
            Vector3 screenPosition = new Vector3(pointerPosition.x, pointerPosition.y, 0f);
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            // Conservamos la misma profundidad del target para evitar saltos
            worldPosition.z = dragTarget.position.z;

            return worldPosition;
        }

        // Finaliza el arrastre y decide si la palabra vuelve a su lugar
        private void EndDrag()
        {
            isDragging = false;

            // Preguntamos si la palabra se solto sobre un hueco valido
            bool wasDroppedIntoBlank = TryDropIntoBlank();

            // Si se entrego correctamente, el slot se vacia
            if (wasDroppedIntoBlank)
            {
                ClearWord();
            }

            // Si se debe volver o si se consumo el drop, regresamos al origen
            if (snapBackOnRelease || wasDroppedIntoBlank)
            {
                dragTarget.position = dragStartPosition;
            }
        }

        // Intenta colocar la palabra en un hueco de prompts
        private bool TryDropIntoBlank()
        {
            // Sin palabra no hay nada que entregar
            if (!HasWord)
            {
                return false;
            }

            // Sin controlador global no podemos completar el drop
            if (PromptsControl.Instance == null)
            {
                return false;
            }

            // Usamos la posicion actual del target para probar el slot correcto
            Vector2 dropPoint = new Vector2(dragTarget.position.x, dragTarget.position.y);
            return PromptsControl.Instance.TryFillBlankAtWorldPoint(dropPoint, CurrentWord, this);
        }

        // Busca las acciones de input necesarias para drag y puntero
        private void ResolveActions()
        {
            // La accion Drag controla cuando el jugador toma la palabra
            if (dragAction == null)
            {
                dragAction = InputSystem.actions.FindAction("Drag");

                if (dragAction == null)
                {
                    Debug.LogWarning($"Input action 'Drag' was not found.", this);
                }
            }

            // La accion Point da la posicion exacta del puntero en pantalla
            if (pointerAction == null)
            {
                pointerAction = InputSystem.actions.FindAction("Point");

                if (pointerAction == null)
                {
                    Debug.LogWarning($"Input action 'Point' was not found. Drag will use pointer event data instead.", this);
                }
            }
        }

        // Si el texto ya existe, lo usa como palabra inicial del slot
        private void SyncWordFromLabelIfNeeded()
        {
            // Si ya tenemos palabra o falta el label, no hacemos nada
            if (wordLabel == null || HasWord)
            {
                return;
            }

            // Leemos el contenido visible y lo convertimos en estado interno
            string labelText = wordLabel.text;
            if (string.IsNullOrWhiteSpace(labelText))
            {
                return;
            }

            // Si el "label" tiene texto, lo usamos como palabra actual del slot
            CurrentWord = labelText;
            HasWord = true;
        }
    }
}
