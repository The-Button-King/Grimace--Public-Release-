using UnityEngine;
using TMPro;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: DamageEffect
 *
 * Author:  Joseph Gilmore 
 *
 * Purpose: Create text effect to show damage delt to AI 
 * 
 * Functions:       
 *              private void Awake()
 *              private void OnEnable()
 *              public void CreateDamageText( float dmg, bool crit,Vector3 pos )
 *              private void Update()
 *              private void ScaleText()
 *              private void MoveText()
 *              private void FadeText()
 *              
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 06/07/2022    JG          1.00        - Created a basic version of the text
 * 22/07/2022    JG          1.01        - Round down dmg text
 * 24/07/2022    JG          1.02        - Fixed if dmg is less than one 
 ****************************************************************************************************/
[RequireComponent(typeof(TextMeshPro))]
public class DamageEffect : MonoBehaviour
{
    private const float         m_maxScaleTime  = 1.0f;                             // Length of the scale time 
    private Vector2             m_moveMutiplerRange = new Vector2(6.0f, 8.0f);      // Random Move range
    private Vector2             m_textRange = new Vector2(1.1f, 1.5f);              // More random values to make text fly out differently
    private TextMeshPro         m_damageText;                                       // Reference to text mesh pro
    private float               m_scaleTimer;                                       // Time to track scaling 
    private Color               m_textColor;                                        // Stores text colour 
    private Vector3             m_moveVector;                                       // Stores vector to move text tp  
    private static int          m_sortingOrder;                                     // Static int to layer the text in front of each other   
    private AssetPool           m_assetPool;                                        // Rerefence to asset pool 
    private float               m_moveVectorMutipler;                               // Value that mutiplys the move vector   
    private float               m_normalFontSize = 8.0f;                            // Default font size  
    private float               m_critFontSize = 18.0f;                             // Font size for crit hits
    private Color               m_normalColor = Color.yellow;                       // Default text colour 
    private Color               m_critColor = Color.red;                            // Crit hit colour 
    private Vector2             m_snapRange = new Vector2(5.0f, 9.0f);              // Snap mutiplyer range  
    private float               m_snapMultiplier;                                   // Snap mutipler   
    private const float         k_scaleAmount = 1.0f;                               // Scale amount  
    private float               m_fadeSpeed = 2.0f;                                 // Speed the fade text  
    private float               m_interperlationValue = 0.0f;                       // Lerp value 

    /***************************************************
    *   Function        : Awake  
    *   Purpose         : Get references 
    *   Parameters      : N/A
    *   Returns         : void    
    *   Date altered    : 03/08/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Awake()
    {
        // Get references 
        m_damageText = GetComponent<TextMeshPro>();
        m_assetPool = GameObject.Find("Pool").GetComponent<AssetPool>();    
    }
    /***************************************************
    *   Function        : OnEnable   
    *   Purpose         : Setup damage text 
    *   Parameters      : N/A
    *   Returns         : void    
    *   Date altered    : 24/07/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
        // Reset text
        m_damageText.transform.localScale = Vector3.one;
        m_damageText.color = Color.yellow;

    }
    /***************************************************
    *   Function        : CreateDamageText   
    *   Purpose         : Setup text with the correct vars    
    *   Parameters      : float dmg, bool crit,Vector3 pos 
    *   Returns         : void    
    *   Date altered    : 24/07/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void CreateDamageText( float dmg, bool crit,Vector3 pos )
    {
        int dmgText = ( int )Mathf.Floor( dmg );

        // If dmg text rounds down to 0 
        if( dmgText == 0 )
        {
            // Set damage to text
            m_damageText.SetText( dmg.ToString() );
        }
        else
        {
            // Set damage to text
            m_damageText.SetText( dmgText.ToString() );
        }
      

        // Set Position of text
        transform.position = pos;

        // Reset interplation
        m_interperlationValue = 0f;
        
        if ( crit )
        {
            // If its a crit change font size 
            m_damageText.fontSize = m_critFontSize;

            // set colour 
            m_textColor = m_critColor;
        }
        else
        {
            // Set text to normal
            m_damageText.fontSize = m_normalFontSize;
            m_textColor = m_normalColor;
        }

        // Set fade time 
        m_scaleTimer = m_maxScaleTime;

        m_damageText.color = m_textColor;

        // Create a random move mutipler vector 
        m_moveVectorMutipler = Random.Range( m_moveMutiplerRange.x, m_moveMutiplerRange.y );
        m_moveVector = new Vector3( Random.Range( m_textRange.x,m_textRange.y ), Random.Range( m_textRange.x, m_textRange.y ), Random.Range( m_textRange.x, m_textRange.y )) * m_moveVectorMutipler;

        
        // Lay over the top of last text by increasing sorting order 
        m_sortingOrder++;
        m_damageText.sortingOrder = m_sortingOrder;
    }
    /***************************************************
    *   Function        : Update   
    *   Purpose         : Move the text in a pattern   
    *   Parameters      : N/A  
    *   Returns         : Void    
    *   Date altered    : 05/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Update()
    {
        // Reduce face timer
        m_scaleTimer -= Time.deltaTime;

        // Update text to animate it 
        MoveText();
        ScaleText();
        FadeText();
       
    }
      /***************************************************
      *   Function        : ScaleText  
      *   Purpose         : Increase & decrease the size of the text 
      *   Parameters      : N/A  
      *   Returns         : Void    
      *   Date altered    : 05/07/2022
      *   Contributors    : JG
      *   Notes           :    
      *   See also        :    
      ******************************************************/
    private void ScaleText()
    {
        // If half the scale time not completed
        if (m_scaleTimer > m_maxScaleTime * 0.5f)
        {
            // Increase local scale 
            transform.localScale += Vector3.one * k_scaleAmount * Time.deltaTime;

        }
        else
        {
            // Decrease local scale 
            transform.localScale -= Vector3.one * k_scaleAmount * Time.deltaTime;
        }
    }
     /***************************************************
      *   Function        : MoveText
      *   Purpose         : Move the text upwards in a snaping fasion
      *   Parameters      : N/A  
      *   Returns         : Void    
      *   Date altered    : 05/07/2022
      *   Contributors    : JG
      *   Notes           :    
      *   See also        :    
      ******************************************************/
    private void MoveText()
    {
        //Move text by the move transform
        transform.position += m_moveVector * Time.deltaTime;

        // Get a random snap mutiplier 
        m_snapMultiplier = Random.Range( m_snapRange.x, m_snapRange.y );

        // Alter move transform to create snap effect
        m_moveVector -= m_moveVector * m_snapMultiplier * Time.deltaTime;

        // Get the text to face the camera 
        transform.rotation = Camera.main.transform.rotation;
    }
    /***************************************************
      *   Function        : FadeText 
      *   Purpose         : Ease the text alpha until its faded
      *   Parameters      : N/A  
      *   Returns         : Void    
      *   Date altered    : 05/07/2022
      *   Contributors    : JG
      *   Notes           :    
      *   See also        :    
      ******************************************************/
    private void FadeText()
    {
        // If not completed fade Interperlate
        m_interperlationValue += m_fadeSpeed * Time.deltaTime;

        // Lerp the alpha by easying in and out
        m_textColor.a = Mathf.SmoothStep(m_textColor.a, 0, m_interperlationValue);

        // Set the text colour to new alpha
        m_damageText.color = m_textColor;

        // If alpha hit 0 text faded 
        if ( m_textColor.a <= 0 )
        {
         // Return text to pool
          m_assetPool.ReturnObjectToPool(gameObject);
        }
    }
}
