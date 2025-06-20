using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] PlayerSO getPlayer;
    [SerializeField] Vector3 move;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isRun;
    Rigidbody rbPlayer;
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        CheckGround();
        Jump();
    }
    private void FixedUpdate()
    {
        Move();
        Run();
    }
    void Inputs()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        move = (transform.right * x + transform.forward * z).normalized * getPlayer.spd;
    }
    void Move()
    {
        rbPlayer.velocity = new Vector3 (move.x, rbPlayer.velocity.y, move.z);
    }
    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }
    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rbPlayer.AddForce(Vector3.up * getPlayer.jumpForce, ForceMode.Impulse);
        }
    }
    void Run()
    {
        float originalSpd = getPlayer.spd;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
            getPlayer.spd = getPlayer.runSpd;
        }
        else
        {
            isRun = false;
        }
    }
}
