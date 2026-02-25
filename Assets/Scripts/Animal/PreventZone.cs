using UnityEngine;

public class PreventZone : MonoBehaviour
{
    [SerializeField] Transform spawnAnimalPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<FarmAnimal>(out FarmAnimal farmAnimal))
        {
            farmAnimal.GetComponent<Rigidbody2D>().linearVelocity *= -1;
        }
    
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<FarmAnimal>(out FarmAnimal farmAnimal))
        {
            Vector2 this_farmAnimal = farmAnimal.transform.position - this.transform.position;
            float direct = Vector2.Dot(Vector2.up, this_farmAnimal.normalized);
            if(direct > 0)
            {
                farmAnimal.transform.position = spawnAnimalPoint.position;
            }
            
        }
    }
}
