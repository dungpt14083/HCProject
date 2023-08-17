using UnityEngine;
using System.Collections;

namespace BubblesShot
{
    public class LinePoint : MonoBehaviour
    {
        int nextWayPoint;
        float speed = 3;
        public Vector2 startPoint;
        public Vector2 nextPoint;
        // Use this for initialization
        void Start()
        {
            transform.position = DrawLine.waypoints[0];
            nextWayPoint++;
        }

        // Update is called once per frame
        void Update()
        {
            if (startPoint == nextPoint) GetComponent<SpriteRenderer>().enabled = false;

            transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
            if ((Vector2)transform.position == nextPoint)
            {
                nextWayPoint = 0;
                transform.position = startPoint;
            }
        }
    }
}
