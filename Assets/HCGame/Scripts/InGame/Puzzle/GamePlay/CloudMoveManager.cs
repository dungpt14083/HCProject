using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class CloudMoveManager : MonoBehaviour
{





    public List<GameObject> cloud, pos;

    public float speedMove, speedDelay;
    [Button]
    public void CloudMove()
    {
        var rd = Random.Range(0, cloud.Count);
        var rd2 = Random.Range(0, 2);  // 0 left 1 right
        var rd3 = Random.Range(0, 2);  // 0 bot 1 top
        var dir = rd2 == 1 ? 0 : 1;



        cloud[rd].transform.position = pos[rd3].transform.GetChild(rd2).position;
        cloud[rd].transform.DOMove(pos[rd3].transform.GetChild(dir).position, speedMove).SetDelay(speedDelay).OnComplete(() =>
        {
            CloudMove();
        });
    }


    private void Start()
    {
        DOVirtual.DelayedCall(5, () => { CloudMove(); });
    }











}
