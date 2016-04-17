using UnityEngine;
using System.Collections;

/**
 * Exmaple if an animated view of an occupant.
 */
namespace CBSK
{
    public class SimpleOccupantAnimator : MonoBehaviour
    {

        public string walkNorthAnimation;
        public string walkSouthAnimation;
        public string attackNorthAnimation;

        public Animator anim;

        /// <summary>
        /// Show sprite and start the animation.
        /// </summary>
        public void Show()
        {
            StartCoroutine("DoAnimation");
        }

        /// <summary>
        /// Stop the animation and hide sprite.
        /// </summary>
        public void Hide()
        {
            StopCoroutine("DoAnimation");
        }

        /// <summary>
        /// Do the animation.
        /// </summary>
        virtual protected IEnumerator DoAnimation()
        {
            // Random delay
            yield return new WaitForSeconds(Random.Range(0.0f, 0.75f));

            while (true)
            {
                // North
                anim.Play(walkNorthAnimation);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                // Attack
                anim.Play(attackNorthAnimation);
                yield return new WaitForSeconds(3.0f);
                // South
                anim.Play(walkSouthAnimation);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }
        }
    }
}