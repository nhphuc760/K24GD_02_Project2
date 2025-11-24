using UnityEngine;
using System.Collections;
public class ItemDropFloat : MonoBehaviour
{
    public float floatAmplitude = 0.1f;    //Biên độ dao động (cao thấp)
    public float floatFrequency = 2f;      //tốc độ dao động
    public float floatDelay = 0.5f;        // Thời gian chờ trước khi bắt đầu lơ lửng
    [SerializeField] float timeDestroy = 10f; //trong 10s nếu không nhặt sẽ hủy
    Vector3 startPos;
    float floatTimer;
    Rigidbody2D rb;
    [SerializeField] ItemDataSO reSourceSO;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if(rb != null )
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rb.linearVelocity = randomDir * Random.Range(1f,2f) + Vector2.up * 1f;
            rb.gravityScale = 1f;
        }

        StartCoroutine(StartFloating());
    }

    IEnumerator StartFloating()
    {
        yield return new WaitForSeconds(floatDelay);
        GetComponent<Collider2D>().isTrigger = true;
        if(rb  != null )
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        startPos = transform.position;
        float timeHolder = timeDestroy;
        while (timeHolder > 0)
        {
            timeHolder -= Time.deltaTime;
            floatTimer += Time.deltaTime * floatFrequency;
            transform.position = startPos + new Vector3(0, Mathf.Sin(floatTimer) * floatAmplitude, 0);
            yield return null;
        }
        Destroy(this.gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEventManager.Ins.inventoryEvent.AddItem(reSourceSO, 1);
            Destroy(gameObject);
        }
    }

}
