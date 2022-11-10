using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
                    return;
                }
            }

            if(angle <= searchAngle)
            {
                transform.rotation = Quaternion.LookRotation(playerDirection);
                double eachDistance = Math.Sqrt(Math.Pow(playerDirection.x, 2) + Math.Pow(playerDirection.z, 2));
                if(eachDistance < (searchArea.radius / 2.0f))
                {
                    enemyManager.isChasing = false;
                    enemyManager.isAttacking = true;
                }
                else
                {
                    enemyManager.isAttacking = false;
                    enemyManager.isChasing = true;
                }
            }
            else
            {
                enemyManager.isChasing = false;
                enemyManager.isAttacking = false;
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            enemyManager.isChasing = false;
        }    
    }
}
