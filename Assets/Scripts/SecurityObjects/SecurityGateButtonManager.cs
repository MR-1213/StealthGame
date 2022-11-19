using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityGateButtonManager : MonoBehaviour
{
    public bool isPush;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isPush = true;
        }
    }
}
