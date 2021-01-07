using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    float horizontalMove;
    float verticalMove;
    [SerializeField] float normalSpeed = 0.5f;
    [SerializeField] GameObject childSprite = null;
    float speed;
    bool dead = false;

    [Header("Dash")]
    [SerializeField] float dashDuration = 0.2f; // somente do ataque e não do charge
    [SerializeField] float dashSpeed = 3f; // vel do personagem qdo ataca
    [SerializeField] float chargeDuration = 0f; // tempo de charge
    [SerializeField] float maxChargeDuration = 0f; //max tempo até um ataque automatico
    [SerializeField] float speedWhileDashing = 0f;
    bool charged = false;
    float mouseHold = 0f;
    Vector3 dashDirection = Vector3.zero;
    bool dashing = false;
    bool canDash = false;

    [Header("Collision")]
    [SerializeField] float raycastSize = 0.1f;
    [SerializeField] LayerMask wall = default;
    [SerializeField] LayerMask mixedLayer = default; // wall + enemy + moveis
    bool hitUp = false; // se não pode ir pra cima == atingiu algo acima
    bool hitDown = false;
    bool hitRight = false;
    bool hitLeft = false;
    RaycastHit2D[] emptyArray = new RaycastHit2D[1];

    Animator animator = null;

    LevelConfig levelConfig;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("chargeSpeed", 1 / chargeDuration);
        animator.SetFloat("attackSpeed", 1 / dashDuration);
        speed = normalSpeed;
        levelConfig = FindObjectOfType<LevelConfig>();
        transform.position = levelConfig.GetInitialPos();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        if (!dashing)
        {
            PlayerRotation();
            Attack();
            Move();
        }
        else
        {
            Dash();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (dashing && collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collider.GetComponent<Enemy>().Die();
        }
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            animator.SetTrigger("shock");
            SoundManager.PlaySound("player_death");
            StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<SceneLoader>().ReloadScene();
    }

    // change from charge animation
    private void AttackCharged()
    {
        charged = true;
        SoundManager.PlaySound("attack_charge");
    }

    private void Attack()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            speed = speedWhileDashing;
            canDash = true;
            animator.SetTrigger("charge");
        }
        if (canDash)
        {
            if (Input.GetMouseButton(0))
            {
                mouseHold += Time.deltaTime;

            }
            if (Input.GetMouseButtonUp(0) || mouseHold >= maxChargeDuration)
            {
                if (charged)
                {
                    Vector3 attackDirection = childSprite.transform.up * (dashSpeed * dashDuration + 0.1f);
                    attackDirection = transform.position + attackDirection;
                    RaycastHit2D predictAttack = Physics2D.Linecast(transform.position, attackDirection, mixedLayer);
                    if (predictAttack.collider != null && predictAttack.collider.CompareTag("Enemy"))
                    {
                        animator.SetTrigger("attack");
                    }
                    else
                    {
                        animator.SetTrigger("attackBloodless");
                    }
                    StartCoroutine(DashRoutine());
                }
                else
                {
                    animator.SetTrigger("stoppedCharge");
                }
                charged = false;
                mouseHold = 0f;
                canDash = false;
                speed = normalSpeed;
            }
        }
        
    }

    IEnumerator DashRoutine()
    {
        dashing = true;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        dashDirection = new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y, 0).normalized;
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        dashDirection = Vector3.zero;
    }

    private void Dash()
    {
        Vector3 dashDirectionThisFrame = dashDirection;
        if (dashDirection.y > 0 && hitUp)
        {
            dashDirectionThisFrame.y = 0;
        }
        if (dashDirection.y < 0 && hitDown)
        {
            dashDirectionThisFrame.y = 0;
        }
        if (dashDirectionThisFrame.x > 0 && hitRight)
        {
            dashDirectionThisFrame.x = 0;
        }
        else if (dashDirection.x < 0 && hitLeft)
        {
            dashDirectionThisFrame.x = 0;
        }

        if (!dead)
            transform.position += dashDirectionThisFrame * dashSpeed * Time.deltaTime;
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.W) && !hitUp)
        {
            verticalMove = 1; 
        }
        else if (Input.GetKey(KeyCode.S) && !hitDown)
        {
            verticalMove = -1;
        }
        else
        {
            verticalMove = 0;
        }
        if (Input.GetKey(KeyCode.D) && !hitRight)
        {
            horizontalMove = 1;
        }
        else if (Input.GetKey(KeyCode.A) && !hitLeft)
        {
            horizontalMove = -1;
        }
        else
        {
            horizontalMove = 0;
        }
        
        Vector3 movePosition = new Vector3(horizontalMove, verticalMove, 0).normalized;
        if (movePosition == Vector3.zero)
        {
            animator.SetBool("walking", false);
        }
        else
        {
            animator.SetBool("walking", true);
        }

        if (!dead)
            transform.position += movePosition * speed * Time.deltaTime;
    }

    private void CheckCollision()
    {
        LayerMask layer = dashing ? wall : mixedLayer;

        float vertX = raycastSize / Mathf.Tan(Mathf.Deg2Rad * 50);
        Vector2 up_left = new Vector2(-vertX, raycastSize);
        Vector2 up_right = new Vector2(vertX, raycastSize);
        Vector2 up_normal = new Vector2(0, raycastSize);
        Vector2 down_left = new Vector2(-vertX, -raycastSize);
        Vector2 down_right = new Vector2(vertX, -raycastSize);
        Vector2 down_normal = new Vector2(0, -raycastSize);
        float horiY = raycastSize * Mathf.Tan(Mathf.Deg2Rad * 40);
        Vector2 left_top = new Vector2(-raycastSize, horiY);
        Vector2 left_bottom = new Vector2(-raycastSize, -horiY);
        Vector2 left_normal = new Vector2(-raycastSize, 0);
        Vector2 right_top = new Vector2(raycastSize, horiY);
        Vector2 right_bottom = new Vector2(raycastSize, -horiY);
        Vector2 right_normal = new Vector2(raycastSize, 0);

        int hit_up_left = Physics2D.RaycastNonAlloc(transform.position, up_left, emptyArray, raycastSize, layer);
        int hit_up_right = Physics2D.RaycastNonAlloc(transform.position, up_right, emptyArray, raycastSize, layer);
        int hit_up_normal = Physics2D.RaycastNonAlloc(transform.position, up_normal, emptyArray, raycastSize, layer);
        int hit_down_left = Physics2D.RaycastNonAlloc(transform.position, down_left, emptyArray, raycastSize, layer);
        int hit_down_right = Physics2D.RaycastNonAlloc(transform.position, down_right, emptyArray, raycastSize, layer);
        int hit_down_normal = Physics2D.RaycastNonAlloc(transform.position, down_normal, emptyArray, raycastSize, layer);
        int hit_left_top = Physics2D.RaycastNonAlloc(transform.position, left_top, emptyArray, raycastSize, layer);
        int hit_left_bottom = Physics2D.RaycastNonAlloc(transform.position, left_bottom, emptyArray, raycastSize, layer);
        int hit_left_normal = Physics2D.RaycastNonAlloc(transform.position, left_normal, emptyArray, raycastSize, layer);
        int hit_right_top = Physics2D.RaycastNonAlloc(transform.position, right_top, emptyArray, raycastSize, layer);
        int hit_right_bottom = Physics2D.RaycastNonAlloc(transform.position, right_bottom, emptyArray, raycastSize, layer);
        int hit_right_normal = Physics2D.RaycastNonAlloc(transform.position, right_normal, emptyArray, raycastSize, layer);

        if (hit_up_left != 0 || hit_up_right != 0 || hit_up_normal != 0)
        {
            hitUp = true;
        }
        else
        {
            hitUp = false;
        }

        if (hit_down_left != 0 || hit_down_right != 0 || hit_down_normal != 0)
        {
            hitDown = true;
        }
        else
        {
            hitDown = false;
        }

        if (hit_left_top != 0 || hit_left_bottom != 0 || hit_left_normal != 0)
        {
            hitLeft = true;
        }
        else
        {
            hitLeft = false;
        }

        if (hit_right_top != 0 || hit_right_bottom != 0 || hit_right_normal != 0)
        {
            hitRight = true;
        }
        else
        {
            hitRight = false;
        }
        
        /*
        Debug.DrawRay(transform.position, up_left);
        Debug.DrawRay(transform.position, up_right);
        Debug.DrawRay(transform.position, up_normal);
        Debug.DrawRay(transform.position, down_left);
        Debug.DrawRay(transform.position, down_right);
        Debug.DrawRay(transform.position, down_normal);
        Debug.DrawRay(transform.position, left_top);
        Debug.DrawRay(transform.position, left_bottom);
        Debug.DrawRay(transform.position, left_normal);
        Debug.DrawRay(transform.position, right_top);
        Debug.DrawRay(transform.position, right_bottom);
        Debug.DrawRay(transform.position, right_normal);
        */
    }

    private void PlayerRotation()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        if (!dead)
            childSprite.transform.up = direction;
    }

    private void PlaySound(string soundEffect)
    {
        switch (soundEffect)
        {
            case "attack_made":
                SoundManager.PlaySound("attack_made");
                break;
            case "attack_blood":
                SoundManager.PlaySound("attack_blood");
                break;
            case "attack_charge":
                SoundManager.PlaySound("attack_charge");
                break;
            case "player_death":
                SoundManager.PlaySound("player_death");
                break;
            case "steps":
                SoundManager.PlaySound("steps");
                break;
            default:
                break;
        }
    }
}
