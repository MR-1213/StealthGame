using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using UnityEditor;

public class EnemySearchPlayer : MonoBehaviour
{
    EnemyManager enemyManager;
    NavMeshAgent navMeshAgent;
    Animator animator;

    [SerializeField]
    private SphereCollider searchArea;
    public Slider hpSlider;
    
    private Vector3 detectPosition;
    public float searchAngle = 120f;
    public LayerMask obstacleLayer;
    public bool isDetecting = false;
    private float interval;
    private int currentHP;
    private int maxHP = 20;
    private int damage = 1;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
        currentHP = maxHP;
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += 1.0f;
        if(other.CompareTag("Player") && navMeshAgent.enabled == true)
        {
            Vector3 playerDirection = other.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if(Physics.Raycast(rayCastOrigin, playerDirection, out hit, searchArea.radius, obstacleLayer))
            {
                if(hit.collider.CompareTag("Obstacle")) return;
            }

            enemyManager.ChangeToChasingAndAttackingState();
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        hpSlider.value = currentHP;
        RaycastHit hit;
        NavMeshHit navHit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += 1.0f;
        if(other.CompareTag("Player") && navMeshAgent.enabled == true)
        {
            isDetecting = true;
            Vector3 playerDirection = other.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if(Physics.Raycast(rayCastOrigin, playerDirection, out hit, searchArea.radius, obstacleLayer))
            {
                if(hit.collider.CompareTag("Obstacle")) return;
            }

            double eachDistance = Math.Sqrt(Math.Pow(playerDirection.x, 2) + Math.Pow(playerDirection.z, 2));

            if(angle <= searchAngle || eachDistance < 1.0f)
            {
                enemyManager.isFounding = true;
                transform.rotation = Quaternion.LookRotation(playerDirection);
                animator.SetInteger("ID", 0);

                if(NavMesh.SamplePosition(other.transform.position, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    //other.transform.position = navHit.position;
                }
                navMeshAgent.SetDestination(other.transform.position);
                navMeshAgent.speed = 3.5f;
            }

            if(angle <= searchAngle && eachDistance < (searchArea.radius / 2.0f))
            { 
                interval += Time.deltaTime;
                enemyManager.isFounding = true;
                transform.rotation = Quaternion.LookRotation(playerDirection);
                animator.SetInteger("ID", 0);
                if(NavMesh.SamplePosition(other.transform.position, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    //other.transform.position = navHit.position;
                }
                navMeshAgent.SetDestination(other.transform.position);

                if(interval < 1.0f) return;
                currentHP -= damage;
                hpSlider.value = currentHP;
                Debug.Log("攻撃");
                if(hpSlider.value == 0) hpSlider.value = 0;
                interval = 0;
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && navMeshAgent.enabled == true)
        {
            Vector3 lastPlayerPosition = other.transform.position;
            navMeshAgent.SetDestination(lastPlayerPosition);
            enemyManager.isFounding = false;
            isDetecting = false;
        }    
    }

    public void SetDetectPosition(RaycastHit hit)
    {
        navMeshAgent.SetDestination(hit.transform.position);
        navMeshAgent.speed = 3.5f;
        enemyManager.isFounding = true;
        enemyManager.ChangeToChasingAndAttackingState();
    }
}
