using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 offset;

    void Awake()
    {
        // transform position: 내 위치
        // playerTransform posㅑtion: 지정한 객체의 위치
        offset = transform.position - playerTransform.position;    // position은 vector3로 반환
        // 벡터 방향과 크기를 구함
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerTransform.position + offset;
    }
}
