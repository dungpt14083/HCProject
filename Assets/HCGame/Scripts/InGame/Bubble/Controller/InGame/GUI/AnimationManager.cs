using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using InitScriptName;
using System.Collections.Generic;
using BubblesShot;

namespace BubblesShot
{
    public class AnimationManager : MonoBehaviour
    {
        UserData _userData;
        public bool PlayOnEnable = true;
        bool WaitForPickupFriends;

        bool WaitForAksFriends;
        System.Collections.Generic.Dictionary<string, string> parameters;

        void OnEnable()
        {
            if (PlayOnEnable)
            {
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);

                if (name == "Fire")
                {


                }
            }
            if (name == "MenuPlay")
            {
                for (int i = 1; i <= 3; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
                }
                int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", PlayerPrefs.GetInt("OpenLevel")), 0);
                if (stars > 0)
                {
                    for (int i = 1; i <= stars; i++)
                    {
                        transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
                    }

                }
                else
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
                    }

                }

            }

        }
        void OnDisable()
        {
            //if( PlayOnEnable )
            //{
            //    if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
            //        GetComponent<SequencePlayer>().sequenceArray[0].Play
            //}
        }
        IEnumerator MenuComplete()
        {
            for (int i = 1; i <= mainscript.Instance.stars; i++)
            {
                //  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
                transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.hit);
            }
        }
        IEnumerator MenuCompleteScoring()
        {
            var _dataGamePlay = Commons.GetDataGamePlay();
            Text scores = transform.Find("Image").Find("Scores").GetComponent<Text>();
            for (int i = 0; i <= _dataGamePlay.scores; i += 500)
            {
                scores.text = "" + i;
                // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
                yield return new WaitForSeconds(0.00001f);
            }
            scores.text = "" + _dataGamePlay.scores;
        }


        public void PlaySoundButton()
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        }

        public IEnumerator Close()
        {
            yield return new WaitForSeconds(0.5f);
        }

        public void CloseMenu()
        {
            //SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.click );
            //if( gameObject.name == "MenuPreGameOver" )
            //{
            //    ShowGameOver();
            //}
            //if( gameObject.name == "MenuComplete" )
            //{
            //     //Application.LoadLevel( "map" );
            //}
            //if( gameObject.name == "MenuGameOver" )
            //{
            //    //Application.LoadLevel( "map" );
            //}
            //
            //if( Application.loadedLevelName == "game" )
            //{
            //    if( GamePlay.Instance.GameStatus == GameState.Pause )
            //    {
            //        GamePlay.Instance.GameStatus = GameState.WaitAfterClose;
            //
            //    }
            //}
            //SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.swish[1] );
            //
            //gameObject.SetActive( false );
        }

        //public void Play()
        //{
        //    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        //    if (gameObject.name == "MenuPreGameOver")
        //    {
        //        if (InitScript.Gems >= 12)
        //        {
        //            InitScript.Instance.SpendGems(12);
        //            LevelData.LimitAmount += 12;
        //            GamePlay.Instance.GameStatus = GameState.WaitAfterClose;
        //            gameObject.SetActive(false);

        //        }
        //        else
        //        {
        //            BuyGems();
        //        }
        //    }
        //    else if (gameObject.name == "MenuGameOver")
        //    {
        //        Application.LoadLevel("map");
        //    }
        //    else if (gameObject.name == "MenuPlay")
        //    {
        //        if (InitScript.Lifes > 0)
        //        {
        //            InitScript.Instance.SpendLife(1);

        //            Application.LoadLevel("game");
        //        }
        //        else
        //        {
        //            BuyLifeShop();
        //        }

        //    }
        //    else if (gameObject.name == "PlayMain")
        //    {
        //        Application.LoadLevel("map");
        //    }
        //}

        public void PlayTutorial()
        {
            //        SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.click );
            GamePlay.Instance.GameStatus = GameState.Playing;
            //    mainscript.Instance.dropDownTime = Time.time + 0.5f;
            //        CloseMenu();
        }

        public void Next()
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
            CloseMenu();
        }

        void ShowGameOver()
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver);

            GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
            gameObject.SetActive(false);

        }

        #region Settings
        public void ShowSettings(GameObject menuSettings)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
            if (!menuSettings.activeSelf)
            {
                menuSettings.SetActive(true);
            }
            else menuSettings.SetActive(false);
        }

        public void Info()
        {
            if (Application.loadedLevelName == "map" || Application.loadedLevelName == "menu")
                GameObject.Find("Canvas").transform.Find("Tutorial").gameObject.SetActive(true);
            else
                GameObject.Find("Canvas").transform.Find("PreTutorial").gameObject.SetActive(true);
        }

        public void Quit()
        {
            if (Application.loadedLevelName == "game")
                Application.LoadLevel("map");
            else
                Application.Quit();
        }



        #endregion
    }
}
