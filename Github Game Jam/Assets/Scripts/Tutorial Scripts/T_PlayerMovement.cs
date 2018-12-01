using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float speed, jumpForceHigh, jumpForceLow, maxTapDelay = 0.3f, maxImmunityTime = 2f, respawnTimer = 0;
    float direction;
    bool isGround;
    public LayerMask Ground;
    GridSize gs;
    float tapTimer = 0, immunityTimer = 10f;
    public bool canJump = false, canDoubleJump = false, canRespawn = false, canMove = true, disableRespawn = false;
    bool timerOn = false, doubleJump = false,  immunity = false;
    [SerializeField] GameObject playerIndicator;
    Vector3 maxH;
    Animator indicatorAnim, playerAnim;
    [SerializeField] Animator poofAnim;
    public Material mat1, mat2;
    SoundManager sm;
    // Use this for initialization
    void Start()
    {
        Invoke("LateStart", Time.deltaTime);
        playerAnim = GetComponent<Animator>();
        indicatorAnim = playerIndicator.GetComponent<Animator>();
        playerIndicator.GetComponent<Renderer>().enabled = false;
        maxH = 5 * Vector3.up;
        rb2d = GetComponent<Rigidbody2D>();
        gs = FindObjectOfType<GridSize>();
    }
    void LateStart()
    {
        sm = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!T_LevelController.freezeGame)
        {
            RespawnControl();
            Movement();
            Jump();
            Respawn();
            ImmunityControl();
            PoofFollow();
        }
    }

    void FixedUpdate()
    {
        if (!SnapMovement.gameOver)
        {
            isGrounded();
        }
    }

    void Movement()
    {
        direction = Input.GetAxisRaw("Horizontal");
        if(canMove)
            rb2d.velocity = new Vector2(direction * speed, rb2d.velocity.y);
        playerAnim.SetFloat("Velocity", rb2d.velocity.x);
        if (Mathf.Abs(transform.position.x) > (GridSize.scale * (gs.gridWidth - 0.5f) / 2f))
        {
            transform.position = new Vector3(transform.position.x / Mathf.Abs(transform.position.x) * (GridSize.scale * (gs.gridWidth - 0.5f) / 2f), transform.position.y);
        }
    }

    void Jump()
    {
        StartTimer();
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true && !timerOn && canJump)
        {
            sm.PlaySFX(sm.audioList[6]);
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(new Vector2(0, jumpForceLow), ForceMode2D.Impulse);
            timerOn = true;
        }
        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space) && doubleJump == false && tapTimer > 0 && tapTimer < maxTapDelay && transform.position.y < maxH.y)
        {
            doubleJump = true;
            timerOn = false;
            tapTimer = 0;
        }
        if (tapTimer >= maxTapDelay)
        {
            timerOn = false;
            tapTimer = 0;
        }
        if (doubleJump && rb2d.velocity.y <= 0)
        {
            sm.PlaySFX(sm.audioList[7]);
            FindObjectOfType<T_LevelController>().dj = true;
            poofAnim.SetBool("DoubleJump", true);
            doubleJump = false;
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(new Vector2(0, jumpForceHigh), ForceMode2D.Impulse);
        }
    }

    void StartTimer()
    {
        if (timerOn)
            tapTimer += Time.deltaTime;
    }

    void isGrounded()
    {
        RaycastHit2D R1 = Physics2D.Raycast(transform.position + Vector3.right * GridSize.scale / 4, Vector2.down, 0.4f, Ground);
        RaycastHit2D R2 = Physics2D.Raycast(transform.position - Vector3.right * GridSize.scale / 4, Vector2.down, 0.4f, Ground);
        if (R1 || R2)
        {
            isGround = true;
            poofAnim.SetBool("DoubleJump", false);
            rb2d.gravityScale = 3.0f;
        }
        else
        {
            isGround = false;
        }
        playerAnim.SetBool("IsGrounded", isGround);
    }

    void Respawn()
    {
        if (Input.GetKey(KeyCode.R) && canRespawn && !disableRespawn)
        {
            Spawn(true);
        }
    }

    public void MakeImmune()
    {
        immunityTimer = 0;
        StartCoroutine(BlinkPlayer());
    }

    void ImmunityControl()
    {
        immunityTimer += Time.deltaTime;
        if (immunityTimer < maxImmunityTime)
            immunity = true;
        else
            immunity = false;
    }

    void RespawnControl()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer > 3)
            canRespawn = true;
        else
            canRespawn = false;
    }


    void PoofFollow()
    {
        if (!poofAnim.GetBool("DoubleJump"))
            poofAnim.gameObject.transform.position = transform.position + Vector3.down * 0.2f;
    }

    public IEnumerator BlinkPlayer()
    {
        int blinkCount = 0;
        while (blinkCount < 6)
        {
            GetComponent<Renderer>().material = mat1;
            blinkCount++;

            yield return new WaitForSeconds(maxImmunityTime / 14);
            if (blinkCount < 5)
            {
                GetComponent<Renderer>().material = mat2;
                yield return new WaitForSeconds(maxImmunityTime / 14);
            }
        }
        GetComponent<Renderer>().material = mat1;
    }

    public void Spawn(bool DecayHp)
    {
        if (DecayHp && !immunity)
        {
            HealthSystem.hp--;
            sm.PlaySFX(sm.audioList[4]);
        }
        if (HealthSystem.hp == 0)
        {
            HealthSystem.hp = 6;
        }
        else
        {
            MakeImmune();
            transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
            rb2d.velocity = Vector3.zero;
            rb2d.gravityScale = 1f;
            canRespawn = false;
            indicatorAnim.SetBool("Play", true);
            playerIndicator.GetComponent<Renderer>().enabled = true;
            playerIndicator.transform.position = new Vector3(transform.position.x, maxH.y - 0.5f);
            respawnTimer = 0;
        }
    }
}
