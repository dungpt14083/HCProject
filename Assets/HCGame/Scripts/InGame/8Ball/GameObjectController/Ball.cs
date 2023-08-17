using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public Rigidbody Rigidbody => rb;
    [Header("Sound")]
    public AudioSource cueVsBall;
    public float cueVsBallMaxVol = 0.8f;
    public AudioSource ballVsBallAudioSource;
    public float ballVsBallMaxVol = 0.6f;
    

    private void Start()
    {
        //GameController.Instance.onSettingUpdate += UpdateSetting;
        if (rb == null)
            rb = GetComponent<Rigidbody>();

    }

    public void SetBallTexture(Texture texture)
    {
        var renderer = gameObject.GetComponentInChildren<MeshRenderer>();
        renderer.material.SetTexture("_MainTex", texture);
    }

    public void SoundCueBall(float volume)
    {
        if(cueVsBall != null)
        {
            cueVsBall.volume = Mathf.Min(cueVsBallMaxVol, volume);
            cueVsBall.Play();
        }
    }

    public void SoundBallVsBall(float volume)
    {
        if (ballVsBallAudioSource != null)
        {
            //Debug.Log($"SHPT Play sound of {transform.name} with volume = {Mathf.Min(ballVsBallMaxVol, volume)}");
            ballVsBallAudioSource.volume = Mathf.Max(ballVsBallMaxVol, volume);
            ballVsBallAudioSource.Play();
            //Debug.Log($"SHPT Play sound with volume = {volume}");
        }
    }

}
