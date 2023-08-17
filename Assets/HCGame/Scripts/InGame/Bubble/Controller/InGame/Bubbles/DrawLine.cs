using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BubblesShot;
using Spine.Unity;
using System.Drawing;
using Color = UnityEngine.Color;

namespace BubblesShot
{
    public class DrawLine : MonoBehaviour
    {
        //[SerializeField] private Image _timePanel;
        //[SerializeField] private TextMeshProUGUI _time;
        //
        //[SerializeField] private Image _frame1;
        //[SerializeField] private Image _avatar1;
        //[SerializeField] private TextMeshProUGUI _score1;
        //
        //[SerializeField] private Image _frame2;
        //[SerializeField] private Image _avatar2;
        //[SerializeField] private TextMeshProUGUI _score2;
        [SerializeField] private GameObject _gun;
        [SerializeField] private SkeletonAnimation _charactor;

        LineRenderer line;
        bool draw = false;
        Color col;

        public static Vector2[] waypoints = new Vector2[3];
        public float addAngle = 90;
        public GameObject pointer;
        GameObject[] pointers = new GameObject[20];
        GameObject[] pointers2 = new GameObject[20];
        Vector3 lastMousePos;
        private bool startAnim;
        private bool _rotationGun;
        public static float waitingTime = 0;

        // Use this for initialization
        void Start()
        {
            line = GetComponent<LineRenderer>();
            GeneratePoints();
            GeneratePositionsPoints();
            HidePoints();
            waypoints[0] = transform.position;
            waypoints[1] = transform.position + Vector3.up * 5;
            _rotationGun = false;
            _charactor.AnimationName = "";
        }

