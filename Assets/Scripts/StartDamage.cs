using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDamage : MonoBehaviour
{
    public float _amount = 50f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<HealthBar>().Damage(_amount);
        Destroy(this);
    }
}
