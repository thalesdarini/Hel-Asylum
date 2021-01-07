using TreeEditor;
using UnityEngine;

public class ToFollow : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float ratioDistance = 0f; // % de proximidade do player em relação ao mouse
    
    [SerializeField] float cameraSize = 1f;

    [Header("Dead Zone")]
    [Range(0f, 1f)] [SerializeField] float deadZoneWidth;
    [Range(0f, 1f)] [SerializeField] float deadZoneHeight;
    [SerializeField] bool deadZone = false;

    // CHANGE THIS: it must depend on camera size
    float pixelsPerUnit;
    
    // Variables
    float heightToWidthRatio;
    Vector2 screenCenter;
    float xOffset;
    float yOffset;

    // Cached references
    Player player;

    private void Awake()
    {
        heightToWidthRatio = (float)Screen.height / Screen.width;
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (deadZone)
        {
            pixelsPerUnit = (Screen.height / 2) / cameraSize;
            xOffset = deadZoneWidth * Screen.width / pixelsPerUnit / 2; // half the offset in world space units
            yOffset = deadZoneHeight * Screen.height / pixelsPerUnit / 2; // half the offset in world space units
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PositionToFollow();

        // draw deadZone
        /*
        Vector2 debugBottomLeft = new Vector2(player.transform.position.x - xOffset, player.transform.position.y - yOffset);
        Vector2 debugBottomRight = new Vector2(player.transform.position.x + xOffset, player.transform.position.y - yOffset);
        Vector2 debugUpperLeft = new Vector2(player.transform.position.x - xOffset, player.transform.position.y + yOffset);
        Vector2 debugUpperRight = new Vector2(player.transform.position.x + xOffset, player.transform.position.y + yOffset);
        Debug.DrawLine(debugBottomLeft, debugBottomRight, Color.green);
        Debug.DrawLine(debugBottomLeft, debugUpperLeft, Color.green);
        Debug.DrawLine(debugUpperLeft, debugUpperRight, Color.green);
        Debug.DrawLine(debugUpperRight, debugBottomRight, Color.green);
        */
    }

    Vector2 PositionToFollow()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerPosition = player.transform.position;

        Vector2 toFollowPosition = mousePosition;
        toFollowPosition -= screenCenter; // position relative to the center of the screen
        toFollowPosition.x *= heightToWidthRatio; // clamping x position so it matches height
        toFollowPosition += screenCenter; // returning to usual coordinates

        toFollowPosition = Camera.main.ScreenToWorldPoint(toFollowPosition);

        Debug.DrawLine(playerPosition, toFollowPosition);
        toFollowPosition = new Vector2(toFollowPosition.x - playerPosition.x, toFollowPosition.y - playerPosition.y);
        toFollowPosition *= ratioDistance;
        toFollowPosition += playerPosition;
        Debug.DrawLine(playerPosition, toFollowPosition, Color.magenta);
        
        if (deadZone)
            toFollowPosition = DeadZone(toFollowPosition, playerPosition);
        Debug.DrawLine(playerPosition, toFollowPosition, Color.red);

        return toFollowPosition;
    }

    /*
     * When inside the deadZone newPosition is centered on the screen. It will only start moving when its 
     * distance from toFollowPosition is greater than the offset. Then, when moving, it keeps that distance from
     * toFollowPosition.
     */
    Vector2 DeadZone(Vector2 toFollowPosition, Vector2 playerPosition)
    {
        Vector2 newPosition = toFollowPosition;

        if (toFollowPosition.x > (playerPosition.x + xOffset))
        {
            newPosition.x -= xOffset;
        }
        else if (toFollowPosition.x < (playerPosition.x - xOffset))
        {
            newPosition.x += xOffset;
        }
        else
        {
            newPosition.x = playerPosition.x;
        }

        if (toFollowPosition.y > (playerPosition.y + yOffset))
        {
            newPosition.y -= yOffset;
        }
        else if (toFollowPosition.y < (playerPosition.y - yOffset))
        {
            newPosition.y += yOffset;
        }
        else
        {
            newPosition.y = playerPosition.y;
        }

        return newPosition;
    }
}
