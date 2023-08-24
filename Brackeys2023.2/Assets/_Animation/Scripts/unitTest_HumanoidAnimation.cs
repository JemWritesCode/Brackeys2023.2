using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using the Animation namespace created for this project
using Animation;

public class unitTest_HumanoidAnimation : MonoBehaviour
{

    humanoidAnimationStateController dummySC;
    [SerializeField] private float input_vel_fbw = 0.0f;
    [SerializeField] private float input_vel_lat = 0.0f;
    [SerializeField] private bool input_take_item = false;
    [SerializeField] private bool input_melee_punch = false;
    [SerializeField] private bool input_melee_roll = false;
    [SerializeField] private bool input_melee_punish = false;
    [SerializeField] private bool input_melee_interrupt_punish = false;
    [SerializeField] private bool input_melee_slam = false;

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

        // Execute once taking the item
        if (input_take_item)
        {
            dummySC.animateActionTakeItem();
            input_take_item = false;
        }

        // MELEE Punch
        if (input_melee_punch)
        {
            dummySC.animateMeleePunch();
            input_melee_punch = false;
        }

        // MELEE Roll Dodge
        if (input_melee_roll)
        {
            dummySC.animateMeleeRoll();
            input_melee_roll = false;
        }

        // MELEE Start Punish
        if (input_melee_punish)
        {
            dummySC.animateMeleeStartPunish();
            input_melee_punish = false;
        }

        // MELEE Interrupt the Start Punish
        if (input_melee_interrupt_punish)
        {
            dummySC.animateMeleeInterruptPunish();
            input_melee_interrupt_punish = false;
        }

        // MELEE Slam
        if (input_melee_slam)
        {

            dummySC.animateMeleeSlam();
            input_melee_slam = false;

        }

    }

}
