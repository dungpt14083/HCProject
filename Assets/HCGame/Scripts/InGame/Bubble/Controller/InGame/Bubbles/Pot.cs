using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BubblesShot
{
    public class Pot : MonoBehaviour
    {
        public int scoreBonus;
        public GameObject splashPrefab;

        // Use this for initialization
        void Start()
        {

        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.name.Contains("ball"))
            {
                col.gameObject.GetComponent<ball>().SplashDestroy();
                col.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                col.gameObject.GetComponent<Collider2D>().enabled = false;
                PlaySplash(col.contacts[0].point);
            }
        }

        void PlaySplash(Vector2 pos)
        {
            StartCoroutine(SoundsCounter());
            if (mainscript.Instance.potSounds < 4)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.pops);

            GameObject splash = (GameObject)Instantiate(splashPrefab, transform.position + Vector3.up * 0.9f + Vector3.left * 0.35f, Quaternion.identity);
            Destroy(splash, 2f);

            mainscript.Instance.PopupScore(scoreBonus, transform.position + Vector3.up);

            //WebSocket - Send ball counter be destroy on pot to server
            //Using model ScoreCompute in DataInGameModel
            //scores = _dataGamePlay.scores += value;
            //if(scoreBonus == 100) brokenBubbleOnHole100 = 1
            //else brokenBubbleOnHole200 = 1

        }

        IEnumerator SoundsCounter()
        {
            mainscript.Instance.potSounds++;
            yield return new WaitForSeconds(0.2f);
            mainscript.Instance.potSounds--;
        }
    }

}
