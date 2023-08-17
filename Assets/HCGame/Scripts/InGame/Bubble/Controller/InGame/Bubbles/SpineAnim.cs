using Spine.Unity;
using UnityEngine;

namespace BubblesShot
{
    public class SpineAnim : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ChangeSkinColor(Color color)
        {
            skeletonAnimation.skeleton.SetColor(color);
        }
    }
}
