using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] List<Transform> waypoints = null;
    [SerializeField] float normalSpeed = 0f;
    [SerializeField] float angleSpeed = 360f;
    [SerializeField] float angleSpeedCharge = 180f;
    [SerializeField] float closestDistance = 0.3f;
    [SerializeField] bool noMove = false;
    int waypointIndex = 0;
    float speed;
    Vector2 targetVector;
    Quaternion targetRotation;
    Quaternion targetRotationCount1;


    [Header("Vision")]
    [SerializeField] float visionAngleDegFar = 60f;
    [SerializeField] float visionDistanceFar = 4f;
    [SerializeField] float visionAngleDegClose = 60f;
    [SerializeField] float visionDistanceClose = 4f;
    [SerializeField] LayerMask wall = default;
    Animator animator;
    Player player = null;

    [Header("Attack")]
    [SerializeField] float chargeTime = 2f;
    [SerializeField] float maxShotDistance = 2f;
    [SerializeField] GameObject taser = null;
    [SerializeField] float taserSpeed = 1f;
    [SerializeField] LayerMask playerLayer = default;
    [SerializeField] float taserOffsetFromPlayer = 0.05f;
    bool charging = false;
    bool hasShot = false;
    Vector2 lastPlayerPosition;
    Vector2 taserInitialPoint;

    [Header("TaserLine")]
    [SerializeField] GameObject taserLine = null;

    // Start is called before the first frame update
    void Start()
    {
        // Taser
        taser.SetActive(false);
        taserLine.transform.position = transform.position + (transform.up * taserOffsetFromPlayer);
        taserLine.SetActive(false);
        
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        animator.SetFloat("chargeSpeed", 1 / chargeTime);

        // movement related
        if (!noMove)
        {
            if (waypoints.Count != 1)
            {
                animator.SetBool("move", true);
                transform.position = waypoints[waypointIndex].position;
                var currentWaypoint = waypoints[waypointIndex].position;
                var newWaypoint = waypoints[waypointIndex + 1].position;
                transform.up = new Vector2(newWaypoint.x - currentWaypoint.x, newWaypoint.y - currentWaypoint.y);
                targetVector = transform.up;
                targetRotation = Quaternion.LookRotation(transform.forward, targetVector);
            }
            else
            {
                animator.SetBool("move", false);
                transform.position = waypoints[waypointIndex].position;
                Vector2 targetVectorCount1 = transform.up;
                targetRotationCount1 = Quaternion.LookRotation(transform.forward, targetVectorCount1); ;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (!hasShot)
            DetectPlayer();
        else
            Shoot();
    }

    public void Die()
    {
        animator.SetTrigger("die");
        SoundManager.PlaySound("attack_blood");
        SoundManager.PlaySound("enemy_death");
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        taser.SetActive(false);
        taserLine.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        FindObjectOfType<LevelConfig>().DecreaseEnemyCount();
        enabled = false;
    }

    // called in the end of animation enemy_charge
    private void ActivateShoot()
    {
        hasShot = true;
        taser.SetActive(true);
        taserLine.SetActive(true);
        taserLine.transform.localScale = new Vector3(1, 0, 1);
        taser.transform.position = transform.position + (transform.up * taserOffsetFromPlayer);
        taserInitialPoint = taser.transform.position;

        Vector2 aux = new Vector2(player.transform.position.x - taser.transform.position.x, player.transform.position.y - taser.transform.position.y).normalized * maxShotDistance;
        lastPlayerPosition = (Vector2) taser.transform.position + aux;
        SoundManager.PlaySound("taser");
    }

    private void Shoot()
    {
        var movementThisFrame = taserSpeed * Time.deltaTime;
        taser.transform.position = Vector2.MoveTowards(taser.transform.position, lastPlayerPosition, movementThisFrame);
        float distance = Vector2.Distance(taserInitialPoint, (Vector2)taser.transform.position);
        taserLine.transform.localScale = new Vector3(1, 2f * distance, 1); // 2f baseado no tamanho de 50 pixeis do sprite da linha
        Collider2D hitPlayer = Physics2D.OverlapPoint(taser.transform.position, playerLayer);
        Debug.DrawLine(taser.transform.position, lastPlayerPosition);
        if (hitPlayer)
        {
            player.Die();
            enabled = false;
        }
        else if ((Vector2) taser.transform.position == lastPlayerPosition)
        {
            taser.SetActive(false);
            taserLine.SetActive(false);
            hasShot = false;
            animator.SetTrigger("stoppedCharge");
            charging = false;
        }
    }

    private void DetectPlayer()
    {
        Vector2 vectorFromToPlayer = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        float angle = Vector2.Angle(transform.up, vectorFromToPlayer);
        float distance = vectorFromToPlayer.magnitude;

        // debug
        /*
        Vector2 debugVector0 = transform.up * visionDistanceFar;
        float debugAngle0 = Mathf.Deg2Rad * visionAngleDegFar / 2;
        Vector2 debugVector1 = new Vector2(debugVector0.x * Mathf.Cos(debugAngle0) - debugVector0.y * Mathf.Sin(debugAngle0),
                                           debugVector0.y * Mathf.Cos(debugAngle0) + debugVector0.x * Mathf.Sin(debugAngle0));
        Vector2 debugVector2 = new Vector2(debugVector0.x * Mathf.Cos(-debugAngle0) - debugVector0.y * Mathf.Sin(-debugAngle0),
                                           debugVector0.y * Mathf.Cos(-debugAngle0) + debugVector0.x * Mathf.Sin(-debugAngle0));
        Debug.DrawRay(transform.position, debugVector1, Color.green);
        Debug.DrawRay(transform.position, debugVector2, Color.green);

        Vector2 debugVector3 = transform.up * visionDistanceClose;
        float debugAngle3 = Mathf.Deg2Rad * visionAngleDegClose / 2;
        Vector2 debugVector4 = new Vector2(debugVector3.x * Mathf.Cos(debugAngle3) - debugVector3.y * Mathf.Sin(debugAngle3),
                                           debugVector3.y * Mathf.Cos(debugAngle3) + debugVector3.x * Mathf.Sin(debugAngle3));
        Vector2 debugVector5 = new Vector2(debugVector3.x * Mathf.Cos(-debugAngle3) - debugVector3.y * Mathf.Sin(-debugAngle3),
                                           debugVector3.y * Mathf.Cos(-debugAngle3) + debugVector3.x * Mathf.Sin(-debugAngle3));
        Debug.DrawRay(transform.position, debugVector4, Color.red);
        Debug.DrawRay(transform.position, debugVector5, Color.red);
        */

        if ( ((angle < visionAngleDegFar / 2) && distance < visionDistanceFar) || ((angle < visionAngleDegClose / 2) && distance < visionDistanceClose) )
        {
            RaycastHit2D hit_wall = Physics2D.Raycast(transform.position, vectorFromToPlayer, distance, wall);
            if (hit_wall.collider == null)
            {
                Debug.DrawRay(transform.position, vectorFromToPlayer);
                if (!charging)
                {
                    animator.SetTrigger("charge");
                    charging = true;
                }
            }
            else if (charging)
            {
                animator.SetTrigger("stoppedCharge");
                charging = false;
            }
        }
        else if (charging)
        {
            animator.SetTrigger("stoppedCharge");
            charging = false;
        }
    }

    private void ChangeVelocity(float multiplier)
    {
        speed = multiplier * normalSpeed;
    }

    private void Move()
    {
        var movementThisFrame = speed * Time.deltaTime;
        if (!charging)
        {
            if (!noMove)
            {
                if (waypoints.Count > 1)
                {
                    var targetPosition = waypoints[waypointIndex].position;
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angleSpeed * Time.deltaTime);
                    if (targetPosition == transform.position)
                    {
                        var currentWaypoint = waypoints[waypointIndex].position;
                        waypointIndex = (waypointIndex == waypoints.Count - 1) ? 0 : (waypointIndex + 1);
                        var newWaypoint = waypoints[waypointIndex].position;
                        targetVector = new Vector2(newWaypoint.x - currentWaypoint.x, newWaypoint.y - currentWaypoint.y);
                        targetRotation = Quaternion.LookRotation(transform.forward, targetVector);
                    }
                }
                else
                {
                    var targetPosition = waypoints[waypointIndex].position;
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotationCount1, angleSpeed * Time.deltaTime);
                }
                
            }
        }
        else if (!hasShot)
        {
            var targetPosition = player.transform.position;
            float distanceFromPlayer = Vector2.Distance(transform.position, targetPosition);
            if (distanceFromPlayer > closestDistance && !noMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            }
            Vector2 targetVectorCharge = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
            Quaternion targetRotationCharge = Quaternion.LookRotation(transform.forward, targetVectorCharge);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotationCharge, angleSpeedCharge * Time.deltaTime);
        }
        
    }
}

