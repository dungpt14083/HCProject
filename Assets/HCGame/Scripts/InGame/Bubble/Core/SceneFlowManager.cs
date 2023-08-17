using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubblesShot
{
    public enum Scenes
    {
        BubbleLoading,
        BubbleGameplay
    }
    public class SceneFlowManager : Singleton<SceneFlowManager>
    {
        public void LoadScene(Scenes sceneType, bool showLoadingScene = false)
        {
            SceneManager.LoadScene(sceneType.ToString());
        }
    }
}
