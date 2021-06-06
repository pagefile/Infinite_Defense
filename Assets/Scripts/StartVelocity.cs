using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVelocity : MonoBehaviour
{
    public Vector3 velocity = default;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
