using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiGoalKeeperMovement : MonoBehaviour
{
    private NavMeshAgent goalkeeper;
    public Animator aiAnim;
    private float dist;
    private float followThreshold;

    [SerializeField]
    Transform targetTransform;


    // Start is called before the first frame update
    void Start()
    {
        goalkeeper = GetComponent<NavMeshAgent>();  
    }

     private void FixedUpdate()
    {
        // if(aiAnim.GetBool("rightOrleft") == true){
        //     aiAnim.SetBool("rightOrleft", false);
        // }
        // else{
        //     aiAnim.SetBool("rightOrleft", true);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(targetTransform.position, transform.position);
        goalkeeper.SetDestination(targetTransform.position);
        // if(dist >= 2.0f && dist < 10.0f){
        //     enemyAgent.isStopped = false;
        //     enemyAgent.SetDestination(targetTransform.position);
        // }
        // else if(dist < 2.0f){
        //     aiAnim.SetBool("isWalkBool", false);
        //     aiAnim.SetTrigger("isAttackTrigger");
        // }
        // else{
        //     enemyAgent.isStopped = true;   
        // }
    }
}
