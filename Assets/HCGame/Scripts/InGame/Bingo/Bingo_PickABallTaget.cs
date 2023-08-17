using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bingo
{
    public class Bingo_PickABallTaget : MonoBehaviour
    {
        #region singleton
        public static Bingo_PickABallTaget instance;
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
        public void SpawnPickABallTarget(string value,int timespawn)
        {
            if (value == "")
            {
                Debug.LogError("NULL DATA");
                return;
            }
            OnSpawnCallback?.Invoke();
    
            var newTarget = Instantiate(targetPref);
            newTarget.gameObject.SetActive(true);
            newTarget.transform.SetParent(contents.transform);
            newTarget.transform.position = pos1.transform.position;
    
    
            newTarget.GetComponent<Bingo_GameTargetSingleItem>().SetupSelf(value, timespawn);
            Bingo_SoundManager.instance.PlaySound_NewTarget(value);
            StartCoroutine(ActivateGameObjectsAfterDelay(contents, newTarget, 3));

        }
    
    
    
        IEnumerator ActivateGameObjectsAfterDelay(GameObject obj1, GameObject obj2, float delay)
        {
            yield return new WaitForSeconds(delay);

            obj1.SetActive(false);
            obj2.SetActive(false);
            Bingo_GameTargetSpawner.instance.contents.SetActive(true);
            Bingo_NetworkManager.instance.SendMessageNoClickPickABalType7(0,0,7);
        }
    
    
    
    
    
    
    
    
    
    }
}

