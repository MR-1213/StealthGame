using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemySearchPlayer : MonoBehaviour
{
    EnemyManager enemyManager;

    [SerializeField]
    private SphereCollider searchArea;
    public float searchAngle = 120f;
    public LayerMask obstacleLayer;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += 1.0f;
        if(other.tag == "Player")
        {
            Vector3 playerDirection = other.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if(Physics.Raycast(rayCastOrigin, playerDirection, out hit, 5.0f, obstacleLayer))
            {
                if(hit.collider.CompareTag("Obstacle"))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    return;
                }
            }

            if(angle <= searchAngle)
            {
                enemyManager.ChangeMoveToPlayerState();
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            enemyManager.MissingPlayer();
        }    
    }

    #if UNITY_EDITOR
    //　サーチする角度表示
    private void OnDrawGizmos() {
        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
    }
    #endif
}
