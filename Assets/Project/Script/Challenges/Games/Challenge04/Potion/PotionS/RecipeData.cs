using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NuevaReceta",
    menuName = "Potion/Recipe"
)]
public class RecipeData : ScriptableObject
{
    [Header("Información")]
    public string recipeName;

    [Header("Ingredientes")]
    public List<string> ingredients = new List<string>();

    [Header("Recompensa")]
    public ElementType rewardPower;

    [Header("Combate")]
    public GameObject enemyPrefab;

    [Header("Reto asociado a la casa")]
    [Tooltip("ID del reto que se marca como completado al ganar la pelea de esta casa. " +
             "Usa los valores de ChallengeProgressManager, p.ej. 'house_challenge_01'. " +
             "Déjalo vacío si esta receta no debe completar ningún reto.")]
    public string challengeId;
}