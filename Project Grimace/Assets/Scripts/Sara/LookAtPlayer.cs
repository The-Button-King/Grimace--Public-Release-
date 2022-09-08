using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
* Type: Class
* 
* Name: EmergencyLight
*
* Author: Sara Burton (with assistance from Joseph Gilmore)
*
* Purpose: Makes cameras look at player
* 
* Functions:   void Start()
*              void Update()
* 
* References: 
* 
* See Also: 
*
* Change Log:
* Date         Initials    Version     Comments
* 28/06/22     SB          1.0         Initial creation
* 29/07/22     WH          1.1         Added header
****************************************************************************************************/
public class LookAtPlayer : MonoBehaviour
{

    private GameObject player;
    private Transform myTransform;
    public float TurnSpeed = 50f;

    void Start()
    {
        player = GameObject.FindWithTag("Player"); // Finds player
        myTransform = GetComponent<Transform>(); // Finds itself
    }

    void Update()
    {
        var targetDirection = player.transform.position - myTransform.position; // finds the direction to look at

        // targetDirection.z = Mathf.Clamp(targetDirection.z, -5f, 5f);

        // Debug.Log(targetDirection);

        var targetRotation = Quaternion.LookRotation(targetDirection); // finds the rotation to look at

        var deltaAngle = Quaternion.Angle(myTransform.rotation, targetRotation); // Finds the amount it needs to rotate

        // Debug.Log(transform.rotation.y);

        if (deltaAngle == 0f)
        {
            return;
        }

        myTransform.rotation = Quaternion.Slerp(
            myTransform.rotation,
            targetRotation,
            TurnSpeed * Time.deltaTime / deltaAngle);
    }
}
