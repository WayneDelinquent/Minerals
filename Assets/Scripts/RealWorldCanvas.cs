using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldCanvas : MonoBehaviour
{
    Quaternion startingRotation;

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = gameObject.transform.rotation;   
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = startingRotation; 
    }
}
