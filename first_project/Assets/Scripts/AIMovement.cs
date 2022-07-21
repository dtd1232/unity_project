using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private float dist;
    public float followThreshold;
    public Animator enemyAnim;

    [SerializeField]    // 에디터에서 참조하고 사용할 수는 있는데, 다른 클래스에서는 사용할 수 없음
    public Transform targetTransform;
    // Start is called before the first frame update
    void Start()    // 작동할 때 캐릭터가 생성되도록 설정
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(targetTransform.position, transform.position);
        if(dist < 10.0f)
        {
            enemyAgent.isStopped = false;
            enemyAgent.SetDestination(targetTransform.position);
            enemyAnim.SetBool("isWalkBool", true);
            if(dist < 2.0f)
            {
                enemyAnim.SetTrigger("isAttackTrigger");
            }
        }else{
            enemyAgent.isStopped = true;
            enemyAnim.SetBool("isWalkBool", false);
        }
    }
}
