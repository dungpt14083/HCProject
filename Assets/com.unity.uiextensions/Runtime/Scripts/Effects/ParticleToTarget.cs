using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleToTarget : MonoBehaviour
{
    public Transform Target;
    public bool isWaitFollow;
    public float timeWait;
    
    private ParticleSystem system;

    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];

    int count;

    void Start() {
        if (system == null)
            system = GetComponent<ParticleSystem>();

        if (system == null){
            this.enabled = false;
        }else{
            system.Play();
        }
    }
    void Update()
    {
        StartCoroutine(WaitFollowTarget(isWaitFollow));
    }

    private IEnumerator WaitFollowTarget(bool isWait)
    {
        if (isWait)
        {
            yield return new WaitForSeconds(timeWait);
        }
        else
        {
            yield return null;
        }
        
        count = system.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            
            Vector3 v1 = system.transform.TransformPoint(particle.position);
            Vector3 v2 = Target.transform.position;
            
            //パーティクル生成残り時間に応じて距離をつめる
            Vector3 tarPosi = (v2 - v1) *  (particle.remainingLifetime / particle.startLifetime);
            particle.position = system.transform.InverseTransformPoint(v2 - tarPosi);
            particles[i] = particle;
            
            if (v1 == v2)
            {
                particles[i].remainingLifetime = 0;
            } 
        }
        system.SetParticles(particles, count);
    }
    
}
