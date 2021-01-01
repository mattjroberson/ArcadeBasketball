using UnityEngine;

public class DetectionPhysics : MonoBehaviour
{
    private PlayerScript player;
    private ActionsScript actions;

    private RaycastProperties stealRaycastProps;
    private RaycastProperties shootRaycastProps;

    private const float NUM_RAYS = 3;

    private ContactFilter2D defenderContactFilter;

    void Start()
    {
        player = GetComponent<PlayerScript>();
        actions = GetComponent<ActionsScript>();

        stealRaycastProps = new RaycastProperties(.75f, -.075f, .075f);
        shootRaycastProps = new RaycastProperties(1.1f, -10f, 10f);

        defenderContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("DefenderFilter")));
        defenderContactFilter.useLayerMask = true;
        defenderContactFilter.useTriggers = false;

        actions.events.onSwipeBegin += SwipeBeginEvent; 
    }

    private void SwipeBeginEvent()
    {
        if(IsPlayerInRange("steal")) actions.AttemptSteal();
    }

    //Raycasts to see if their is a player nearby by for stealing and shot contests.
    private bool IsPlayerInRange(string type)
    {
        //Adopt orientation of the player
        bool facingRight = transform.rotation.y == 0 ? true : false;

        RaycastProperties rayProps = type.Equals("steal") ? stealRaycastProps : shootRaycastProps;
        
        Vector2 raySource = player.FrontPoint.FloorPosition;
        Vector2 rayRotation = type.Equals("steal") ? Vector2.zero : GetDirectionToGoal(raySource);
        Vector2 rayDirection = facingRight ? Vector2.right : Vector2.left;

        //Cast each ray
        for (int i = 0; i < NUM_RAYS; i++)
        { 
            //Steal checks straight ahead, while shoot checks rotated toward the goal
            if (type.Equals("steal"))
            {
                rayDirection = new Vector2(rayDirection.x, rayProps.firstY + (rayProps.gapY * i));
            }
            else if (type.Equals("shoot"))
            {
                rayDirection = Quaternion.Euler(0, 0, rayProps.firstY + (rayProps.gapY * i)) * rayRotation;
            }

            Transform hit = ProjectRaycast(raySource, rayDirection, rayProps.distance, type, true);
            if (hit != null) return true;
        }

        return false;
    }

    private Vector2 GetDirectionToGoal(Vector2 raySource)
    {
        return (player.CurrentGoal.baseOfGoal.position - (Vector3)raySource).normalized;
    }

    //Creates a raycast, returns first valid player
    private Transform ProjectRaycast(Vector2 location, Vector2 direction, float distance, string type, bool debug)
    {
        //Draw the rays if in debug mode
        if (debug) Debug.DrawLine(location, location + (direction * distance), Color.red, 3);

        RaycastHit2D[] collisions = new RaycastHit2D[8];
        int count = Physics2D.Raycast(location, direction, defenderContactFilter, collisions, distance);

        for (int i = 0; i < count; i++)
        {
            Transform transform = ValidateCollisions(collisions[i], type);
            if (transform != null) return transform;
        }

        return null;
    }

    //Checks whether the collision is with a valid object or not
    private Transform ValidateCollisions(RaycastHit2D collision, string type)
    {
        if (type.Equals("steal"))
        {
            //Only return the player if they have the ball
            if (collision.transform.GetComponentInChildren<BallScript>() != null)
            {
                return collision.transform;
            }
        }
        else if (type.Equals("shoot"))
        {
            //Only return the player if they are on different teams
            if (collision.transform.parent != transform.parent)
            {
                return collision.transform;
            }
        }

        return null;
    }

    //TODO Make this more precise. Currently triggers for hands and feet
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "basketball") actions.TouchBall();
    }

    class RaycastProperties
    {
        public float distance;
        public float firstY;
        public float gapY;

        public RaycastProperties(float distance, float firstY, float gapY)
        {
            this.distance = distance;
            this.firstY = firstY;
            this.gapY = gapY;
        }
    }
}
