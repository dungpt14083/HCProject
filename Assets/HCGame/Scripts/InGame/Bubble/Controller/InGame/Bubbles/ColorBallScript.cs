using UnityEngine;
using System.Collections;

namespace BubblesShot
{
    public enum BallColor
    {
        blue = 1,
        green,
        red,
        violet,
        yellow,
        random,
        colorful
    }

    public class ColorBallScript : MonoBehaviour
    {
        public Sprite[] sprites;
        public BallColor mainColor;
        //
        public bool isShowBall;
        // Use this for initialization
        void Start()
        {
        }

        public void SetColor(BallColor color)
        {
            mainColor = color;
            foreach (Sprite item in sprites)
            {
                if (item.name == "ball_" + color)
                {
                    GetComponent<SpriteRenderer>().sprite = item;
                    gameObject.tag = "" + color;
                }
            }
        }

        public void SetShowHide(bool isShow)
        {
            isShowBall = isShow;

            var col = gameObject.GetComponent<SpriteRenderer>().color;
            if (isShow)
                col.a = 1;
            else
                col.a = 0;
            gameObject.GetComponent<SpriteRenderer>().color = col;
        }

        public void ChangeRandomColor()
        {
            mainscript.Instance.GetColorsInGame();
            SetColor((BallColor)mainscript.colorsDict[Random.Range(0, mainscript.colorsDict.Count)]);
            GetComponent<Animation>().Stop();
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y <= -16 && transform.parent == null) { Destroy(gameObject); }
        }
    }
}
