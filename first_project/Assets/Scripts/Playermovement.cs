using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float fireCoolTime;
    private PlayerManager playerManager;
    private Rigidbody rigid; // 객체
    public Animator playerAnim;
    public GameObject bullet;   // bullet instance
    public Transform firePosition;  // bullet position

    // 3인칭 이동 구현
    public bool useCharacterForward = false;
    public bool userCameraForward = true;
    public float turnSpeed = 10.0f;
    private KeyCode sprintKey = KeyCode.LeftShift;  // 달리기 키를 좌 시프트로 설정

    private float turnSpeedMultiplier;  // 도는 속도 계산
    private float direction = 0.0f; // 방향
    private bool isSprinting = false;   // 스프린트 중인지 상태 확인
    private Vector3 targetDirection;
    private Vector2 input;
    private Quaternion freeRotation;
    private Camera mainCamera;  //메인 카메라 호출
    private float velocity;

    // Start is called before the first frame update
    // Awake는 비활성화 되어있어도 무조건 실행
    // start는 스크립트가 활성화 된 상태여야 실행됨, 비활성화되면 밑에 있는 코드들은 자동으로 비활성화.
    void Awake()
    {
        mainCamera = Camera.main;
        playerManager = GetComponent<PlayerManager>();
        rigid = GetComponent<Rigidbody>();  // Rigidbody component를 가져올 것이다
        speed = 0.5f;
    }

    private void Start()
    {
        StartCoroutine(Fire(fireCoolTime)); // 변수로 지정을 해주면 변수 값만 바꿔서 코드를 재사용할 수 있음.
    }

    // Update is called once per frame
    // Update: 매 프레임마다 호출됨, Update의 종류마다 실행 우선순위가 다름
    private void FixedUpdate()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        UpdateTargetDirection();    //일정 시간마다 업데이트 해주기 위해 fixedupdate에 넣음
        if(input != Vector2.zero && targetDirection.magnitude > 0.1f)   // if, 이동할 떄+targetdirection에 변화가 생겼을 때
        {  
            Vector3 lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);    // 변화된 방향값을 freeRatiation에 저장, 속력이 아닌, 방향이 중요해서 freeRotation에 방향만 저장함.

            var differenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;  // 앞방향을 결정짓는건 y축이 얼마나 회전하느냐, free: 카메라가 보는 방향, transform: 캐릭터가 보는 방향
            var eulerY = transform.eulerAngles.y;

            if(differenceRotation < 0 || differenceRotation > 0)    // rotation 할 때
            {
                eulerY = freeRotation.eulerAngles.y;
            }

            var euler = new Vector3(0, eulerY, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier /* * Time.deltaTime */); // Slerp는 점차적으로 움직이게 함, fixed update이니까 deltaTime을 추가하지 않아도 됨
            // 점진적으로 하기 위해서는 coroutine 필요, Slerp에 coroutine이 포함되어 있음
        }

        if(input.x == 0 && input.y == 0){
            playerAnim.SetBool("isWalkBool", false);
        }else{
            playerAnim.SetBool("isWalkBool", true);
        }

        //transform.Translate(new Vector3(h, 0, v));
        rigid.AddForce(targetDirection * speed, ForceMode.Impulse); // ForceMode: 물리법칙이 적용된 움직임
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !playerManager.isJump)
        {

            playerManager.isJump = true;
            playerAnim.SetTrigger("isJumpTrigger");
            rigid.AddForce(new Vector3 (0, jumpPower, 0), ForceMode.Impulse);

        }

        // if(Input.GetMouseButtonDown(0) && !playerManager.isJump){
            // // parameter1: 무슨 객체 생성
            // // 2: 어디에 생성(position)
            // // 3: 어떤 방향으로 생성
            // Instantiate(bullet, firePosition.position, Quaternion.identity);    // object를 반복적으로 호출해서 사용하고 싶을때
            // 총알 발사 딜레이 계싼 위해서는 enumarate 사용: 함수 자체가 coroutine 실행부터 
        // }

        // if(Input.GetKeyDown(KeyCode.F)){
        //     playerAnim.SetTrigger("isFlyTrigger");  // 작동 방식이 1개임, True-False
        //     playerAnim.SetBool("isFlyBool", true);  // 작동 방식 설정 가능 True or False
        // }
    }

    public void UpdateTargetDirection()
    {
        if(!useCharacterForward)
        {
            turnSpeedMultiplier = 1.0f;

            // forward: 카메라가 현재 바라보는 방향
            var forward = mainCamera.transform.TransformDirection(Vector3.forward); // 메인카메라에서 방향을 받아와서 forward에 저장함
            forward.y = 0;

            var right = mainCamera.transform.TransformDirection(Vector3.right); // 카메라에서 오른쪽 방향 가져오기
            
            targetDirection = input.x * right + input.y * forward;  // vector2값이라서 vector3에서 z값이 vector2에서 y로 들어감
        }
    }

    private IEnumerator Fire(float coolTime)
    {
        // coroutine안에서는 반복문 실행해도 괜찮음. 내가 가져간 프레임 return
        while(true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Instantiate(bullet, firePosition.position, Quaternion.identity);
                yield return new WaitForSeconds(coolTime);  // cycle과 별도로 작동하게 하기 위해서 사용, update함수는 작동하는데 coroutine은 쉬는 상태
            }

            // 다음 프레임으로 넘겨주는 역할
            yield return null;  // update가 coroutine이랑 같이 돌아가는거처럼 보임
        }
    }
}
