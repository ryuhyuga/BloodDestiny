using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerAnimationComponent : MonoBehaviour
{
    private Animator animator;
    private MovementComponent movement; // Để biết trạng thái di chuyển
    private StatsComponent stats;
    private CombatComponent combat;
    // Cache hash của các parameter/state để tối ưu
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashIsGrounded = Animator.StringToHash("IsGrounded");
    private readonly int hashJump = Animator.StringToHash("Jump");
    private readonly int hashHurt = Animator.StringToHash("Hurt");
    private readonly int hashDeath = Animator.StringToHash("Death");
    // Cache hash cho các trigger tấn công (có thể làm động nếu cần)

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<MovementComponent>(); // Giả sử có component này
        combat = GetComponentInParent<CombatComponent>();
        stats = GetComponent<StatsComponent>(); // Giả sử có component này
        GetComponent<StatsComponent>().OnDied += HandleDeath; // Lắng nghe sự kiện chết
         GetComponent<StatsComponent>().OnHealthChanged += HandleHurt; // Lắng nghe sự kiện nhận damage

         if(combat) Debug.LogWarning("CombatComponent không tìm thấy trên PlayerAnimationComponent!", this);
    }

    private void Update()
    {
        // Cập nhật các tham số cơ bản cho Animator
        if (movement)
        {
            animator.SetFloat(hashSpeed, Mathf.Abs(movement.GetComponent<Rigidbody2D>().linearVelocity.x)); // Dùng velocity x
            animator.SetBool(hashIsGrounded, movement.IsGrounded);
        }
    }

    public void PlayAttackAnimation(string triggerName)
    {
         if (string.IsNullOrEmpty(triggerName)) return;
         //Debug.Log($"Playing Animation: {triggerName}");
        animator.SetTrigger(triggerName);
    }

     public void PlayJumpAnimation()
     {
         animator.SetTrigger(hashJump);
     }

     private void HandleHurt(float currentHealth, float maxHealth)
     {
         // Chỉ trigger hurt nếu còn sống
         if (currentHealth > 0) {
              animator.SetTrigger(hashHurt);
         }
     }

     private void HandleDeath()
     {
         animator.SetTrigger(hashDeath);
         // Có thể tắt các component khác sau khi animation chết hoàn thành
         this.enabled = false; // Tắt component này đi
         GetComponent<MovementComponent>().enabled = false;
         GetComponent<CombatComponent>().enabled = false;
         // ...
     }

     // Gọi khi đổi vũ khí/skill để animator dùng đúng override controller
     public void UpdateAnimatorController(RuntimeAnimatorController controller)
     {
         animator.runtimeAnimatorController = controller;
     }

     public void CacheAnimationHashes() {
          // Có thể cần cập nhật lại Hash nếu Override Controller thay đổi cấu trúc state
          Debug.Log("Animation Hashes potentially need recaching if states changed.");
     }

     public float GetCurrentAnimationLength()
     {
         AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Layer 0
         return stateInfo.length;
     }

     // --- Animation Event Calls ---
     // Các hàm này sẽ được gọi từ Animation Event đặt trên các clip animation
     public void AE_ActivateHitbox() {
         combat?.ActivateCurrentAttackHitbox();
     }
     public void AE_DeactivateHitbox() {
          combat?.DeactivateCurrentAttackHitbox();
     }
     public void AE_AttackFinished() {
           combat?.OnAttackAnimationEnd();
          // Có thể reset combo ở đây nếu muốn chặt chẽ theo animation
           //GetComponent<ComboComponent>()?.ResetCombo();
     }
     public void AE_StepSFX() {
          // Play footstep sound
     }
     public void AE_EnableRootMotion() {
         animator.applyRootMotion = true;
     }
     public void AE_DisableRootMotion() {
          animator.applyRootMotion = false;
     }
}