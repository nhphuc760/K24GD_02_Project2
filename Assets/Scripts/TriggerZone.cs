using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            Vector3 direct = collision.transform.position - transform.position;
            float dot = Vector3.Dot(direct.normalized, Vector3.down);
            SpriteRenderer playerVisual = collision.GetComponentInChildren<SpriteRenderer>();
            if (dot < 0)
            {
                
                playerVisual.sortingLayerName = "BuildingLower";
                playerVisual.sortingOrder = 5;
                Physics2D.IgnoreLayerCollision(0,2 , true);
               
            }
            else
            {
                playerVisual.sortingLayerName = "Player";
                playerVisual.sortingOrder = 0;
                Physics2D.IgnoreLayerCollision(0, 2, false);
            }
        }
    }
}
