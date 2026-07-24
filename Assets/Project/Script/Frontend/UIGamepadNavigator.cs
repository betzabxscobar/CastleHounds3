using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// -------------------------------------------------------------
// Hace que TODOS los menús del juego se puedan usar con el mando:
//   - Cruceta / stick izquierdo  -> moverse entre botones
//   - Botón X (cruz / buttonSouth) -> confirmar (Submit)
//   - Botón O (círculo / buttonEast) -> cancelar (Cancel)
//
// Reutiliza la navegación nativa de Unity UI (EventSystem), así que
// el ratón sigue funcionando igual. No hay que configurar nada por
// escena: el bootstrap de abajo lo instala solo en cualquier escena
// que tenga UI.
// -------------------------------------------------------------
public class UIGamepadNavigator : MonoBehaviour
{
    private GameObject lastSelected;


    private void Start()
    {
        EnsureInputModule();
        SelectFirst();
    }


    private void Update()
    {
        EventSystem es = EventSystem.current;
        if (es == null)
        {
            return;
        }

        GameObject current = es.currentSelectedGameObject;

        // Si hay un botón seleccionado y sigue activo, recordarlo.
        if (current != null && current.activeInHierarchy)
        {
            lastSelected = current;
            return;
        }

        // Se perdió la selección (por ejemplo tras un clic de ratón, o
        // porque se abrió un panel nuevo como el de pausa). En cuanto el
        // jugador toca la cruceta / stick / X, se vuelve a enfocar un botón.
        if (NavigationPressed())
        {
            GameObject target =
                (lastSelected != null && lastSelected.activeInHierarchy)
                    ? lastSelected
                    : FindFirstSelectable();

            if (target != null)
            {
                es.SetSelectedGameObject(target);
                lastSelected = target;
            }
        }
    }


    private void SelectFirst()
    {
        EventSystem es = EventSystem.current;
        if (es == null)
        {
            return;
        }

        GameObject first = FindFirstSelectable();
        if (first != null)
        {
            es.SetSelectedGameObject(first);
            lastSelected = first;
        }
    }


    // Asegura que el EventSystem use el módulo del nuevo Input System
    // con las acciones de UI por defecto (que ya incluyen cruceta = Navigate
    // y botón X = Submit).
    private static void EnsureInputModule()
    {
        EventSystem es = EventSystem.current;
        if (es == null)
        {
            es = FindObjectOfType<EventSystem>();
        }

        if (es == null)
        {
            GameObject go = new GameObject("EventSystem");
            es = go.AddComponent<EventSystem>();
        }

        // Desactivar el módulo viejo (Input Manager) si existe.
        StandaloneInputModule legacy = es.GetComponent<StandaloneInputModule>();
        if (legacy != null)
        {
            legacy.enabled = false;
        }

        InputSystemUIInputModule module = es.GetComponent<InputSystemUIInputModule>();
        if (module == null)
        {
            module = es.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        // Si no tiene acciones asignadas, usar las de por defecto
        // (Navigate con cruceta/stick, Submit con X, Cancel con O).
        if (module.actionsAsset == null)
        {
            module.AssignDefaultActions();
        }
    }


    private static GameObject FindFirstSelectable()
    {
        Selectable[] all = Selectable.allSelectablesArray;

        for (int i = 0; i < all.Length; i++)
        {
            Selectable s = all[i];

            if (s != null &&
                s.IsActive() &&
                s.IsInteractable() &&
                s.gameObject.activeInHierarchy)
            {
                return s.gameObject;
            }
        }

        return null;
    }


    private static bool NavigationPressed()
    {
        Gamepad gp = Gamepad.current;
        if (gp != null)
        {
            if (gp.dpad.up.wasPressedThisFrame ||
                gp.dpad.down.wasPressedThisFrame ||
                gp.dpad.left.wasPressedThisFrame ||
                gp.dpad.right.wasPressedThisFrame ||
                gp.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }

            if (gp.leftStick.ReadValue().magnitude > 0.5f)
            {
                return true;
            }
        }

        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.upArrowKey.wasPressedThisFrame ||
                kb.downArrowKey.wasPressedThisFrame ||
                kb.leftArrowKey.wasPressedThisFrame ||
                kb.rightArrowKey.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }
}


// -------------------------------------------------------------
// Instala automáticamente el UIGamepadNavigator en cualquier escena
// que tenga UI (un Canvas). Así no hay que agregar nada a mano en
// cada menú.
// -------------------------------------------------------------
public static class MenuGamepadBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneHandler()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        Install(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Install(scene);
    }

    private static void Install(Scene scene)
    {
        // Solo si la escena tiene UI.
        if (Object.FindAnyObjectByType<Canvas>() == null)
        {
            return;
        }

        // No duplicar si ya existe.
        if (Object.FindAnyObjectByType<UIGamepadNavigator>() != null)
        {
            return;
        }

        GameObject go = new GameObject("UIGamepadNavigator");
        go.AddComponent<UIGamepadNavigator>();
    }
}
