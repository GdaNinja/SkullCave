using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] PlayerManager getPlayer;
    [SerializeField] PlayerCam getCam;
    [SerializeField] CharacterController characterController; // Referência ao componente CharacterController, que controlará o movimento
    [SerializeField] GameObject stamina;

    Vector3 velocity;
    Coroutine recharge;
    public Image StaminaBar;
    public float timeDelay;

    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck; // Transform usado para checar se o jogador está tocando o chão
    [SerializeField] float groundDistance = 0.4f; // Raio da esfera usada para detectar o chão
    [SerializeField] LayerMask groundMask; // Máscara de camada usada para identificar o que é "chão"
    void Awake()
    {
        stamina.SetActive(false);
        characterController = GetComponent<CharacterController>();
        getCam = GetComponent<PlayerCam>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        Move();
        Jump();
    }
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        getCam.camMove();
        // Pega a direção da câmera ignorando a inclinação vertical
        Vector3 camForward = getCam.cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = getCam.cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = camRight * x + camForward * z; // Move com base na câmera, não no transform do player
        float speed = getPlayer.spd;

        if (Input.GetKey(KeyCode.LeftShift) && getPlayer.stamina > 0)
        {
            stamina.SetActive(true);
            speed = getPlayer.runSpd;

            if (x != 0 || z != 0) // só gasta se estiver se movendo
            {
                getPlayer.stamina -= 10f * Time.deltaTime;
                if (getPlayer.stamina < 0) getPlayer.stamina = 0;
                StaminaBar.fillAmount = getPlayer.stamina / getPlayer.staminaMax;

                if (recharge != null) StopCoroutine(recharge);
                recharge = StartCoroutine(RechargeStamina());
            }
        }

        characterController.Move(move.normalized * speed * Time.deltaTime); // Move o personagem na direção desejada com base na velocidade

        velocity.y += getPlayer.gravity * Time.deltaTime; // Aplica a gravidade manualmente no eixo Y

        characterController.Move(velocity * Time.deltaTime); // Move o personagem para baixo (gravidade)
    }
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Usa uma esfera invisível na posição do groundCheck para verificar se o personagem está tocando o chão

        // Se estiver no chão e estiver caindo, força o personagem a ficar levemente colado no chão
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Garante que o personagem "grude" no chão e não fique flutuando
        }
    }
    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(getPlayer.jumpForce * -2f * getPlayer.gravity); // Calcula a força de pulo com base na fórmula de física: v = sqrt(2gh)
            Debug.Log("PULO! Velocity.y = " + velocity.y);
        }
    }
    void OnDrawGizmos()
    {
        // Define a cor da gizmo
        Gizmos.color = Color.red;

        // Desenha uma esfera na posição do groundCheck com o mesmo raio
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
    IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(timeDelay);
        float timeRecharge = 5;
        while (getPlayer.stamina < getPlayer.staminaMax)
        {
            if (Input.GetKey(KeyCode.LeftShift)) yield break; // cancela se tentar correr

            getPlayer.stamina += timeRecharge/3;
            if (getPlayer.stamina > getPlayer.staminaMax) getPlayer.stamina = getPlayer.staminaMax;
            StaminaBar.fillAmount = getPlayer.stamina / getPlayer.staminaMax;
            yield return new WaitForSeconds(.1f);
        }
            yield return new WaitForSeconds(timeDelay);
            stamina.SetActive(false);
    }
}