        void HidePoints()
        {
            foreach (GameObject item in pointers)
            {
                item.GetComponent<SpriteRenderer>().enabled = false;
            }

            foreach (GameObject item in pointers2)
            {
                item.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        private void GeneratePositionsPoints()
        {
            if (mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy != null)
            {
                col = mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<SpriteRenderer>().sprite.texture.GetPixelBilinear(0.6f, 0.6f);
                col.a = 1;
            }

            HidePoints();

            for (int i = 0; i < pointers.Length; i++)
            {
                Vector2 AB = waypoints[1] - waypoints[0];
                AB = AB.normalized;
                float step = i / 2f;

                if (step < (waypoints[1] - waypoints[0]).magnitude)
                {
                    pointers[i].GetComponent<SpriteRenderer>().enabled = true;
                    pointers[i].transform.position = waypoints[0] + (step * AB);
                    pointers[i].GetComponent<SpriteRenderer>().color = col;
                    pointers[i].GetComponent<LinePoint>().startPoint = pointers[i].transform.position;
                    pointers[i].GetComponent<LinePoint>().nextPoint = pointers[i].transform.position;
                    if (i > 0)
                        pointers[i - 1].GetComponent<LinePoint>().nextPoint = pointers[i].transform.position;
                }
            }
            for (int i = 0; i < pointers2.Length; i++)
            {
                Vector2 AB = waypoints[2] - waypoints[1];
                AB = AB.normalized;
                float step = i / 2f;

                if (step < (waypoints[2] - waypoints[1]).magnitude)
                {
                    pointers2[i].GetComponent<SpriteRenderer>().enabled = true;
                    pointers2[i].transform.position = waypoints[1] + (step * AB);
                    pointers2[i].GetComponent<SpriteRenderer>().color = col;
                    pointers2[i].GetComponent<LinePoint>().startPoint = pointers2[i].transform.position;
                    pointers2[i].GetComponent<LinePoint>().nextPoint = pointers2[i].transform.position;
                    if (i > 0)
                        pointers2[i - 1].GetComponent<LinePoint>().nextPoint = pointers2[i].transform.position;
                }
            }
        }

        void GeneratePoints()
        {
            for (int i = 0; i < pointers.Length; i++)
            {
                pointers[i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
                pointers[i].transform.parent = transform;
            }
            for (int i = 0; i < pointers2.Length; i++)
            {
                pointers2[i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
                pointers2[i].transform.parent = transform;
            }
        }

        // Update is called once per frame
        void Update()
        {
            waitingTime -= Time.deltaTime;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //
            if (pos.y > -3f && pos.y < 5f && GamePlay.Instance.GameStatus != GameState.GameOver && GamePlay.Instance.GameStatus != GameState.Pause && GamePlay.Instance.GameStatus != GameState.WaitForPopup && waitingTime <= 0)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    draw = true;
                    if (GamePlay.Instance.GameStatus == GameState.Playing)
                    {
                        _rotationGun = true;
                    }
                }
                if (_rotationGun && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > -3f && !mainscript.StopControl)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _gun.transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
                }

                if (draw)
                {
                    _charactor.AnimationName = "animation";
                    Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.back * 10;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (!mainscript.StopControl)
                    {
                        dir.z = 0;
                        if (lastMousePos == dir)
                        {
                            startAnim = true;
                        }
                        else startAnim = false;
                        lastMousePos = dir;
                        line.SetPosition(0, transform.position);

                        waypoints[0] = transform.position;
                        var reDrawLine2 = false;
                        float angle = 0;
                        addAngle = 180;
                        RaycastHit2D[] hit = Physics2D.LinecastAll(waypoints[0], waypoints[0] + ((Vector2)dir - waypoints[0]).normalized * 10);
                        foreach (RaycastHit2D item in hit)
                        {
                            Vector2 point = item.point;
                            line.SetPosition(1, point);

                            if (waypoints[1].x < 0) addAngle = 0;
                            if (item.collider.gameObject.layer == LayerMask.NameToLayer("Border") && item.collider.gameObject.name != "GameOverBorder" && item.collider.gameObject.name != "borderForRoundedLevels")
                            {
                                reDrawLine2 = true;
                                waypoints[1] = point;
                                //waypoints[2] = point;
                                line.SetPosition(1, dir);
                                waypoints[1] = point;
                                angle = Vector2.Angle(waypoints[0] - waypoints[1], (point - Vector2.up * 100) - (Vector2)point);
                                if (waypoints[1].x > 0) angle = Vector2.Angle(waypoints[0] - waypoints[1], (Vector2)point - (point - Vector2.up * 100));
                                waypoints[2] = Quaternion.AngleAxis(angle + addAngle, Vector3.back) * ((Vector2)point - (point - Vector2.up * 100));
                                Vector2 AB = waypoints[2] - waypoints[1];
                                AB = AB.normalized;
                                line.SetPosition(2, waypoints[2]);
                                break;
                            }
                            else if (item.collider.gameObject.layer == LayerMask.NameToLayer("Ball"))
                            {
                                line.SetPosition(1, point);
                                waypoints[1] = point;
                                waypoints[2] = point;
                                Vector2 AB = waypoints[2] - waypoints[1];
                                AB = AB.normalized;
                                line.SetPosition(2, waypoints[1] + (0.1f * AB));
                                break;
                            }
                            else
                            {
                                waypoints[1] = waypoints[0] + ((Vector2)dir - waypoints[0]).normalized * 10;
                                waypoints[2] = waypoints[0] + ((Vector2)dir - waypoints[0]).normalized * 10;
                            }
                        }
                        if (reDrawLine2)
                        {
                            RaycastHit2D[] hit2 = Physics2D.LinecastAll(waypoints[1], waypoints[1] + ((Vector2)waypoints[2] - waypoints[1]).normalized * 10);
                            foreach (RaycastHit2D item in hit2)
                            {
                                Vector2 point = item.point; if (item.collider.gameObject.layer == LayerMask.NameToLayer("Ball"))
                                {
                                    line.SetPosition(1, point);
                                    waypoints[2] = point;
                                    Vector2 AB = waypoints[2] - waypoints[1];
                                    AB = AB.normalized;
                                    line.SetPosition(2, waypoints[1] + (0.1f * AB));
                                    break;
                                }

                            }
                        }

                        if (!startAnim)
                            GeneratePositionsPoints();
                    }
                }
                else
                {
                    _charactor.AnimationName = "";
                    HidePoints();
                }
            }
            else
            {
                HidePoints();
            }
            if (Input.GetMouseButtonUp(0))
            {
                draw = false;
                _rotationGun = false;
                waitingTime = .5f;
            }
        }
    }
}
