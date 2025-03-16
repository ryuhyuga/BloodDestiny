using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    public GameObject damagePopupPrefab;
    public Canvas mainCanvas; // Canvas ở chế độ Screen Space

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnDamageText(float damage, Vector3 worldPos)
    {
        Debug.Log($"[DamagePopupManager] SpawnDamageText called with damage={damage}, worldPos={worldPos}");
        Debug.Log($"damagePopupPrefab={damagePopupPrefab}, mainCanvas={mainCanvas}");
        // Nếu prefab hoặc mainCanvas chưa được gán, ta bỏ qua
        if (damagePopupPrefab == null || mainCanvas == null)
        {
            Debug.LogWarning("damagePopupPrefab or mainCanvas is NULL => skipping spawn!");
            return;
        }

        // Thêm offset Y = 1f để text xuất hiện trên đầu đối tượng
        Vector3 spawnPos = worldPos + new Vector3(0, 1f, 0);

        // Chuyển sang toạ độ màn hình (pixel)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPos);

        // Tạo popup dưới mainCanvas
        GameObject popup = Instantiate(damagePopupPrefab, mainCanvas.transform);
        Debug.Log($"[DamagePopupManager] popup instantiated => {popup.name}");
        // Lấy RectTransform để đặt vị trí hiển thị
        RectTransform rect = popup.GetComponent<RectTransform>();
        // Đặt anchoredPosition = screenPos
        rect.anchoredPosition = screenPos;

        // Setup text hiển thị damage
        DamagePopup dmgPopup = popup.GetComponent<DamagePopup>();
        dmgPopup.Setup(damage);
    }
}
