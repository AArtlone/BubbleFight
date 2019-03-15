using UnityEngine;

public class Diego : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank tankInterface;
    private VisionCone vision;
    private int state = 0;

    //Astart path variables
    private AStarPath aStarPath;
    private AStarNodeDetector aNodeDetector;
    private GameObject nodeGrid;

    private bool lookForTarget = true;
    public LayerMask NodeMask;


    private void Start()
    {
        tankInterface = GetComponent<Tank>();
        nodeGrid = GameObject.Find("NodeListN");
        vision = transform.parent.GetComponentInChildren<VisionCone>();
        aStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        aNodeDetector = transform.parent.GetComponentInChildren<AStarNodeDetector>();

    }

    private void Update()
    {
        //health
        if (tankInterface.GetHealth() <= 5 && (state == 0 || state == 2)) {
            state = 1;
        }

        if (tankInterface.GetAmmo() <= 0 && (state == 0 || state == 1)) {
            state = 2;
        }

        if (state == 0) {
            if (vision.DistanceToTarget > tankInterface.GetFireRange())
            {
                if (lookForTarget)
                {
                    FollowPlayer("Tank");
                    lookForTarget = false;
                }
            }
            else {
                tankInterface.RotateTurret(vision.Target.transform);
                tankInterface.Shoot();
            }
        }

        if (state == 1)
        {
            if (lookForTarget)
            {
                FollowPlayer("Health Pickup");
                lookForTarget = false;
            }
        }

        if (state == 2)
        {
            if (lookForTarget)
            {
                FollowPlayer("Ammo Pickup");
                lookForTarget = false;
            }
        }
    }

    private void FollowPlayer(string pTarget) {
        vision.TargetToLookFor = pTarget;
        if (vision.Target.gameObject.tag == pTarget)
        {
            Vector3 targetPosition = vision.Target.transform.position;
            Vector3 direction = targetPosition - transform.position;
            float length = direction.magnitude;
            direction.Normalize();
            Ray ray = new Ray(transform.position, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, length, NodeMask);

            GameObject node = hits[hits.Length - 1].transform.gameObject;

            aStarPath.startNode = aNodeDetector.CurrentNode;
            aStarPath.endNode = node.GetComponent<AStarNode>();
            aNodeDetector.CurrentNodeIndexInPath = 1;
            aNodeDetector.PathOfTank = aStarPath.FindShortestPath();

        }

    }

    private void GetDirectionVector(){
        
    }

    private void EnableLookingForTarget()
    {
        lookForTarget = true;
    }

    private void FixedUpdate()
    {

    }
}
