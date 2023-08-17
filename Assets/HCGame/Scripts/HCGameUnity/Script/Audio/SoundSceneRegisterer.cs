using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public class SoundSceneRegisterer : MonoBehaviour
{
    // Start is called before the first frame update

    private AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        if(null != EazySoundManager.Gameobject && null != source)
        {
            EazySoundManager.RegisterSceneSoundSource(source);
        }
    }

    private void OnDestroy()
    {

        if (null != EazySoundManager.Gameobject && null != source)
        {
            EazySoundManager.UnregisterSceneSoundSource(source);
        }
    }
}
