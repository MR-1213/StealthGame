using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SecurityLightManager : MonoBehaviour
{
    private bool isAlert;

    private void Start() 
    {
        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity:800, sequencesCapacity:200);
    }
    public void Alert()
    {
        if(this.gameObject.name == "RotationLight")
        {
            isAlert = true;
        }
    }

    public void DisableAlert()
    {
        isAlert = false;
    }

    private void Update() 
    {
        if(isAlert) transform.DORotate(new Vector3(0,360,0), 5, RotateMode.LocalAxisAdd);
    }
}
