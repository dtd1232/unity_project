using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    private PlayerManager playerManager;
    private Rigidbody rigid; // 객체
    // Start is called before the first frame update
    // Awake는 비활성화 되어있어도 무조건 실행
    // start는 스크립트가 활성화 된 상태여야 실행됨, 비활성화되면 밑에 있는 코드들은 자동으로 비활성화.
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rigid = GetComponent<Rigidbody>();  // Rigidbody component를 가져올 것이다
        speed = 0.5f;
    }

    // Update is called once per frame
    // Update: 매 프레임마다 호출됨, Update의 종류마다 실행 우선순위가 다름
    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //transform.Translate(new Vector3(h, 0, v));
        rigid.AddForce(new Vector3 (h, 0, v)*speed, ForceMode.Impulse); // ForceMode: 물리법칙이 적용된 움직임
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !playerManager.isJump){

            playerManager.isJump = true;
            rigid.AddForce(new Vector3 (0, jumpPower, 0), ForceMode.Impulse);

        }
    }

}
