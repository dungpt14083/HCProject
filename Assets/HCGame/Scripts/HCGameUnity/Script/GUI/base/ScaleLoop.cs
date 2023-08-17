using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ScaleLoop : MonoBehaviour
{
    [SerializeField] private float animateTime = 0.5f;
    [SerializeField] private float idleTime = 1.0f;
    [SerializeField] private float animateScale = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PunchScaleLoop());
    }

    public IEnumerator PunchScaleLoop()
    {
        while(true)
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * animateScale, animateTime);
            yield return new WaitForSeconds(animateTime + idleTime);
        }
    }
}
