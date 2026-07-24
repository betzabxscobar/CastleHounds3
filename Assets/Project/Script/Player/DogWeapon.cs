using UnityEngine;

public class DogWeapon : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    private Collider weaponCollider;

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
    }

    public void EnableHitbox()
    {
        weaponCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemigoSalud enemy = other.GetComponent<EnemigoSalud>();

        if (enemy != null)
        {
            enemy.RecibirDanio(damage);
        }
    }
}