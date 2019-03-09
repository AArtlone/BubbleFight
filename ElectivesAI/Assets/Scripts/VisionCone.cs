using UnityEngine;

public class VisionCone : MonoBehaviour
{
    private GameObject _agent;
    
    void Start()
    {
     if (_agent == null)
        {
            // This is the parent tank game object we use for the raycasting,
            // since we dont want to use this script's gameobject as a start of distance.
            _agent = transform.parent.transform.parent.gameObject;
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
            if (hitObject.tag == "Wall")
            {
                Debug.DrawLine(agentPosition, targetPosition, Color.red);
                return;
            } else if (hitObject.tag == "Tank")
            {
                Debug.DrawLine(agentPosition, targetPosition, Color.green);
            }
        }
    }
}
