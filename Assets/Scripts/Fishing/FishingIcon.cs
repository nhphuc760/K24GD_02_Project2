using UnityEngine;

public class FishingIcon : MonoBehaviour
{
    Vector3 direct;
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    [SerializeField] float speed = 1.5f;
    [SerializeField] SpriteRenderer sprite;
    void Start()
    {
        direct = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direct * speed * Time.deltaTime;
        if(Vector3.Distance(transform.position, top.position) <= 0.0001f && Vector3.Distance(transform.position, bottom.position) <= 0.0001f)
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
