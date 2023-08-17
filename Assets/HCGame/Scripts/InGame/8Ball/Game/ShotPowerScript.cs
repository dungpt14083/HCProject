using System;
using UnityEngine;
using System.Collections;
using Tweens.Plugins;

public class ShotPowerScript : MonoBehaviour {

    CueController cueScript;
    public GameObject shotColorIndicator;
    private Color initColor;
    public GameObject posEnd;
    public bool mouseDown = false;
    public GameObject moveTo;
    public GameObject cue;
    public GameObject cueMain;
    public GameObject cueMainMoveTo;
    public float powerMultiplyer = 1f;
    private Vector3 initialPos;
    private float initXPos;
    // Use this for initialization
    public GameObject mainObject;
    public Animator anim;
    private bool deactivateDone = false;
    private Vector3 initMainCuePos;
    private Vector3 initMainCouPositionDetector;
    // GameManager gameManager;
    [Header("UI")]
    [SerializeField] public ShotPowerCue shotPowerCue;


    private void Awake()
    {
        //anim = mainObject.GetComponent<Animator>();
        initColor = shotColorIndicator.GetComponent<SpriteRenderer>().color;
        cueScript = GameObject.Find("WhiteBall").GetComponent<CueController>();        
    }

    void Start() {
        // gameManager = GameManager.Instance;
        Debug.Log("ShotPowerScript start ");
        initialPos = cue.transform.position;
        setIndicatorColor();

        if ( EightBallGameSystem.Instance.IsMyTurn)
        {
            anim.Play("MakeVisible");
            shotPowerCue.TweenMove.OnPlay();
        }
        else
        {
            anim.Play("ShotPowerAnimation");
            shotPowerCue.TweenMove.OnBack();
        }

        shotPowerCue.OnPowerChanged += OnPowerChange;
        shotPowerCue.OnPowerFire += OnShot;
    }

    private void OnDestroy()
    {
         if(shotPowerCue != null && shotPowerCue.OnPowerChanged != null) shotPowerCue.OnPowerChanged -= OnPowerChange;
         if (shotPowerCue != null && shotPowerCue.OnPowerFire != null) shotPowerCue.OnPowerFire -= OnShot;
    }

    private void OnPowerChange(float value)
    {
        cueMain.transform.position = Vector3.Lerp(initMainCuePos, initMainCouPositionDetector, value);
    }

    void OnShot(float value)
    {
        Debug.Log($"OnShot {value}");
        Invoke("deactivate", 0.5f);
        if (value <= 0.01f) return;
        GameController.Instance.Shoot(value, cueScript.posDetector.transform.position, cueScript.trickShotAdd);
        deactivateDone = true;        
        //var steps = (1.6f * value);
        anim.Play("ShotPowerAnimation");
        shotPowerCue.TweenMove.OnBack();
        cue.transform.position = initialPos;
        cueMain.transform.position = initMainCuePos;
        shotColorIndicator.GetComponent<SpriteRenderer>().color = initColor;
    }

    public void OnMouseDown() {
        mouseDown = true;
        initXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        deactivateDone = false;
        initMainCuePos = cueMain.transform.position;
        initMainCouPositionDetector = cueMainMoveTo.transform.position;        
        // GameManager.Instance.ballHand.SetActive(false);
    }
    
    
    public void deactivate() {
        mouseDown = false;

    }
    
    void Update() {
        if (mouseDown && !deactivateDone) {
            Vector3 cuePos = cue.transform.position;
            float newYPos = initialPos.y + (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - initXPos);


            if (newYPos < initialPos.y && newYPos > posEnd.transform.position.y) {
                cuePos.y = newYPos;
                cue.transform.position = cuePos;

            } else {

                if (newYPos > initialPos.y)
                    cue.transform.position = initialPos;
                else if (newYPos < posEnd.transform.position.y) {
                    Vector3 pos = cue.transform.position;
                    pos.y = posEnd.transform.position.y;
                    cue.transform.position = pos;
                }
            }
            setIndicatorColor();
        }
    }

    // Sets indicator color when cue is moving
    private void setIndicatorColor() {
        float add = (Mathf.Abs((cue.transform.position.y - initialPos.y)) * 45 + 80) / 255.0f;
        Color color = shotColorIndicator.GetComponent<SpriteRenderer>().color;
        color.r = add;
        color.g = add;
        color.b = add;

        shotColorIndicator.GetComponent<SpriteRenderer>().color = color;
    }


}
