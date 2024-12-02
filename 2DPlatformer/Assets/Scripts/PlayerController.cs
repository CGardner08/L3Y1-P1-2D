using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
 
{
    [Header("UI")]
    public TMP_Text timerTxt;
    public float timer;

    [Header("Health")]
    public Slider healthSlider;
    public int maxHealth;
    public int currentHealth;

    [Header("Shooting")]
    public Transform shootingPoint;
    public GameObject bullet;
    bool isFacingRight; 

    [Header("Main")]
    public float moveSpeed;
    public float jumpForce;
    float inputs;
    public Rigidbody2D rb;
    public float groundDistance;
    public LayerMask layerMask;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    public TrailRenderer tr;
   
    
    RaycastHit2D hit;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;

        healthSlider.maxValue = maxHealth;

        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }


        timer += Time.deltaTime;
        timerTxt.text = timer.ToString("F2");
        
        Movement();
        Health();
        Shoot();
        MovementDirection();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate() 
    {
        if (isDashing)
        {
            return;
        }
    }
    
    void Movement()
    {
        inputs = Input.GetAxisRaw("Horizontal");
        rb.velocity = new UnityEngine.Vector2(inputs * moveSpeed, rb.velocity.y);

        hit = Physics2D.Raycast(transform.position, -transform.up, groundDistance, layerMask);
        Debug.DrawRay(transform.position, -transform.up * groundDistance, Color.yellow);

        if (hit.collider)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    void Health()
    {
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Instantiate(bullet, shootingPoint.position, shootingPoint.rotation);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void MovementDirection()
    {
        if(isFacingRight && inputs < -.1f)
        {
            Flip();
        }
        else if (!isFacingRight && inputs > .1f)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f,0f);

        dashingPower = -dashingPower;
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            transform.position = startPos;
        }
        if (other.gameObject.CompareTag("Exit"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            currentHealth--;
            Destroy(other.gameObject);
        }
    }
}

   

