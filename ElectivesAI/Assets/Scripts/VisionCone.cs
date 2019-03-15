using UnityEngine;
using System;

public class VisionCone : MonoBehaviour
{
    // The tank of the current sight sensors.
    private GameObject _agent;
    public GameObject EyesAnchor;

    #region Target in sight references
    [NonSerialized]
    public GameObject Target;
    [NonSerialized]
    public GameObject Obstacle;
    public float DistanceToTarget;
    [NonSerialized]
    public string TargetToLookFor = "Tank";
    #endregion

    public Vector3 LastSeenDestination;

    void Start()
    {
        if (_agent == null)
        {
            // This is the parent tank game object we use for the raycasting,
            // since we dont want to use this script's gameobject as a start of distance.
            _agent = transform.parent.transform.parent.transform.GetChild(0).gameObject;
        }
    }

    private void Update()
    {
        // These lines keep the eyes to the anchor of the tank's turret.
        transform.parent.position = new Vector3(EyesAnchor.transform.position.x, EyesAnchor.transform.position.y, EyesAnchor.transform.position.z + 0.131f);
        transform.parent.rotation = EyesAnchor.transform.rotation;

        if (Target != null)
        {
            DistanceToTarget = Vector3.Distance(_agent.transform.position, Target.transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // We calculate the distance and direction between the spotted object
        // and the current object, in order to cast a ray and check for obstacles.
        GameObject target = other.gameObject;
        Vector3 agentPosition = _agent.transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 direction = targetPosition - agentPosition;

        float length = direction.magnitude;
        direction.Normalize();

        Ray ray = new Ray(agentPosition, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, length);

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].collider.gameObject;
            // Depending on the scale of the game and the unique objects with tags
            // replace this code block of if-s with a switch statement instead!
            //Debug.Log(hitObject.tag + " | " + hitObject.name);
            if (hitObject.tag == "Obstacle")
            {
                //Debug.DrawLine(agentPosition, targetPosition, Color.red);
                Obstacle = hitObject;
                return;
            }

            if (hitObject.tag == TargetToLookFor)
            {
                Target = hitObject;
                //Debug.DrawLine(agentPosition, targetPosition, Color.black);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == TargetToLookFor) {
            Target = null;
        }
        if (other.gameObject.name == "Node Detector")
        {
            LastSeenDestination = other.gameObject.transform.parent.position;
            Debug.Log(LastSeenDestination);
        }
    }
}
