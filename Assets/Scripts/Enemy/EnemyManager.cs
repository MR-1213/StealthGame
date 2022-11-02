using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    
    public Transform point_1;
    public Transform point_2;

    NavMeshAgent navMeshAgent;
    Animator animator;

    enum State
    {
        MoveToPoint_1,
        MoveToPoint_2,
        DoNothing,

    }

    State currentState = State.MoveToPoint_1;
    State targetState = State.DoNothing;
    bool stateEnter = false;
    float stateTime = 0;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        ChangeState(State.MoveToPoint_1);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        stateEnter = true;
        stateTime = 0;
        Debug.Log(currentState.ToString());
    }



    // Update is called once per frame
    void Update()
    {
        stateTime += Time.deltaTime;
        float speed = navMeshAgent.velocity.magnitude;

        animator.SetFloat("EnemySpeed", speed);

        switch (currentState)
        {
            case State.MoveToPoint_1: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_1.position);
                    navMeshAgent.speed = 1.7f;
                    targetState = State.DoNothing;
                }
                
                break;
            }
            
            case State.MoveToPoint_2: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_2.position);
                    navMeshAgent.speed = 2.5f;
                    targetState = State.DoNothing;
                }
                
                break;
            }

            case State.DoNothing: {

                break;
            }
        }
    }
}
