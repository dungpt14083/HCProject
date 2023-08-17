using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;
using System;
namespace Bingo
{
    public class Bingo_GameTargetSpawner : MonoBehaviour
    {
        #region singleton
        public static Bingo_GameTargetSpawner instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }


        #endregion
        
        [TitleGroup("___________  Reference  __________")]
        public Action OnSpawnCallback; //all function registered on editor
        public float distanceTargetMove;
        public GameObject pos1, pos2, contents;
        public GameObject targetPref;


        private void Start()
        {
            distanceTargetMove = pos1.transform.position.x - pos2.transform.position.x;
        }

        [Button]
        public void SpawnNewTarget(BingoGameData data)
        {
            if (data.choseTarget == "")
            {
                Debug.LogError("NULL DATA");
                return;
            }
            OnSpawnCallback?.Invoke();

            var newTarget = Instantiate(targetPref);
            newTarget.transform.SetParent(contents.transform);
            newTarget.transform.position = pos1.transform.position;

            
            newTarget.GetComponent<Bingo_GameTargetSingleItem>().SetupSelf(data.choseTarget, data.choseTargetSecondTime);
            Bingo_SoundManager.instance.PlaySound_NewTarget(data.choseTarget);

        }

    }
}