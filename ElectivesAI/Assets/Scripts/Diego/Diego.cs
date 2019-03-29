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
        
        vision = transform.parent.GetComponentInChildren<VisionCone>();
        aStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        aNodeDetector = transform.parent.GetComponentInChildren<AStarNodeDetector>();
        aNodeDetector.CurrentNode = aStarPath.endNode;
        nodeGrid = GameObject.Find("NodeListD");
    }

    private void Update()
    {
        //health
        if (state == 0) //offensive state
        {
            if (vision.DistanceToTarget > tankInterface.GetFireRange()) //if distance between tank and target is bigger than fire range
            {
                if (lookForTarget)
                {
                    FollowPlayer("Tank"); //finds objects with tank tag
                    //lookForTarget = false;
                }
            }
            else if (vision.Target != null)
            {
                tankInterface.RotateTurret(vision.Target.transform); //if in range, rotate turret and shoot
                tankInterface.Shoot();
            }

            if (vision.Target == null)
            {
                MoveTank();
            }
        }
        else {
            tankInterface.RotateTheTank(gameObject.transform);
        }
        
        if (tankInterface.GetHealth()  <= 5 && (state == 0 || state == 2)) {
            //state = 1;
        }

        if (tankInterface.GetAmmo() <= 0 && (state == 0 || state == 1)) {
            //state = 2;
        }
        
        if (state == 1) //not yet implemented
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
        if (vision.Target.gameObject.tag == pTarget) //if object is tag name Tank
        {
            Vector3 targetPosition = vision.Target.transform.position;
            Vector3 direction = targetPosition - transform.position; 
            float length = direction.magnitude; //calculates length of vector
            direction.Normalize();

            Ray ray = new Ray(transform.position, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, length, NodeMask);

            GameObject node = hits[hits.Length - 1].transform.gameObject;

            //Debug.Log(node);
            aStarPath.startNode = aNodeDetector.CurrentNode;
            aStarPath.endNode = node.GetComponent<AStarNode>();
            aNodeDetector.CurrentNodeIndexInPath = 1;
            aNodeDetector.PathOfTank = aStarPath.FindShortestPath();
            MoveTank();
        }
    }

    private void MoveTank()
    {
        tankInterface.MoveTheTank("Forward"); //Moves tank forward
        tankInterface.RotateTheTank(aNodeDetector.PathOfTank[aNodeDetector.PathOfTank.Count - 1 - aNodeDetector.CurrentNodeIndexInPath].transform); //moves tank along nodes 

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
