using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
* Type: Class
* 
* Name: EmergencyLight
*
* Author: Sara Burton
*
* Purpose: Spinning light in end room
* 
* Functions:   void Update()
* 
* References: 
* 
* See Also: 
*
* Change Log:
* Date         Initials    Version     Comments
* 25/06/22     SB          1.0         Initial creation
* 29/07/22     WH          1.1         Added header
****************************************************************************************************/
public class EmergencyLight : MonoBehaviour
{

    public float spinValue = 5f;
    public bool quit = false;

    void Update()
    {
        if (!quit)
        {
            this.gameObject.GetComponent<Light>().enabled = true;
            this.gameObject.GetComponent<AudioSource>().mute = false;
            transform.Rotate(0f, spinValue, 0f, Space.World);
        }
        else
        {
            this.gameObject.GetComponent<Light>().enabled = false;
            this.gameObject.GetComponent<AudioSource>().mute = true;
        }
    }
}
