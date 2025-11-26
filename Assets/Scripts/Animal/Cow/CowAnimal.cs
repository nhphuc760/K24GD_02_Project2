using UnityEngine;

public class CowAnimal : FarmAnimal
{
    [SerializeField] Animator animator;
    int HORIZONTAL = Animator.StringToHash("Horizontal");
    int VERTICAL = Animator.StringToHash("Vertical");
    protected override void Flip()
    {
        Vector2 direct = rb.linearVelocity.normalized;
        animator.SetFloat(HORIZONTAL, direct.x);
        animator.SetFloat(VERTICAL, direct.y);
        visual.flipX = direct.x > 0 ? false : true;
    }
}
