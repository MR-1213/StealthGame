using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SecurityLightManager : MonoBehaviour
{
    private AudioSource audioSource;
    public Transform entrance_Lamp;
    public Transform safe_Lamp;
    private Tweener tweener;
    private bool isAlert;
    private float alertTime;

    private void Start() 
    {
        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity:800, sequencesCapacity:200);
        audioSource = GetComponent<AudioSource>();
    }
    public void Alert()
    {
        isAlert = true;
    }

    public void DisableAlert()
    {
        isAlert = false;
    }

    private void Update() 
    {
        alertTime += Time.deltaTime;
        if(isAlert)
        {
            tweener = entrance_Lamp.transform.DORotate(new Vector3(0,360,0), 5, RotateMode.LocalAxisAdd).SetLoops(-1).Play();
            safe_Lamp.transform.DORotate(new Vector3(0,360,0), 5, RotateMode.LocalAxisAdd).SetLoops(-1).Play();

            if(alertTime > 1.0f)
            {
                audioSource.PlayOneShot(audioSource.clip);
                alertTime = 0;
            }
        }
        else if(tweener != null)
        {
            tweener.Kill();
        }
    }
}
