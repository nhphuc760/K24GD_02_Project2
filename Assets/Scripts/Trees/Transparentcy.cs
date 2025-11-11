using UnityEngine;

public class Transparentcy : MonoBehaviour
{
    [SerializeField] SpriteRenderer body;
    [SerializeField] float alpha = .5f;

    private void Awake()
    {
        var body = transform.Find("Body");
        if(body != null)
        {
            body.gameObject.SetActive(true);
        }
        var root = transform.Find("Root");
        if(root != null)
        {
            root.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            body.color = new Color(body.color.r, body.color.g, body.color.b, alpha);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            body.color = Color.white;
        }
    }
}
