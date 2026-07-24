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
}