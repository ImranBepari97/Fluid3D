using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRocket : MonoBehaviour
{

    public float speed = 50f;
    public float lifetime = 15f;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (transform.forward * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;
        if(lifetime < 0) {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) {

        Destroy(this.gameObject);    
    }
}
