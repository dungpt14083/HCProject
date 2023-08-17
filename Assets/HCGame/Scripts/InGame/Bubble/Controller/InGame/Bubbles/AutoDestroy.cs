using UnityEngine;
using System.Collections;

namespace BubblesShot
{
    public class AutoDestroy : MonoBehaviour
    {

        // Use this for initialization
        void OnEnable()
        {
            Invoke("Hide", 2);
        }

        void Hide()
        {
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
