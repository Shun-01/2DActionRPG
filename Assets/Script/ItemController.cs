using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public int recoveryValue;

    [SerializeField]
    private float appearTime;

    public float waitTime;

    void Start()
    {
        Destroy(gameObject, appearTime);
    }

    // Update is called once per frame
    void Update()
    {
     if(waitTime > 0)
        {
            waitTime-= Time.deltaTime;
        }
    }
}
