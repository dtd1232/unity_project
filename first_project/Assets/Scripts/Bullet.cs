using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 0, bulletSpeed * Time.deltaTime));
        Destroy(this.gameObject, 10.0f); // 10초뒤에 총알 사라짐
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Target"){
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
