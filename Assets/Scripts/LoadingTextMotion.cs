using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadingTextMotion : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 2f;
    public float amplitude = 5f;
    Vector3 startPos;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
