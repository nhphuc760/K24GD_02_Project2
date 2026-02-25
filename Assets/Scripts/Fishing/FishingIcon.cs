using UnityEngine;

public class FishingIcon : MonoBehaviour
{
    Vector2 direct;
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    [SerializeField] float speed = 1.5f;
    [SerializeField] SpriteRenderer sprite;
    void Start()
    {
        direct = new Vector2(0, 1);
    }

    private void OnEnable()
    {
        speed = Random.Range(.5f, speed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)direct * speed * Time.deltaTime;
        if(Vector2.Distance(transform.position, top.position) <= 0.1f || Vector2.Distance(transform.position, bottom.position) <= 0.1f)
        {
            Flip();
        }
    }

    void Flip()
    {
        direct *= -1;
        sprite.flipY = !sprite.flipY;
    }

}
