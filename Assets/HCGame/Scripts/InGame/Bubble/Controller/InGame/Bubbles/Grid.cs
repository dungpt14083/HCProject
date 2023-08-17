using UnityEngine;
using System.Collections;
using InitScriptName;

namespace BubblesShot
{
    public class Grid : MonoBehaviour
    {
        [SerializeField]
        private GameObject busy;

        public GameObject Busy
        {
            get { return busy; }
            set
            {
                if (value != null)
                {
                    if (value.GetComponent<ball>() != null)
                    {
                        if (!value.GetComponent<ball>().NotSorting)
                        {
                            value.GetComponent<ball>().mesh = gameObject;
                        }
                    }

                }

                busy = value;
            }
        }

        public float offset;
        public GameObject boxFirst;
        public GameObject boxSecond;
        public static bool waitForAnim;
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (busy == null)
            {
                GameObject box = null;
                GameObject ball = null;
                if (name == "boxCatapult" && !Grid.waitForAnim)
                {
                    box = boxSecond;
                    ball = box.GetComponent<Grid>().busy;
                    if (ball != null)
                    {
                        ball.GetComponent<bouncer>().bounceToCatapult(transform.position);
                        busy = ball;
                        ball.transform.parent = transform;
                    }
                }
                else if (name == "boxSecond" && !Grid.waitForAnim)
                {
                    if (GamePlay.Instance.GameStatus == GameState.Playing || GamePlay.Instance.GameStatus == GameState.Win)
                    {
                        busy = Camera.main.GetComponent<mainscript>().createFirstBall(transform.position);
                        busy.transform.parent = transform;
                    }
                }
            }

            if (busy != null && !Grid.waitForAnim)
            {
                if (name == "boxCatapult")
                {
                    if (busy.GetComponent<ball>().setTarget)
                        busy = null;
                }
                else if (name == "boxSecond")
                {
                    if (Vector3.Distance(transform.position, busy.transform.position) > 0.9f)
                    {
                        busy = null;
                    }
                }
            }
        }

        public void BounceFrom(GameObject box)
        {
            GameObject ball = box.GetComponent<Grid>().busy;
            if (ball != null && busy != null)
            {
                busy.GetComponent<bouncer>().bounceTo(box.transform.position);
                box.GetComponent<Grid>().busy = busy;
                busy = ball;
            }
        }

        void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.name.IndexOf("ball") > -1 && busy == null)
            {
                busy = other.gameObject;
            }
        }

        void OnTriggerExit(Collider other)
        {
            //busy = null;
        }

        public void destroy()
        {
            tag = "Mesh";
            Destroy(busy);
            busy = null;
        }
    }

}