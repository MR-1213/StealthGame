using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Linq;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    EnemySearchPlayer enemySearchPlayer;

    [Header("Destinations")]
    public Transform point_1;
    public Transform point_2;
    public Transform point_3;
    public Transform point_4;
    public Transform point_Toilet;
    public Transform point_Player;


    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public Text text; //デバッグ用のパラメータ表示テキスト

    /* ステートに関わるパラメータ */
    private State currentState = State.MoveToDestination;
    private State targetState = State.DoNothing;
    private Points currentPoint = Points.Point1;
    public bool isFounding;
    //敵に見つかった場合にtrueになる。コライダーの外に出るかChasingAndAttackingステートから出るとfalseになる。
    private bool stateEnter = false; //ステートに入った最初の1フレームだけtrueになる。
    private float stateTime = 0; //ステートに入ってからの経過時間。
    private float maxStandByTime = 3.0f; //EnemyがPlayerを見失った際に待機する時間。この時間が経過したのち、行動を再開する。
    public float standByTime = 0; //待機し始めてからの経過時間。

    Dictionary<Desire, float> desireDictionary = new Dictionary<Desire, float>();

    enum State
    {
        MoveToDestination,
        GoToToilet,
        ChasingAndAttacking,
        DoNothing,
    }

    enum Points
    {
        Point1,
        Point2,
        Point3,
        Point4,
    }

    enum Animation_State
    {
        Patrol = 0,
        Toilet = 1,
    }

    enum Desire
    {
        Toilet,
        ChaseAndAttack,
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newState">次のステート</param>
    private void ChangeState(State newState)
    {
        currentState = newState;
        stateEnter = true;
        stateTime = 0;
        Debug.Log(currentState.ToString());
    }

    public void ChangeToChasingAndAttackingState()
    {
        ChangeState(State.ChasingAndAttacking);
    }

    private void ChangePoint(Points newPoint)
    {
        currentPoint = newPoint;
        Debug.Log(currentPoint.ToString());
    }

    private void ChangeAnimationState(Animation_State state)
    {
        animator.SetInteger("ID", (int)state);
    }

    private void Start()
    {
        enemySearchPlayer = GetComponent<EnemySearchPlayer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        foreach (Desire desire in Enum.GetValues(typeof(Desire)))
        {
            desireDictionary.Add(desire, 0f);
        } 

        ChangeState(State.MoveToDestination);   
    }
    
    private void Update() 
    {
        stateTime += Time.deltaTime;
        float speed = navMeshAgent.velocity.magnitude;

        animator.SetFloat("EnemySpeed", speed);

        if(currentState != State.GoToToilet)
        {
            desireDictionary[Desire.Toilet] += Time.deltaTime / 1200.0f;
        }

        if(isFounding)
        {
            desireDictionary[Desire.ChaseAndAttack] = 100;
        }

        IOrderedEnumerable<KeyValuePair<Desire, float>> sortedDesire = desireDictionary.OrderByDescending(i => i.Value);

        text.text ="";
        foreach(KeyValuePair<Desire, float> sortedDesireElement in sortedDesire)
        {
            text.text += sortedDesireElement.Key.ToString() + ":" + sortedDesireElement.Value + "\n";
        }
        text.text += navMeshAgent.speed.ToString() + "\n";
        text.text += stateTime.ToString() + "\n";
        text.text += standByTime.ToString() + "\n";

        switch(currentState)
        {
            case State.MoveToDestination: {

                if(stateEnter)
                {
                    var topDesireElement = sortedDesire.ElementAt(0); //最も欲求の大きいものを取得

                    if(topDesireElement.Value >= 1.0f)
                    {
                        switch(topDesireElement.Key)
                        {
                            case Desire.Toilet:
                                navMeshAgent.SetDestination(point_Toilet.position);
                                navMeshAgent.speed = 3.0f;
                                targetState = State.GoToToilet;
                                break;
                            case Desire.ChaseAndAttack:
                                targetState = State.ChasingAndAttacking;
                                break;
                        }
                    }
                    else
                    {
                        switch(currentPoint)
                        {
                            case Points.Point1:
                                navMeshAgent.SetDestination(point_1.position);
                                break;
                            case Points.Point2:
                                navMeshAgent.SetDestination(point_2.position);
                                break;
                            case Points.Point3:
                                navMeshAgent.SetDestination(point_3.position);
                                break;
                            case Points.Point4:
                                navMeshAgent.SetDestination(point_4.position);
                                break;
                        }
                        navMeshAgent.speed = 2.0f;
                        targetState = State.DoNothing;
                    }
                }

                ChangeAnimationState(Animation_State.Patrol);

                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending) //目的地に到着したら
                {
                    ChangeState(targetState); //targetStateの行動を次の行動とする
                    return;
                }

                return;
            }

            case State.DoNothing: {

                if(stateTime >= 4.0f)
                {
                    ChangeState(State.MoveToDestination);
                    int nextPoint = (int)currentPoint + 1;
                    if(nextPoint >= Enum.GetNames(typeof(Points)).Length)
                    {
                        nextPoint = 0;
                    }
                    ChangePoint((Points)nextPoint);
                }

                return;
            }

            case State.GoToToilet: {

                if(stateEnter)
                {
                    navMeshAgent.enabled = false;
                    ChangeAnimationState(Animation_State.Toilet);
                    transform.position = point_Toilet.position;
                    transform.rotation = point_Toilet.rotation;
                }

                if(stateTime >= 4.0f)
                {
                    navMeshAgent.enabled = true;
                    desireDictionary[Desire.Toilet] = 0;
                    ChangeState(State.MoveToDestination);
                    return;
                }

                return;
            }

            case State.ChasingAndAttacking: {
                
                if(!isFounding)
                {
                    desireDictionary[Desire.ChaseAndAttack] = 0;

                    if(stateEnter)
                    {
                        navMeshAgent.SetDestination(point_Player.position);
                        Debug.Log("stateEnter");
                    } 
                    return;
                }

                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    standByTime += Time.deltaTime;
                    if(standByTime >= maxStandByTime && !enemySearchPlayer.isDetecting)
                    {
                        desireDictionary[Desire.ChaseAndAttack] = 0;
                        ChangeState(State.MoveToDestination);
                        isFounding = false;
                        standByTime = 0;
                        return;
                    }
                }
                else
                {
                    standByTime = 0;
                }

                Debug.Log(navMeshAgent.remainingDistance);
                navMeshAgent.SetDestination(point_Player.position);
                return;
            }
        }

    }

    private void LateUpdate() 
    {
        if(stateTime != 0)
        {
            stateEnter = false;
        }    
    }
}
