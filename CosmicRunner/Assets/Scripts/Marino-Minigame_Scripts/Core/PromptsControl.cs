using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace MECS
{
    // Controla prompts, palabras y feedback de calidad
    public class PromptsControl : MonoBehaviour
    {
        // Instancia global para que otros sistemas hablen con los prompts
        public static PromptsControl Instance;

        [Header("Word Configuration")]
        // Lista de palabras que pueden salir en las cajas
        [SerializeField] private string[] boxWords;
        // Slots visuales donde se guardan palabras recogidas
        [SerializeField] private DraggableWord[] wordSlots;

        [HideInInspector] public int currentWordCount;
        [HideInInspector] public int wordStorageCapacity;

        [Header("Prompt Configuration")]
        // Banco completo de prompts disponibles
        [SerializeField] private PromptEntry[] promptPool;
        // Slots activos en la interfaz de prompts
        [SerializeField] private PromptSlot[] promptSlots;
        
        [Header("Scoring Configuration")]
        // Puntos que otorga una respuesta mediocre
        [SerializeField] private int mehPoints = 5;
        // Puntos que otorga una respuesta correcta normal
        [SerializeField] private int okPoints = 10;
        // Puntos que otorga una respuesta excelente
        [SerializeField] private int goodPoints = 15;

        [Header("Feedback UI")]
        // Tiempo que el mensaje de calidad permanece visible
        [SerializeField] private float qualityDisplaySeconds = 5f;

        [Header("Testing Settings")]
        // Si se limpian los slots de palabras al iniciar
        [SerializeField] private bool clearSlotsOnStart = true;
        // Si se asignan prompts aleatorios al iniciar
        [SerializeField] private bool assignRandomPromptsOnStart = true;

        [Header("API Settings")]
        // Base del API del minijuego
        [SerializeField] private string apiBaseUrl = "https://localhost:8443/minigame";

        [Header("Input Settings")]
        // Accion que confirma clicks o disparos sobre la UI
        [SerializeField] private string clickActionName = "Drag";
        // Accion que da la posicion del puntero
        [SerializeField] private string pointerActionName = "Point";
        // Camara usada para traducir pantalla a mundo
        [SerializeField] private Camera worldCamera;

        // Entrada de click del sistema nuevo
        private InputAction clickAction;
        // Entrada de posicion del puntero
        private InputAction pointerAction;
        // Servicio simple para traer datos del minijuego
        private MinigameApiService apiService;
        

        // Prepara singleton, referencias y estado inicial
        private void Awake()
        {
            // Evita duplicados del controlador global
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Guardamos la instancia activa
            Instance = this;

            // El limite de almacenamiento depende de cuantos slots existan
            ResolveWordSlots();

            // Si no asignamos camara, usamos la principal
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            apiService = new MinigameApiService(apiBaseUrl);

            // Al iniciar, ocultamos cualquier texto de calidad que ya estuviera activo
            if (promptSlots != null && promptSlots.Length > 0)
            {
                for (int i = 0; i < promptSlots.Length; i++)
                {
                    TMP_Text qt = promptSlots[i] != null ? promptSlots[i].qualityText : null;
                    if (qt != null)
                    {
                        qt.gameObject.SetActive(false);
                    }
                }
            }

            // Resolvemos input y dejamos el estado de juego listo
            ResolveInputActions();
        }

        // Si no se asignaron slots manualmente, los buscamos en la escena
        private void ResolveWordSlots()
        {
            if (wordSlots != null && wordSlots.Length > 0)
            {
                wordStorageCapacity = wordSlots.Length;
                return;
            }

            wordSlots = FindObjectsByType<DraggableWord>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            wordStorageCapacity = wordSlots != null ? wordSlots.Length : 0;
        }

        // Cargamos datos de API antes de preparar el estado inicial
        private IEnumerator Start()
        {
            if (apiService != null)
            {
                List<string> loadedWords = null;
                List<PromptEntry> loadedPrompts = null;

                yield return apiService.GetWords(words => loadedWords = words);
                yield return apiService.GetPrompts(prompts => loadedPrompts = prompts);

                if (loadedWords == null || loadedWords.Count == 0)
                {
                    Debug.LogError("API did not return any usable words. PromptsControl will not initialize.", this);
                    yield break;
                }

                if (loadedPrompts == null || loadedPrompts.Count == 0)
                {
                    Debug.LogError("API did not return any usable prompts. PromptsControl will not initialize.", this);
                    yield break;
                }

                boxWords = loadedWords != null ? loadedWords.ToArray() : new string[0];
                promptPool = loadedPrompts != null ? loadedPrompts.ToArray() : new PromptEntry[0];
                InitializeGameplayState();
                yield break;
            }

            Debug.LogError("PromptsControl could not start because the API service is missing.", this);
        }

        // Enciende las acciones de entrada cuando el objeto esta activo
        private void OnEnable()
        {
            ResolveInputActions();

            // Activamos la accion de click si existe
            if (clickAction != null)
            {
                clickAction.Enable();
            }

            // Activamos la accion del puntero si existe
            if (pointerAction != null)
            {
                pointerAction.Enable();
            }
        }

        // Desactiva las acciones para no dejar input vivo
        private void OnDisable()
        {
            if (clickAction != null)
            {
                clickAction.Disable();
            }

            if (pointerAction != null)
            {
                pointerAction.Disable();
            }
        }

        // Revisa timers de feedback y clicks sobre prompts
        private void Update()
        {
            // Sin input, camara o puntero no se puede procesar nada
            if (clickAction == null || pointerAction == null || worldCamera == null)
            {
                return;
            }

            // Cada frame comprobamos si algun mensaje de calidad debe ocultarse
            UpdateQualityTimers();

            // Solo seguimos si hubo un click real este frame
            if (!clickAction.WasPressedThisFrame())
            {
                return;
            }

            // Convertimos la posicion del puntero a coordenadas del mundo
            Vector2 pointerScreen = pointerAction.ReadValue<Vector2>();
            Vector3 pointerWorld3 = worldCamera.ScreenToWorldPoint(new Vector3(pointerScreen.x, pointerScreen.y, 0f));
            Vector2 pointerWorld = new Vector2(pointerWorld3.x, pointerWorld3.y);

            // Enviamos el click a la logica de enviar o cancelar prompts
            HandlePromptActionClick(pointerWorld);
        }

        // Oculta mensajes de calidad cuando ya paso su tiempo
        private void UpdateQualityTimers()
        {
            // Sin slots no hay nada que revisar
            if (promptSlots == null)
            {
                return;
            }

            // Revisamos slot por slot para ocultar solo el que expire
            float now = Time.time;
            for (int i = 0; i < promptSlots.Length; i++)
            {
                PromptSlot slot = promptSlots[i];
                if (slot == null)
                {
                    continue;
                }

                // Si llego el momento, ocultamos el texto de calidad de ese slot
                if (slot.qualityHideAt > 0f && now >= slot.qualityHideAt)
                {
                    slot.HideQuality();
                }
            }
        }

        // Procesa la recogida de una caja y la convierte en una palabra util
        public void HandleBoxCollected()
        {
            // Si no hay palabras configuradas, no podemos generar ninguna recompensa
            if (boxWords == null || boxWords.Length == 0)
            {
                Debug.LogWarning("No words configured in PromptsControl.", this);
                return;
            }

            // Si no hay slots, no hay donde guardar la palabra
            if (wordSlots == null || wordSlots.Length == 0)
            {
                Debug.LogWarning("No word slots configured in PromptsControl.", this);
                return;
            }

            // Elegimos una palabra aleatoria del banco configurado
            string randomWord = boxWords[Random.Range(0, boxWords.Length)];
            bool wasAssigned = AssignToFirstEmptySlot(randomWord);

            // Si ya no quedan slots libres, informamos que la palabra se ignoro
            if (!wasAssigned)
            {
                Debug.Log("All word slots are full. Collected word was ignored.", this);
            }

            // Recalculamos el contador de palabras visibles
            RefreshWordCount();
        }

        // Limpia todos los slots de palabras para reiniciar la partida
        public void ClearAllWordSlots()
        {
            if (wordSlots == null)
            {
                return;
            }

            // Limpiamos cada slot individualmente
            foreach (DraggableWord slot in wordSlots)
            {
                if (slot == null)
                {
                    continue;
                }

                slot.ClearWord();
            }

            // Actualizamos el contador global despues de vaciar todo
            RefreshWordCount();
        }

        // Cancela los prompts que fueron llenados pero no enviados
        public void CancelAllUnsubmittedPromptFills()
        {
            // Si no hay slots de prompts no hay nada que revisar
            if (promptSlots == null)
            {
                return;
            }

            // Solo cancelamos los que estan llenos y aun no se enviaron
            foreach (PromptSlot slot in promptSlots)
            {
                if (slot == null || !slot.isFilled || slot.isSubmitted)
                {
                    continue;
                }

                slot.CancelFill();
            }
        }

        // Oculta todos los mensajes de calidad visibles
        public void HideAllQualityTexts()
        {
            if (promptSlots == null)
            {
                return;
            }

            // Recorremos cada slot para limpiar su feedback visual
            foreach (PromptSlot slot in promptSlots)
            {
                if (slot == null)
                {
                    continue;
                }

                slot.HideQuality();
            }
        }

        // Notifica a la UI que cambio el estado de las palabras
        public void NotifyWordSlotsChanged()
        {
            RefreshWordCount();
        }

        // Deja el juego listo para empezar con el estado inicial configurado
        private void InitializeGameplayState()
        {
            // Opcionalmente vaciamos las palabras del jugador
            if (clearSlotsOnStart)
            {
                ClearAllWordSlots();
            }

            // Opcionalmente asignamos prompts al azar al cargar la escena
            if (assignRandomPromptsOnStart)
            {
                ClearAllPromptSlots();
                AssignRandomPromptsToSlots();
            }

            // Sincronizamos el contador de palabras al terminar
            RefreshWordCount();
        }

        // Intenta rellenar un hueco de prompt segun la posicion del puntero
        public bool TryFillBlankAtWorldPoint(Vector2 worldPoint, string word, DraggableWord source = null)
        {
            // Si no hay slots de prompt, no se puede rellenar nada
            if (promptSlots == null || promptSlots.Length == 0)
            {
                return false;
            }

            // Buscamos el slot que contiene el punto del puntero
            foreach (PromptSlot slot in promptSlots)
            {
                if (slot == null || !slot.ContainsPoint(worldPoint))
                {
                    continue;
                }

                // Si el slot acepta la palabra, terminamos aqui
                if (slot.TryFillWithWord(word, source))
                {
                    return true;
                }
            }

            return false;
        }

        // Evalua un prompt ya llenado y aplica score y feedback
        public void SubmitPrompt(int slotIndex)
        {
            // Primero aseguramos que el indice exista
            if (!IsValidSlotIndex(slotIndex))
            {
                return;
            }

            // Si el slot no esta listo, no hacemos nada
            PromptSlot slot = promptSlots[slotIndex];
            if (slot == null || slot.assignedPrompt == null || slot.isSubmitted || !slot.isFilled)
            {
                return;
            }

            // Normalizamos la respuesta antes de compararla
            string droppedWordTrimmed = slot.droppedWord.Trim();
            PromptQuality matchQuality = CheckAnswerQuality(slot.assignedPrompt, droppedWordTrimmed);
            bool hasScoringMatch = matchQuality == PromptQuality.Meh
                || matchQuality == PromptQuality.Ok
                || matchQuality == PromptQuality.Good;

            // Solo sumamos puntos si la respuesta tuvo alguna calidad valida
            if (hasScoringMatch)
            {
                GameControl gameControl = GameControl.Instance;
                if (gameControl != null)
                {
                    if (GameControl.Instance.sfxManager != null)
                    {
                        GameControl.Instance.sfxManager.PlayGoodSound();
                    }
                    
                    gameControl.AddScore(GetPointsForQuality(matchQuality));
                    gameControl.AddCompletedPrompt();
                }
            }

            // Marcamos el slot como enviado antes de cambiar su contenido
            PromptEntry currentPrompt = slot.assignedPrompt;
            slot.isSubmitted = true;
            AssignRandomPromptToSlot(slotIndex, currentPrompt);

            // Mostramos un mensaje de calidad solo cuando la respuesta fue aceptable
            if (hasScoringMatch && slot?.qualityText != null)
            {
                int pts = GetPointsForQuality(matchQuality);
                // El texto final depende del nivel de calidad obtenido
                string msg = matchQuality switch
                {
                    PromptQuality.Good => $"Perfecto! +{pts}",
                    PromptQuality.Ok => $"Bien. +{pts}",
                    PromptQuality.Meh => $"Ok... +{pts}",
                    _ => null
                };

                Color col = matchQuality switch
                {
                    PromptQuality.Good => Color.green,
                    PromptQuality.Ok => Color.white,
                    PromptQuality.Meh => Color.black,
                    _ => Color.white
                };

                // Si hay texto valido, lo mostramos por unos segundos
                if (msg != null)
                {
                    slot.ShowQuality(msg, col, qualityDisplaySeconds);
                }
            }
        }

        // Asigna un nuevo prompt a un slot evitando uno concreto
        private void AssignRandomPromptToSlot(int slotIndex, PromptEntry promptToAvoid)
        {
            // Verificamos indices y que el banco tenga datos
            if (!IsValidSlotIndex(slotIndex) || promptPool == null || promptPool.Length == 0)
            {
                return;
            }

            // Si el slot no existe, no seguimos
            PromptSlot slot = promptSlots[slotIndex];
            if (slot == null)
            {
                return;
            }

            // Buscamos un prompt distinto al que se acaba de resolver
            PromptEntry nextPrompt = GetRandomPrompt(promptToAvoid);
            slot.SetPrompt(nextPrompt);
        }

        // Obtiene un prompt al azar intentando no repetir el actual
        private PromptEntry GetRandomPrompt(PromptEntry promptToAvoid)
        {
            // Si no hay banco de prompts, devolvemos nada
            if (promptPool == null || promptPool.Length == 0)
            {
                return null;
            }

            // Si solo hay uno o no hay nada que evitar, devolvemos cualquiera
            if (promptPool.Length == 1 || promptToAvoid == null)
            {
                return promptPool[Random.Range(0, promptPool.Length)];
            }

            // Hacemos varios intentos aleatorios antes de caer a una busqueda lineal
            for (int i = 0; i < 8; i++)
            {
                PromptEntry candidate = promptPool[Random.Range(0, promptPool.Length)];
                if (candidate != promptToAvoid)
                {
                    return candidate;
                }
            }

            // Si la aleatoriedad falla, encontramos el primero distinto
            foreach (PromptEntry prompt in promptPool)
            {
                if (prompt != promptToAvoid)
                {
                    return prompt;
                }
            }

            // Ultimo recurso: devolvemos el primero disponible
            return promptPool[0];
        }

        // Compara la palabra entregada con las listas de calidad del prompt
        private PromptQuality CheckAnswerQuality(PromptEntry prompt, string word)
        {
            // Si falta el prompt o la palabra, no hay evaluacion posible
            if (prompt == null || string.IsNullOrWhiteSpace(word))
            {
                return (PromptQuality)(-1);
            }

            // Primero probamos la mejor calidad
            if (IsWordInArray(prompt.goodAnswers, word))
            {
                return PromptQuality.Good;
            }

            // Luego la calidad intermedia
            if (IsWordInArray(prompt.okAnswers, word))
            {
                return PromptQuality.Ok;
            }

            // Por ultimo la calidad baja
            if (IsWordInArray(prompt.mehAnswers, word))
            {
                return PromptQuality.Meh;
            }

            // Si no coincide con ninguna lista, devolvemos invalido
            return (PromptQuality)(-1);
        }

        // Verifica si una palabra existe dentro de un arreglo
        private bool IsWordInArray(string[] array, string word)
        {
            // Sin datos no hay forma de hacer match
            if (array == null || array.Length == 0)
            {
                return false;
            }

            // Comparamos ignorando mayusculas, minusculas y espacios extra
            foreach (string item in array)
            {
                if (string.Equals(item.Trim(), word, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Cancela el llenado de un prompt especifico
        public void CancelPrompt(int slotIndex)
        {
            // Si el indice no existe, salimos rapido
            if (!IsValidSlotIndex(slotIndex))
            {
                return;
            }

            // Si el slot no existe o no tiene prompt, no hay nada que cancelar
            PromptSlot slot = promptSlots[slotIndex];
            if (slot == null || slot.assignedPrompt == null)
            {
                return;
            }

            // Un prompt enviado ya no se puede deshacer
            if (slot.isSubmitted)
            {
                return;
            }

            // Devolvemos la palabra al slot original y limpiamos el prompt
            slot.CancelFill();
        }

        // Limpia todos los slots de prompt de la interfaz
        public void ClearAllPromptSlots()
        {
            if (promptSlots == null)
            {
                return;
            }

            // Recorremos uno por uno para borrar estado y texto
            foreach (PromptSlot slot in promptSlots)
            {
                if (slot == null)
                {
                    continue;
                }

                slot.Clear();
            }
        }

        // Asigna prompts aleatorios unicos a cada slot
        public void AssignRandomPromptsToSlots()
        {
            // Si no hay datos suficientes, no hacemos nada
            if (promptPool == null || promptPool.Length == 0 || promptSlots == null || promptSlots.Length == 0)
            {
                Debug.LogWarning($"AssignRandomPromptsToSlots skipped. promptPool={(promptPool != null ? promptPool.Length : 0)}, promptSlots={(promptSlots != null ? promptSlots.Length : 0)}.", this);
                return;
            }

            // Copiamos el pool para ir quitando prompts ya usados
            List<PromptEntry> availablePrompts = new List<PromptEntry>(promptPool);

            // Asignamos un prompt distinto a cada slot activo
            for (int i = 0; i < promptSlots.Length; i++)
            {
                PromptSlot slot = promptSlots[i];
                if (slot == null)
                {
                    continue;
                }

                // Si ya no quedan prompts unicos, paramos y avisamos
                if (availablePrompts.Count == 0)
                {
                    Debug.LogWarning("Not enough unique prompts for all slots.", this);
                    break;
                }

                // Escogemos uno al azar y lo quitamos del listado libre
                int randomIndex = Random.Range(0, availablePrompts.Count);
                PromptEntry randomPrompt = availablePrompts[randomIndex];
                availablePrompts.RemoveAt(randomIndex);
                slot.SetPrompt(randomPrompt);
            }
        }

        // Convierte una calidad de respuesta en puntos
        private int GetPointsForQuality(PromptQuality quality)
        {
            // Cada calidad tiene su propio valor
            switch (quality)
            {
                case PromptQuality.Meh:
                    return mehPoints;
                case PromptQuality.Good:
                    return goodPoints;
                default:
                    return okPoints;
            }
        }

        

        

        // Verifica que un indice pertenezca al arreglo de prompts
        private bool IsValidSlotIndex(int slotIndex)
        {
            return promptSlots != null && slotIndex >= 0 && slotIndex < promptSlots.Length;
        }

        // Busca el primer slot libre para guardar una palabra recogida
        private bool AssignToFirstEmptySlot(string word)
        {
            // Recorremos todos los slots hasta encontrar uno vacio
            foreach (DraggableWord slot in wordSlots)
            {
                if (slot == null || slot.HasWord)
                {
                    continue;
                }

                // Al primer slot libre le damos la palabra y terminamos
                slot.SetWord(word);
                return true;
            }

            return false;
        }

        // Comprueba donde hizo click el jugador dentro de la zona de prompts
        private void HandlePromptActionClick(Vector2 worldPoint)
        {
            // Sin slots no hay nada que procesar
            if (promptSlots == null)
            {
                return;
            }

            // Revisamos cada slot para ver si el click fue en enviar o cancelar
            for (int i = 0; i < promptSlots.Length; i++)
            {
                PromptSlot slot = promptSlots[i];
                if (slot == null)
                {
                    continue;
                }

                // Si clico en enviar, resolvemos el prompt y salimos
                if (slot.ContainsSubmitPoint(worldPoint))
                {
                    SubmitPrompt(i);
                    return;
                }

                // Si clico en cancelar, devolvemos la palabra y salimos
                if (slot.ContainsCancelPoint(worldPoint))
                {
                    CancelPrompt(i);
                    return;
                }
            }
        }

        // Busca las acciones de input usadas por este controlador
        private void ResolveInputActions()
        {
            // La accion de click sirve para resolver submits y cancels
            if (clickAction == null)
            {
                clickAction = InputSystem.actions.FindAction(clickActionName);
            }

            // La accion del puntero da la posicion del mouse o tactil
            if (pointerAction == null)
            {
                pointerAction = InputSystem.actions.FindAction(pointerActionName);
            }
        }

        // Actualiza la UI con el numero real de palabras en storage
        private void RefreshWordCount()
        {
            // Si no hay slots, el contador es cero y la UI se limpia
            if (wordSlots == null || wordSlots.Length == 0)
            {
                currentWordCount = 0;

                // Si existe UI, la sincronizamos para que no quede desfasada
                if (GameControl.Instance != null && GameControl.Instance.uiControl != null)
                {
                    GameControl.Instance.uiControl.SetWordsStorage(currentWordCount, 0);
                }

                return;
            }

            // Contamos solo los slots que de verdad tienen palabra
            int count = 0;
            foreach (DraggableWord slot in wordSlots)
            {
                if (slot != null && slot.HasWord)
                {
                    count++;
                }
            }

            currentWordCount = count;

            // Refrescamos la UI si el juego ya esta conectado
            if (GameControl.Instance != null && GameControl.Instance.uiControl != null)
            {
                GameControl.Instance.uiControl.SetWordsStorage(currentWordCount, wordStorageCapacity);
            }

            // Auto-open prompts panel cuando el jugador llena el storage
            if (GameControl.Instance != null && GameControl.Instance.uiControl != null)
            {
                var ui = GameControl.Instance.uiControl;
                if (wordStorageCapacity > 0 && currentWordCount == wordStorageCapacity && !ui.IsPromptsPanelOpen())
                {
                    ui.TogglePromptsPanel();
                }
            }
        }
    }
}
