using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


// -------------------------------------------------------------
// Permite jugar el minijuego de la poción con el mando:
//   - Cruceta (arriba/abajo/izquierda/derecha) -> elegir ingrediente
//   - Botón X (cruz / buttonSouth) -> mandarlo al caldero
//
// No reemplaza el control con ratón: ambos funcionan a la vez.
// Basta con poner este componente en un objeto de la escena
// (por ejemplo el mismo del PotionManager). Si no se asignan las
// referencias a mano, se toman automáticamente.
// -------------------------------------------------------------
public class PotionGamepadSelector : MonoBehaviour
{
    [Header("Ingredientes seleccionables (en orden)")]
    [Tooltip("Si se deja vacío, se toman los del PotionManager (objeto Ingredientes).")]
    public List<IngredientDrag> ingredients = new List<IngredientDrag>();


    [Header("Caldero destino")]
    [Tooltip("Si se deja vacío, se busca el PotionCauldron de la escena (objeto Caldero).")]
    public PotionCauldron cauldron;


    [Header("Resaltado del seleccionado")]
    [Tooltip("Cuánto se agranda el ingrediente seleccionado para que se note.")]
    public float selectedScaleMultiplier = 1.25f;


    private int selectedIndex = -1;
    private Vector3[] baseScales;


    private void Start()
    {
        // Tomar ingredientes del PotionManager si no se asignaron a mano.
        if ((ingredients == null || ingredients.Count == 0) && PotionManager.Instance != null)
        {
            ingredients = PotionManager.Instance.ingredients;
        }

        // Último recurso: buscar TODOS los IngredientDrag de la escena
        // (los hijos del objeto "Ingredientes"), ordenados por nombre para
        // que la navegación sea siempre la misma.
        if (ingredients == null || ingredients.Count == 0)
        {
            IngredientDrag[] found = FindObjectsByType<IngredientDrag>(FindObjectsSortMode.None);
            System.Array.Sort(found, (a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            ingredients = new List<IngredientDrag>(found);
        }

        // Buscar el caldero si no se asignó.
        if (cauldron == null)
        {
            cauldron = FindAnyObjectByType<PotionCauldron>();
        }

        Debug.Log(
            "[PotionGamepadSelector] Ingredientes encontrados: " +
            (ingredients != null ? ingredients.Count : 0) +
            " | Caldero: " + (cauldron != null ? cauldron.name : "NINGUNO") +
            " | Mando: " + (Gamepad.current != null ? Gamepad.current.displayName : "NO DETECTADO"));

        // Guardar la escala original de cada ingrediente para el resaltado.
        if (ingredients != null)
        {
            baseScales = new Vector3[ingredients.Count];
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (ingredients[i] != null)
                {
                    baseScales[i] = ingredients[i].transform.localScale;
                }
            }
        }

        SelectFirstAvailable();
    }


    private void Update()
    {
        Gamepad gp = Gamepad.current;
        Keyboard kb = Keyboard.current;


        // ---------- NAVEGACIÓN ----------
        bool prev =
            WasPressed(gp != null ? gp.dpad.up : null, kb != null ? kb.upArrowKey : null) ||
            WasPressed(gp != null ? gp.dpad.left : null, kb != null ? kb.leftArrowKey : null);

        bool next =
            WasPressed(gp != null ? gp.dpad.down : null, kb != null ? kb.downArrowKey : null) ||
            WasPressed(gp != null ? gp.dpad.right : null, kb != null ? kb.rightArrowKey : null);

        if (prev)
        {
            MoveSelection(-1);
        }
        else if (next)
        {
            MoveSelection(1);
        }


        // ---------- CONFIRMAR CON X ----------
        bool place =
            (gp != null && gp.buttonSouth.wasPressedThisFrame) ||
            (kb != null && kb.enterKey.wasPressedThisFrame);

        if (place)
        {
            PlaceSelected();
        }
    }


    private static bool WasPressed(ButtonControl a, ButtonControl b)
    {
        return (a != null && a.wasPressedThisFrame) ||
               (b != null && b.wasPressedThisFrame);
    }


    private bool IsAvailable(int index)
    {
        if (ingredients == null || index < 0 || index >= ingredients.Count)
        {
            return false;
        }

        IngredientDrag ing = ingredients[index];

        return ing != null && ing.gameObject.activeSelf && !ing.IsPlaced;
    }


    private void SelectFirstAvailable()
    {
        if (ingredients == null)
        {
            return;
        }

        for (int i = 0; i < ingredients.Count; i++)
        {
            if (IsAvailable(i))
            {
                SetSelection(i);
                return;
            }
        }

        SetSelection(-1);
    }


    private void MoveSelection(int direction)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            return;
        }

        int count = ingredients.Count;
        int index = selectedIndex;

        // Buscar el siguiente ingrediente disponible en la dirección dada.
        for (int step = 0; step < count; step++)
        {
            index = (index + direction + count) % count;

            if (IsAvailable(index))
            {
                SetSelection(index);
                return;
            }
        }
    }


    private void SetSelection(int newIndex)
    {
        // Quitar el resaltado del anterior.
        if (selectedIndex >= 0 &&
            selectedIndex < ingredients.Count &&
            ingredients[selectedIndex] != null &&
            baseScales != null)
        {
            ingredients[selectedIndex].transform.localScale = baseScales[selectedIndex];
        }

        selectedIndex = newIndex;

        // Aplicar resaltado al nuevo.
        if (selectedIndex >= 0 &&
            selectedIndex < ingredients.Count &&
            ingredients[selectedIndex] != null &&
            baseScales != null)
        {
            ingredients[selectedIndex].transform.localScale =
                baseScales[selectedIndex] * selectedScaleMultiplier;
        }
    }


    private void PlaceSelected()
    {
        if (!IsAvailable(selectedIndex))
        {
            SelectFirstAvailable();
            return;
        }

        IngredientDrag ing = ingredients[selectedIndex];

        // Restaurar la escala antes de mandarlo al caldero.
        if (baseScales != null)
        {
            ing.transform.localScale = baseScales[selectedIndex];
        }

        if (cauldron != null && cauldron.ingredientPoint != null)
        {
            // Anima el ingrediente hacia el caldero (igual que con ratón).
            ing.EnterCauldron(cauldron.ingredientPoint);
        }
        else
        {
            // Sin caldero asignado: colocarlo directo.
            ing.PlaceInCauldron();
        }

        // Pasar la selección al siguiente disponible.
        SelectFirstAvailable();
    }
}
