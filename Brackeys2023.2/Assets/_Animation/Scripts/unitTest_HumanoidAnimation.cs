using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Animation;

public class unitTest_HumanoidAnimation : MonoBehaviour
{

    humanoidAnimationStateController dummySC;
    [SerializeField] private float input_vel_fbw = 0.0f;
    [SerializeField] private float input_vel_lat = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        dummySC = GetComponent<humanoidAnimationStateController>();
    }

    // Update is called once per frame
    void Update()
    {

        // Make the character execute a motion animation given user input
        dummySC.animateMotion( input_vel_fbw, input_vel_lat );        

    }

}
