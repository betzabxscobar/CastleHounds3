using System.Collections.Generic;
using UnityEngine;

public class DogWeapon : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    [Tooltip("Capas donde están los enemigos. Déjalo en 'Everything' si no usas capas específicas.")]
    [SerializeField] private LayerMask capasEnemigo = ~0;

    private Collider weaponCollider;

    // Enemigos ya golpeados en el swing actual, para no aplicarles
    // daño varias veces con un solo ataque.
    private readonly HashSet<EnemigoSalud> golpeadosEsteSwing = new HashSet<EnemigoSalud>();

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();

        if (weaponCollider != null)
        {
            // La hitbox de la espada siempre debe comportarse como trigger.
            weaponCollider.isTrigger = true;
            weaponCollider.enabled = false;
        }
        else
        {
            Debug.LogError(
                "DogWeapon: no hay Collider en " + name +
                ". Añade un Collider (Box/Capsule) marcado como Is Trigger."
            );
        }
    }

    public void EnableHitbox()
    {
        golpeadosEsteSwing.Clear();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        // Detección inmediata: cubre el caso (el más común en combate) en que
        // el enemigo ya está DENTRO de la hitbox al activarse. En ese caso
        // OnTriggerEnter no dispara porque no hay una "entrada" nueva, así que
        // comprobamos el solapamiento a mano.
        DetectarSolapados();
    }

    public void DisableHitbox()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    // Enemigos que ENTRAN en la hitbox mientras está activa (p.ej. si se mueven
    // hacia el perro durante el swing). Requiere Rigidbody para dispararse.
    private void OnTriggerEnter(Collider other)
    {
        IntentarGolpear(other);
    }

    // Enemigos que permanecen dentro de la hitbox. Refuerza la detección.
    private void OnTriggerStay(Collider other)
    {
        IntentarGolpear(other);
    }

    private void DetectarSolapados()
    {
        if (weaponCollider == null)
        {
            return;
        }

        // bounds ya está en espacio mundo (AABB), por eso usamos rotación identidad.
        Bounds b = weaponCollider.bounds;

        Collider[] posibles = Physics.OverlapBox(
            b.center,
            b.extents,
            Quaternion.identity,
            capasEnemigo,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider c in posibles)
        {
            IntentarGolpear(c);
        }
    }

    private void IntentarGolpear(Collider other)
    {
        // El collider del enemigo puede estar en un hijo; se busca hacia arriba.
        EnemigoSalud enemy = other.GetComponentInParent<EnemigoSalud>();

        if (enemy != null && !golpeadosEsteSwing.Contains(enemy))
        {
            golpeadosEsteSwing.Add(enemy);
            enemy.RecibirDanio(damage);
        }
    }
}
