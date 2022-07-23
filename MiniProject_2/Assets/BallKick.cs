using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallKick : MonoBehaviour
{
    public float Power = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Ball") {
            Debug.Log("Ball Touch");
            Vector3 direction = other.transform.position - this.transform.position;
            other.GetComponent<Rigidbody>().AddForce(direction.normalized * Power);
        
    }
}

}
