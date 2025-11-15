using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public AnimationCurve arcCurve;
    private void OnEnable()
    {
        StartCoroutine(FlyFishToPlayer());
        var dir = PlayerMovement.Ins.facingDirection;
        switch (dir)
        {
            case FacingDirection.Back:
                transform.position = Player.Ins.fishingPointBack.position;
                break;
            case FacingDirection.Front:
                transform.position = Player.Ins.fishingPointFront.position;
                break;
            case FacingDirection.Left:
                transform.position = Player.Ins.fishingPointLeft.position;
                break;
            case FacingDirection.Right:
                transform.position = Player.Ins.fishingPointRight.position;
                break;
            default:
                break;
        }

    }
    IEnumerator FlyFishToPlayer()
    {
        Vector2 startPos = Player.Ins.fishingPoint.position;
        Vector2 endPos = Player.Ins.transform.position;
        float time = 0f;
        float flyDuration = PlayerVisual.Ins.animator.GetCurrentAnimatorStateInfo(0).length;
        while (time < flyDuration)
        {
            time+= Time.deltaTime;
            float t = time / flyDuration;
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            float height = PlayerVisual.Ins.animationCurve.Evaluate(t);
            pos.y += height * 3.5f;
            transform.position = pos;
            yield return null;
        }
        transform.position = Player.Ins.transform.position;
        gameObject.SetActive(false);
    }
}
