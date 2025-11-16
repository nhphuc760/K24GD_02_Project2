using Unity.VisualScripting;
using UnityEngine;

public class FishingBar : MonoBehaviour
{
    public Rigidbody rb;
    public float targetTime = 4.0f;
    public float savedTargetTime;
    [SerializeField] Player player;
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    public GameObject p5;
    public GameObject p6;
    public GameObject p7;
    public GameObject p8;

    public bool onFish;
    [SerializeField] PlayerVisual playerVisual;
    void Update()
    {
        if (onFish)
        {
            targetTime += Time.deltaTime;
        }
        if (!onFish)
        {
            targetTime -= Time.deltaTime;
        }
        if(targetTime <= 0.0f)
        {
            transform.localPosition = new Vector3(-3.6086f, -2f, 0);
            onFish = false;
            player.fishGameLossed();
            Destroy(GameObject.Find("Bobber(Clone)"));
            targetTime = 3.0f;
        }
        if (targetTime >= 8.0f)
        {
            transform.localPosition = new Vector3(-3.6086f, -2f, 0);
            onFish = false;
            player.fishGameWon();
            Destroy(GameObject.Find("Bobber(Clone)"));
            targetTime = 3.0f;
        }
        if (targetTime >= 0.0f)
        {
            p1.SetActive(false);
            p2.SetActive(false);
            p3.SetActive(false);
            p4.SetActive(false);
            p5.SetActive(false);
            p6.SetActive(false);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 1.0f)
        {
            p1.SetActive(true);
            p2.SetActive(false);
            p3.SetActive(false);
            p4.SetActive(false);
            p5.SetActive(false);
            p6.SetActive(false);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 2.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(false);
            p4.SetActive(false);
            p5.SetActive(false);
            p6.SetActive(false);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 3.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(true);
            p4.SetActive(false);
            p5.SetActive(false);
            p6.SetActive(false);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 4.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(true);
            p4.SetActive(true);
            p5.SetActive(false);
            p6.SetActive(false);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 5.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(true);
            p4.SetActive(true);
            p5.SetActive(true);
            p6.SetActive(false);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 6.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(true);
            p4.SetActive(true);
            p5.SetActive(true);
            p6.SetActive(true);
            p7.SetActive(false);
            p8.SetActive(false);
        }
        if (targetTime >= 7.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(true);
            p4.SetActive(true);
            p5.SetActive(true);
            p6.SetActive(true);
            p7.SetActive(true);
            p8.SetActive(false);
        }
        if (targetTime >= 8.0f)
        {
            p1.SetActive(true);
            p2.SetActive(true);
            p3.SetActive(true);
            p4.SetActive(true);
            p5.SetActive(true);
            p6.SetActive(true);
            p7.SetActive(true);
            p8.SetActive(true);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up, ForceMode.Impulse);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FishIcon"))
        {
            onFish = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("FishIcon"))
        {
            onFish = false;
        }
    }
}
