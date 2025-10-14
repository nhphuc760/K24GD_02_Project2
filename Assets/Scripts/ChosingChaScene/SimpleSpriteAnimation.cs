using UnityEngine;
using UnityEngine.U2D.Animation;

public class SimpleSpriteAnimation : MonoBehaviour
{
    public string category = "Idle_Front";
    public float frameRate = 6f; // số frame mỗi giây

    private SpriteResolver resolver;
    private int currentFrame = 0;
    private float timer = 0f;

    // đổi số này nếu nhân vật có nhiều frame hơn
    private readonly string[] labels = { "Idle_0", "Idle_1", "Idle_2", "Idle_3" };

    void Awake()
    {
        resolver = GetComponent<SpriteResolver>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % labels.Length;
            resolver.SetCategoryAndLabel(category, labels[currentFrame]);
        }
    }
}
