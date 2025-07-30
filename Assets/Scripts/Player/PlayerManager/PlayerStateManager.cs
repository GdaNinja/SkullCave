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
    public Image StaminaBK;
    public Image StaminaBorder;
    public GameObject staminaUI;
    public Transform playerPivot;
    public AudioSource walikingSound;
    public AudioSource runningSound;
    public Animator animator;

    public Vector3 velocity;
    #region Váriaveis booleanas
    public bool isGrounded;
    public bool isCrouching;
    public bool isRecharging; //A barra de estamina está carregando ou não
    public bool isRunning;
    public bool isBlinking; //A barra de estamina está piscando ou não
    public bool staminaBarActive;
    #endregion
    public Vector2 InputMove;
    public float timeDelay; // Delay pra poder carregar a barra de estamina
    public float blinkSpd;
    Coroutine rechargeCoroutine;

    public PlayerBaseState currentState { get; private set; }

    [Header("Estados")]
    public IdleState idleState = new IdleState();
    public WalkingState walkingState = new WalkingState();
    public RunningState runningState = new RunningState();
    public CrouchingState crouchingState = new CrouchingState();
    public JumpingState jumpingState = new JumpingState();

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Color standardColor = new Color(1, 1, 1, 1);
    [SerializeField] Color lowStaminaColor = new Color(1, 0, 0, 1);

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponent<PlayerCam>();
        animator = GetComponentInChildren<Animator>();
        
    }

    void Start()
    {
        staminaUI.SetActive(false);
        SwitchState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        InputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Inputs de ir nos eixos x e z
        cam.camMove(); //Movimenta��o da c�mera
        float speed = InputMove.magnitude; // de 0 a 1 com base no input
        animator.SetFloat("Speed", speed);
        animator.SetBool("isRunning", isRunning);

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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Usa uma esfera invis�vel na posi��o do groundCheck para verificar se o personagem est� tocando o ch�o

        // Se estiver no ch�o e estiver caindo, for�a o personagem a ficar levemente colado no ch�o
        if (isGrounded && velocity.y < 0) velocity.y = -2f; // Garante que o personagem "grude" no ch�o e n�o fique flutuando
    }
    public bool CanRun()
    {
        return Input.GetKey(KeyCode.LeftShift) && getPlayer.stamina > 0 && !isBlinking;
    }
    public void ShowStaminaUI(bool show)
    {
        if (staminaBarActive == show) return; // Se a barra de estamina estiver ativa for verdadeira e show(o fade in/fade out da barra de estamina também for verdadeira, o códgo para aqui)
        staminaBarActive = show;
        StartCoroutine(showStamina(show));
    }

    public void UpdateStaminaBar()
    {
        StaminaBar.fillAmount = getPlayer.stamina / getPlayer.staminaMax;
    }

    public void StartRechargeStamina()
    {
        if (getPlayer.stamina <= 0)
        {
            StaminaBar.color = lowStaminaColor;
            StaminaBorder.color = lowStaminaColor;
            if (!isBlinking) StartCoroutine(BlinkStaminaBar());
        }
        if (!isRecharging)
        {
            if (rechargeCoroutine != null) StopCoroutine(rechargeCoroutine);
            rechargeCoroutine = StartCoroutine(RechargeStamina());
        }
    }
    IEnumerator showStamina(bool show)
    {
        float duration = 1f; // Tempo total do fade
        float time = 0f;

        // Captura os valores de alpha iniciais
        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;

        // Se for mostrar, ativa antes de começar o fade in
        if (show) staminaUI.SetActive(true);

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);

            StaminaBar.color = new Color(StaminaBar.color.r, StaminaBar.color.g, StaminaBar.color.b, alpha);
            StaminaBK.color = new Color(StaminaBK.color.r, StaminaBK.color.g, StaminaBK.color.b, alpha);
            StaminaBorder.color = new Color(StaminaBorder.color.r, StaminaBorder.color.g, StaminaBorder.color.b, alpha);

            yield return null;
        }

        // Garante valor final preciso
        float finalAlpha = endAlpha;
        StaminaBar.color = new Color(StaminaBar.color.r, StaminaBar.color.g, StaminaBar.color.b, finalAlpha);
        StaminaBK.color = new Color(StaminaBK.color.r, StaminaBK.color.g, StaminaBK.color.b, finalAlpha);
        StaminaBorder.color = new Color(StaminaBorder.color.r, StaminaBorder.color.g, StaminaBorder.color.b, finalAlpha);

        // Se for esconder, desativa depois do fade out
        if (!show) staminaUI.SetActive(false);

    }
    IEnumerator RechargeStamina()
    {
        isRecharging = true;

        yield return new WaitForSeconds(timeDelay);
        float timeRecharge = 5;
        while (getPlayer.stamina < getPlayer.staminaMax)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !isBlinking && InputMove != Vector2.zero)
            {
                isRecharging = false;
                yield break; // cancela se tentar correr
            }
            getPlayer.stamina += timeRecharge / 3;
            if (getPlayer.stamina > getPlayer.staminaMax) getPlayer.stamina = getPlayer.staminaMax;
            StaminaBar.fillAmount = getPlayer.stamina / getPlayer.staminaMax;
            yield return new WaitForSeconds(.1f);
        }
        if (getPlayer.stamina >= getPlayer.staminaMax)
        {
            isBlinking = false;
            StaminaBar.color = standardColor;
            StaminaBorder.color = standardColor;
        }
        yield return new WaitForSeconds(timeDelay);
        if (!CanRun() || getPlayer.stamina >= getPlayer.staminaMax)
        {
            ShowStaminaUI(false);
        }
        isRecharging = false;
    }
    public IEnumerator BlinkStaminaBar()
    {
        isBlinking = true;
        float timeBlink = 0f;
        bool fadeOut = true;

        while (getPlayer.stamina < getPlayer.staminaMax)
        {
            timeBlink += Time.deltaTime * blinkSpd;
            Debug.Log(timeBlink);
            float alpha = fadeOut ? Mathf.Lerp(1f, 0.3f, timeBlink) : Mathf.Lerp(0.3f, 1f, timeBlink); // Ponto de interrogação: if/else. Dois Pontos: Se a condição for verdadeira retorna antes dos dois pontos, senão só depois

            StaminaBar.color = new Color(StaminaBar.color.r, StaminaBar.color.g, StaminaBar.color.b, alpha);
            StaminaBorder.color = new Color(StaminaBorder.color.r, StaminaBorder.color.g, StaminaBorder.color.b, alpha);

            if (timeBlink >= 1f)
            {
                fadeOut = !fadeOut;
                timeBlink = 0f;
            }
            yield return null; // Espera um frame pra voltar a agir
        }
    }
    void OnTriggerStay(Collider other)
    {

    }
    void OnDrawGizmos()
    {
        // Define a cor da gizmo
        Gizmos.color = Color.red;

        // Desenha uma esfera na posi��o do groundCheck com o mesmo raio
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}