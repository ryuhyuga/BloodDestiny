using UnityEngine;

public class LadderInteraction : MonoBehaviour
{
    private bool isOnLadder = false;
    private AdvancedCharacterController characterController;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            characterController = other.GetComponent<AdvancedCharacterController>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            characterController.StopClimbingLadder();
        }
    }

    void Update()
    {
        if (isOnLadder)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (verticalInput != 0)
            {
                characterController.ClimbLadder(verticalInput);
            }
            else
            {
                characterController.StopClimbingLadder();
            }
        }
    }
}