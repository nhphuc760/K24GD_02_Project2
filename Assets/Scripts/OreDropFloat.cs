using UnityEngine;
using System.Collections;
public class OreDropFloat : MonoBehaviour
{
    float floatAmplitude = 0.1f;    //Biên độ dao động (cao thấp)
    float floatFrequency = 2f;      //tốc độ dao động
    public float floatDelay = 0.5f;        // Thời gian chờ trước khi bắt đầu lơ lửng
    Vector3 startPos;
    float floatTimer;
    Rigidbody2D rb;
    bool startFloating = false;

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
        if(rb  != null )
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        startFloating = true;
        startPos = transform.position;
    }
    void Update()
    {
        if(!startFloating) return;

        floatTimer += Time.deltaTime * floatFrequency;
        transform.position = startPos + new Vector3(0, Mathf.Sin(floatTimer) * floatAmplitude, 0);
    }
}
