using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    private float lifetime = 1f;
    private Color textColor;

    void Start()
    {
        textColor = damageText.color;
    }

    void Update()
    {
        // Bay lên
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        // Mờ dần
        textColor.a -= fadeSpeed * Time.deltaTime;
        damageText.color = textColor;
        // Hết lifetime => destroy
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f || textColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(float damage)
    {
        damageText.text = damage.ToString("F0"); // Số nguyên
    }
}
