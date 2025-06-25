using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Atributos")]
    public CharacterController controller;
    public PlayerManager getPlayer;
    public PlayerCam cam;
    public Image StaminaBar;
    public GameObject staminaUI;

    public Vector3 velocity;
    public bool isGrounded;
    public Vector2 InputMove;
    public float timeDelay;

    Coroutine rechargeCoroutine;
    public PlayerBaseState currentState { get; private set; }

    [Header("Estados")]
    public IdleState idleState = new IdleState();
    public WalkingState walkingState = new WalkingState();
    public RunningState runningState = new RunningState();
    public CrouchingState crouchingState = new CrouchingState();
    public JumpingState jumpingState = new JumpingState();
    public InteractingState interactingState = new InteractingState();

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponent<PlayerCam>();
    }

    void Start()
    {
        ShowStaminaUI(false);
        SwitchState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        InputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Inputs de ir nos eixos x e z
        cam.camMove(); //Movimentação da câmera

        currentState?.UpdateState(this);
    }
    public void SwitchState(PlayerBaseState newState)
    {
        if (currentState == newState) return;
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
    public void Move(Vector2 input, float speed)
    {
        Vector3 move = cam.cam.right * input.x + cam.cam.forward * input.y;
        move.y = 0f;
        controller.Move(move.normalized * speed * Time.deltaTime);
    }
    public void Gravity()
    {
        velocity.y += getPlayer.gravity * Time.deltaTime; // Aplica a gravidade manualmente no eixo Y
        controller.Move(velocity * Time.deltaTime); // Move o personagem para baixo (gravidade)
    }
    public void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Usa uma esfera invisível na posição do groundCheck para verificar se o personagem está tocando o chão

        // Se estiver no chão e estiver caindo, força o personagem a ficar levemente colado no chão
        if (isGrounded && velocity.y < 0) velocity.y = -2f; // Garante que o personagem "grude" no chão e não fique flutuando
    }
    public void ShowStaminaUI(bool show) => staminaUI.SetActive(show); // Ativa ou desativa a barra de stamina

    public void UpdateStaminaBar() => StaminaBar.fillAmount = getPlayer.stamina / getPlayer.staminaMax;

    public void StartRechargeStamina()
    {
        if (rechargeCoroutine != null) StopCoroutine(rechargeCoroutine);
        rechargeCoroutine = StartCoroutine(RechargeStamina());
    }
    IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(timeDelay);
        float timeRecharge = 5;
        while (getPlayer.stamina < getPlayer.staminaMax)
        {
            if (Input.GetKey(KeyCode.LeftShift)) yield break; // cancela se tentar correr

            getPlayer.stamina += timeRecharge / 3;
            if (getPlayer.stamina > getPlayer.staminaMax) getPlayer.stamina = getPlayer.staminaMax;
            StaminaBar.fillAmount = getPlayer.stamina / getPlayer.staminaMax;
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(timeDelay);
        ShowStaminaUI(false);
    }
    void OnDrawGizmos()
    {
        // Define a cor da gizmo
        Gizmos.color = Color.red;

        // Desenha uma esfera na posição do groundCheck com o mesmo raio
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
