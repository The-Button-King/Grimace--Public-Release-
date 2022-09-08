using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Audio;

/****************************************************************************************************
* Type: Class
* 
* Name: LightFlickering
*
* Author: Sara Burton
*
* Purpose: Flickers lights in end room
* 
* Functions:   void Update()
*              IEnumerator FlickeringLight()
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
public class LightFlickering : MonoBehaviour
{

    private bool isActive = false;
    private float waitTime;
    public float earlyTime = 0.5f;
    public float lateTime = 2f;
    public Light[] lights;
    public bool quit = false;

    void Update()
    {
            StartCoroutine(FlickeringLight());
    }

    IEnumerator FlickeringLight()
    {
        // Preventing the co-routine to start again if its already running
        if (!isActive)
        {
            // stopping it running again
            isActive = true;

            // Check if the lights have been cut off
            if (quit)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].GetComponent<Light>().enabled = false;
                }

                // turn off sound
                this.gameObject.GetComponent<AudioSource>().mute = true;
                yield break;
            }

            // Setting all lights off
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].GetComponent<Light>().enabled = false;
            }
            // turn off sound
            this.gameObject.GetComponent <AudioSource> ().mute = true;

            // Find a random time to wait
            waitTime = Random.Range(earlyTime, lateTime);

            // wait that random time
            yield return new WaitForSeconds(waitTime);

            // Turn the lights back on
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].GetComponent<Light>().enabled = true;
            }
            // turn on sound
            this.gameObject.GetComponent <AudioSource> ().mute = false;

            // Find a random time to wait
            waitTime = Random.Range(earlyTime, lateTime);

            // wait that random time
            yield return new WaitForSeconds(waitTime);

            // come out the co-routine
            isActive = false;
        }
        else
        {
            yield break;
        }

        
    }
}
