using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
* Type: Class
* 
* Name: ScrollingTexture
*
* Author: Sara Burton
*
* Purpose: Makes texture scroll on elevator trim
* 
* Functions:   void Update()
* 
* References: Script taken from Game Art: How to scroll UVs or How to animate tile able textures in Unity !
* found at https://www.youtube.com/watch?v=YOF4aHV3ALo. Date Accessed: 02/06/22
* 
* See Also: 
*
* Change Log:
* Date         Initials    Version     Comments
* 02/06/22     SB          1.0         Initial creation
* 29/07/22     WH          1.1         Added header
****************************************************************************************************/
public class ScrollingTexture : MonoBehaviour
{
    public float scrollx = 0.5f;
    public float scrolly = 0.5f;

    void Update()
    {
        float offsetx = Time.time * scrollx;
        float offsety = Time.time * scrolly;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetx, offsety);
    }
}
