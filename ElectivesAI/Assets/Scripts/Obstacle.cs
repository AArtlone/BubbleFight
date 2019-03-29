using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name != "Plane")
        {
            if (other.transform.tag == "NodeN" ||
                other.transform.tag == "NodeY" ||
                other.transform.tag == "NodeD" ||
                other.transform.tag == "NodeA")
            {
                other.GetComponent<AStarNode>().unWalkable = true;
            }
        }

        if (other.transform.tag == "Bullet")
        {
            ParticlesManager.Instance.CreateExplosion(other.transform);
            if (gameObject.tag == "Tree")
            {
                AudioManager.Instance.PlayTreeDestroyed();
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
}
