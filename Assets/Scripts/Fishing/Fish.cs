using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement; //đặt tạm tránh conflict
    [SerializeField] PlayerFishing player; //đặt tạm tránh conflict
    [SerializeField] PlayerVisual playerVisual; //đặt tạm tránh conflict
    private void OnEnable()
    {
        StartCoroutine(FlyFishToPlayer());
        var dir = playerMovement.GetDirection();
        if (dir == Vector2.zero) return;
        // Các hướng chuẩn
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        // Tương ứng là vị trí cần đặt
        Transform[] points =
        {
        player.fishingPointBack,
        player.fishingPointFront,
        player.fishingPointLeft,
        player.fishingPointRight
    };
        float bestDot = float.NegativeInfinity;
        Transform bestPoint = player.fishingPointFront;

        for (int i = 0; i < dirs.Length; i++)
        {
            float dot = Vector2.Dot(dir, dirs[i]);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestPoint = points[i];
            }
        }

        transform.position = bestPoint.position;
    }
    IEnumerator FlyFishToPlayer()
    {
        Vector2 startPos = player.fishingPoint.position;
        Vector2 endPos = player.transform.position;
        float time = 0f;
        float flyDuration = playerVisual.animator.GetCurrentAnimatorStateInfo(0).length;
        while (time < flyDuration)
        {
            time+= Time.deltaTime;
            float t = time / flyDuration;
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            float height = playerVisual.animationCurve.Evaluate(t);
            pos.y += height * 3.5f;
            transform.position = pos;
            yield return null;
        }
        transform.position = player.transform.position;
        gameObject.SetActive(false);
    }
}
