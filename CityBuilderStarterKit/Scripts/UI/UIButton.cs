using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;


namespace CBSK
{
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {

        public AudioClip audioClip;

        public Vector3 pressedScale = new Vector3(1.05f, 1.05f, 1.05f);
        public float pressedDuration = .2f;

        public new bool enabled = true;

        private AudioSource audioSource;

        void Start()
        {
            if (audioClip != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
            }
        }

        public virtual void Click()
        {
            throw new NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopAllCoroutines();
            if (enabled)
            {
                StartCoroutine(Scale(pressedScale));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (enabled)
            {
                if (audioClip != null)
                {
                    //Play audio
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                ScaleDown();
                Click();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ScaleDown();
        }

        private void ScaleDown()
        {
            StopAllCoroutines();
            StartCoroutine(Scale(Vector3.one));
        }

        IEnumerator Scale(Vector3 scaleTo)
        {
            while (transform.localScale != scaleTo)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, scaleTo, 1 / pressedDuration * Time.deltaTime);
                yield return null;
            }
        }

    }
}