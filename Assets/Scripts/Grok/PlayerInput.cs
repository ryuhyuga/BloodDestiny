using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(ComboSystem))]
public class PlayerInput : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private ComboSystem comboSystem;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        comboSystem = GetComponent<ComboSystem>();
    }

    void Update()
    {
        // 1) Di chuyển
        float moveH = Input.GetAxis("Horizontal");
        playerMovement.MoveHorizontal(moveH);

        // 2) Nhảy
        if (Input.GetButtonDown("Jump"))
        {
            playerMovement.Jump();
        }

        // 3) Combo
        if (Input.GetKeyDown(KeyCode.J))
        {
            comboSystem.RegisterAttack(false); // Light
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            comboSystem.RegisterAttack(true);  // Heavy
        }
    }
}
