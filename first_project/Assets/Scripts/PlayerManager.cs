using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool isJump;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // private void OnCollisionEnter(Collision other){ // 객체의 충돌 감지
    //     // other.gameObject.name == "floor"일때 바닥 이름이 floor가 아니면 인식할 수 없음, floor여러개 생성했을 때 대처할 수 없음.
    //     // 이 부분을 해결하기 위해 tag 사용.
    //     if(other.gameObject.tag == "floor"){   // 바닥에 닿으면 isJump 초기화
    //         isJump = false;
    //     }
    // }

    private void OnTriggerEnter(Collider other){    // collite 끼리 만났느냐 아니냐.
        if(other.gameObject.tag == "Item"){
            gameManager.currentItem++;  //저장한 아이템 갯수 올려줌
            gameManager.GetItem();
            Destroy(other.gameObject);
        }else if(other.gameObject.tag == "EndPoint"){
            if(gameManager.currentItem == gameManager.totalItem){
                gameManager.ExitGame();
            }
        }
        
    }
}
