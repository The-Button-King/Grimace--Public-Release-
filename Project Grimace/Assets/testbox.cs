using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackGroundMusic;
public class testbox : MonoBehaviour
{
    private void OnTriggerEnter( Collider other )
    {
        if(other.transform.tag == "Player" )
        {
            MusicManager.instance.ToggleCombatMusic( true);
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( other.transform.tag == "Player" )
        {
            MusicManager.instance.ToggleCombatMusic( false );
        }
    }
}
