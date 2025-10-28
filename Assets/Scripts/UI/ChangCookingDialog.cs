using UnityEngine;

public class ChangCookingDialog : MonoBehaviour
{
    [SerializeField] Animator animator;
    RecipeSO recipe;

    private void Start()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        GameEventManager.Ins.cookingEvent.enableDialogCookingChange += CookingEvent_enableDialogCookingChange;
        gameObject.SetActive(false);
    }

    private void CookingEvent_enableDialogCookingChange(RecipeSO recipe)
    {
        this.recipe = recipe;
        gameObject.SetActive(true);
    }

    public void ConfirmChangeCooking()
    {
        GameEventManager.Ins.cookingEvent.ConfirmCookingChange(recipe);
    }
    public void TriggerDisable()
    {
        if (animator != null)
        {
            animator.Play("ChangeCookingDisable");
        }
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        GameEventManager.Ins.cookingEvent.enableDialogCookingChange -= CookingEvent_enableDialogCookingChange;
    }
}
