using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputs playerInputs;
    public Rigidbody rb;
    private Animator animator;
    public float moveSpeed;
    public float sprintMultiplier;
    public float threshold;
    public float mouseSensetivity;

    private float rotationVelocity;
    private bool isSprinting;
    private bool isAttacking;
    private InputAction move;
    private InputAction meleeAtack;
    private InputAction run;
    private InputAction look;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        move = playerInputs.Player.Move;
        move.Enable();

        meleeAtack = playerInputs.Player.MeleeAttack;
        meleeAtack.Enable();
        meleeAtack.performed += MeleeAttack;

        run = playerInputs.Player.Run;
        run.Enable();
        run.performed += RunPerformed;
        run.canceled += RunCanceled;

        look = playerInputs.Player.Look;
        look.Enable();
        look.performed += Look;
    }

    private void OnDisable()
    {
        move.Disable();
        meleeAtack.Disable();
        run.Disable();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 moveDirection = move.ReadValue<Vector2>();
        Vector3 playerMovementInput = new Vector3(moveDirection.x, 0, moveDirection.y);

        if (isAttacking) { return; } // do nothing

        if (isSprinting)
        {
            Vector3 moveVector = transform.TransformDirection(playerMovementInput) * moveSpeed * sprintMultiplier;
            rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        }
        else
        {
            Vector3 moveVector = transform.TransformDirection(playerMovementInput) * moveSpeed;
            rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        }


        if (moveDirection != Vector2.zero) { animator.SetBool("InMotion", true); }
        else { animator.SetBool("InMotion", false); }
    }

    private void MeleeAttack(InputAction.CallbackContext context)
    {

        if (isAttacking) { return; }
        animator.SetTrigger("MeleeAttack");
        StartCoroutine(WaitingForAnimAttackEnd());
    }

    private void RunPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
        animator.SetBool("IsPressedShift", true);
    }

    private void RunCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        animator.SetBool("IsPressedShift", false);
    }

    private void Look(InputAction.CallbackContext context)
    {
        print("kek");
        if(look.ReadValue<Vector2>().sqrMagnitude >= threshold)
        {
            rotationVelocity = look.ReadValue<Vector2>().x * mouseSensetivity * Time.deltaTime;
            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }

    IEnumerator WaitingForAnimAttackEnd()
    {
        isAttacking = true;
        rb.velocity = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(2.05f);
        isAttacking = false;
    }
}
