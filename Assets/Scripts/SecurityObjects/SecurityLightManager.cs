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
    private bool isRotation = false;
    private bool alertEnter = true;

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
        if(isAlert)
        {
            if(alertEnter)
            {
                audioSource.Play();
                entrance_Lamp.transform.DORotate(new Vector3(0,360,0), 1, RotateMode.LocalAxisAdd)
                                    .SetLoops(-1, LoopType.Restart)
                                    .SetLink(entrance_Lamp.gameObject);
                safe_Lamp.transform.DORotate(new Vector3(0,360,0), 1, RotateMode.LocalAxisAdd)
                                    .SetLoops(-1, LoopType.Restart)
                                    .SetLink(safe_Lamp.gameObject);
                alertEnter = false;
            }

            isRotation = true;
        }
        else if(isRotation && entrance_Lamp != null)
        {
            audioSource.Stop();
            Destroy(entrance_Lamp.gameObject);
            Destroy(safe_Lamp.gameObject);
        }
    }
}
