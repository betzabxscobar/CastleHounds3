using UnityEngine;

public sealed class AresAnimatorController : MonoBehaviour
{
    private const string SpeedParam = "Speed";
    private const string AttackParam = "Attack";
    private const string HitParam = "Hit";

    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float walkSpeedValue = 0.5f;
    [SerializeField, Min(0f)] private float runSpeedValue = 1f;
    [SerializeField, Min(0f)] private float attackCooldown = 0.35f;

    private float nextAttackTime;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Configure(Animator configuredAnimator)
    {
        animator = configuredAnimator;
    }

    public void SetMovement(float moveMagnitude01, bool sprinting)
    {
        if (animator == null)
        {
            return;
        }

        float speed = moveMagnitude01 <= 0f ? 0f : (sprinting ? runSpeedValue : walkSpeedValue);
        animator.SetFloat(SpeedParam, speed);
    }

    public void TriggerAttack()
    {
        if (animator == null || Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        animator.SetTrigger(AttackParam);
    }

    public void TriggerHit()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(HitParam);
    }
}
