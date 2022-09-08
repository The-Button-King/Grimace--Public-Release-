using UnityEngine;
using System.Collections.Generic;
/****************************************************************************************************
* Type: Class
* 
* Name: AssetPool
*
* Author: Joseph Gilmore
*
* Purpose: To manage a pool of prefabs to increase optimization
* 
* Functions:        private void Start()
*                   private void PopulatePool()
*                   public GameObject GetObjectFromPool( GameObject gameObjectRequried )
*                   public GameObject GetObjectFromPool( GameObject gameObjectRequried, Vector3 pos )
*                   public GameObject GetObjectFromPool( GameObject gameObjectRequried, Vector3 pos, Quaternion rotation )
*                   private GameObject CreateGameObject(GameObject gameObject)
*                   private GameObject CreateGameObject( GameObject gameObject, Vector3 pos )
*                   private GameObject CreateGameObject( GameObject gameObject, Vector3 pos, Quaternion rotation )
*                   public void ReturnObjectToPool(GameObject gameObject)
*                   
* References:
* 
* See Also: See Prefab called "Pool" to see how objects to pool are assigned 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/05/2022    JG          1.00        - Created the class 
* 30/05/2022    JG          1.01        - Made changes to fix bugs 
* 
* 15/06/2022    WH          1.02        - Added position overloads for get object functions
* 23/06/2022    WH          1.03        - Added rotation overloads
* 24/06/2022    WH          1.04        - Pooled objects now call their reset functions
* 03/07/2022    WH          1.05        - Removed Resetable as object now do it in OnEnable
* 24/07/2022    JG          1.06        - Added features to populate pool at the begining of game using struct to apply amount through inspector 
****************************************************************************************************/
public class AssetPool : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> m_assetPool = new Dictionary<string, Queue<GameObject>>(); // Dictornary to store queues of different asset prefabs for pooling 
    
   
   [System.Serializable]
   private struct ObjectToPool                                                                              // Used to be able to assign the object to pool and how many to pool , group them togther and assign values via inspector 
   {
        [SerializeField]
        public GameObject poolObject;                                                                       // Object going to be pooled
        [SerializeField][Range(1,150)]
        public int       amountToPool;                                                                      // Amount want to pool
   }
  
    [SerializeField]
    private List<ObjectToPool> m_pools = new List<ObjectToPool>();                                          // List of items to pool
    /***************************************************
     *   Function        : Start 
     *   Purpose         : setup class 
     *   Parameters      : N/A 
     *   Returns         : Void   
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void Start()
    {
       // Add all items to pool 
       PopulatePool();
       
    }
    /***************************************************
     *   Function        : PopulatePool   
     *   Purpose         : Fill up the pool with all desired objects  
     *   Parameters      : N/A 
     *   Returns         : Void   
     *   Date altered    : 24/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void PopulatePool()
    {
        // Loop through each object that needs to be in the pool
        foreach(ObjectToPool objectType in m_pools )
        {
            for(int i = 0; i < objectType.amountToPool; i++ )
            {
                // If first item ensure to create a slot in the dictonary 
                if(i == 0 )
                {
                    ReturnObjectToPool( GetObjectFromPool( objectType.poolObject ) );
                }
                else
                {
                    // Add item to pool
                    ReturnObjectToPool( CreateGameObject( objectType.poolObject ) );
                }
                
            }
        }
    }
    /***************************************************
     *   Function        : GetObjectFromPool    
     *   Purpose         : To retrevive an object from the pool or create a new one if one does not exist 
     *   Parameters      : GameObject gameObjectRequried    
     *   Returns         : GameObject prefab  
     *   Date altered    : 03/07/22
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
    *****************************************************/
    public GameObject GetObjectFromPool( GameObject gameObjectRequried )
    {
        // Check to see if the pool contains an existing queue of that object name
        if ( m_assetPool.TryGetValue(gameObjectRequried.name, out Queue<GameObject> gameObjectQueue ))
        {
            // If a Queue exist but is empty 
            if( gameObjectQueue.Count == 0 )
            {
                // Create a new gameobject
                return CreateGameObject(gameObjectRequried);
            }
            else
            {
                // If a queue exists and is popluated with unactive game object
                // Remove object that needs actictvating & assign it
                GameObject prefab = gameObjectQueue.Dequeue();

                // Activate object as now needed
                prefab.SetActive( true );

                // Return requried object
                return prefab;
               
            }
        }
        else
        {
            // If no queue exist for that object name create a new one
            return CreateGameObject( gameObjectRequried );
        }
      
        
    }


    /***************************************************
     *   Function        : GetObjectFromPool    
     *   Purpose         : To retrevive an object from the pool or create a new one if one does not exist 
     *   Parameters      : GameObject gameObjectRequried
     *                     Vector3 pos
     *   Returns         : GameObject prefab  
     *   Date altered    : 03/07/22
     *   Contributors    : JG, WH
     *   Notes           : Overload with position 
     *   See also        :    
    *****************************************************/
    public GameObject GetObjectFromPool( GameObject gameObjectRequried, Vector3 pos )
    {
        // Check to see if the pool contains an existing queue of that object name
        if ( m_assetPool.TryGetValue( gameObjectRequried.name, out Queue<GameObject> gameObjectQueue ) )
        {
            // If a Queue exist but is empty 
            if ( gameObjectQueue.Count == 0 )
            {
                // Create a new gameobject
                return CreateGameObject( gameObjectRequried, pos );
            }
            else
            {
                // If a queue exists and is popluated with unactive game object
                // Remove object that needs actictvating & assign it
                GameObject prefab = gameObjectQueue.Dequeue();

                // Activate object as now needed
                prefab.SetActive( true );
                prefab.transform.position = pos;

                // Return requried object
                return prefab;

            }
        }
        else
        {
            // If no queue exist for that object name create a new one
            return CreateGameObject( gameObjectRequried, pos );
        }


    }

    /***************************************************
     *   Function        : GetObjectFromPool    
     *   Purpose         : To retrevive an object from the pool or create a new one if one does not exist 
     *   Parameters      : GameObject gameObjectRequried
     *                     Vector3 pos
     *                     Quaternion rotation
     *   Returns         : GameObject prefab  
     *   Date altered    : 03/07/22
     *   Contributors    : JG, WH
     *   Notes           : Overload with position and rotation
     *   See also        :    
    *****************************************************/
    public GameObject GetObjectFromPool( GameObject gameObjectRequried, Vector3 pos, Quaternion rotation )
    {
        // Check to see if the pool contains an existing queue of that object name
        if ( m_assetPool.TryGetValue( gameObjectRequried.name, out Queue<GameObject> gameObjectQueue ) )
        {
            // If a Queue exist but is empty 
            if ( gameObjectQueue.Count == 0 )
            {
                // Create a new gameobject
                return CreateGameObject( gameObjectRequried, pos, rotation );
            }
            else
            {
                // If a queue exists and is popluated with unactive game object
                // Remove object that needs actictvating & assign it
                GameObject prefab = gameObjectQueue.Dequeue();

                // Activate object as now needed
                prefab.SetActive( true );
                prefab.transform.position = pos;
                prefab.transform.rotation = rotation;

                // Return requried object
                return prefab;

            }
        }
        else
        {
            // If no queue exist for that object name create a new one
            return CreateGameObject( gameObjectRequried, pos, rotation );
        }


    }

    /***************************************************
     *   Function        : CreateGameObject   
     *   Purpose         : To create a new instance of a game object required and alter its name to fit the key   
     *   Parameters      : GameObject gameObject   
     *   Returns         : GameObject prefab 
     *   Date altered    : 30/05/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    private GameObject CreateGameObject( GameObject gameObject )
    {
        // Create desired prefab from reference 
        GameObject prefab = Instantiate( gameObject );

        // Change name of prefab to the origrinal name so it can be found in the dictonary by its origrinal name instead of the name(clone) etc
        prefab.name = gameObject.name;
     

        return prefab;
    }

    /***************************************************
     *   Function        : CreateGameObject   
     *   Purpose         : To create a new instance of a game object required and alter its name to fit the key   
     *   Parameters      : GameObject gameObject
     *                     Vector3 pos
     *   Returns         : GameObject prefab 
     *   Date altered    : 15/06/2022
     *   Contributors    : JG, WH
     *   Notes           : Overload with position
     *   See also        :    
    ******************************************************/
    private GameObject CreateGameObject( GameObject gameObject, Vector3 pos )
    {
        // Create desired prefab from reference 
        GameObject prefab = Instantiate( gameObject, pos, Quaternion.identity );

       

        // Change name of prefab to the origrinal name so it can be found in the dictonary by its origrinal name instead of the name(clone) etc
        prefab.name = gameObject.name;


        return prefab;
    }

    /***************************************************
     *   Function        : CreateGameObject   
     *   Purpose         : To create a new instance of a game object required and alter its name to fit the key   
     *   Parameters      : GameObject gameObject
     *                     Vector3 pos
     *                     Quaternion rotation
     *   Returns         : GameObject prefab 
     *   Date altered    : 23/06/2022
     *   Contributors    : JG, WH
     *   Notes           : Overload with position and rotation
     *   See also        :    
    ******************************************************/
    private GameObject CreateGameObject( GameObject gameObject, Vector3 pos, Quaternion rotation )
    {
        // Create desired prefab from reference 
        GameObject prefab = Instantiate( gameObject, pos, rotation );

        // Change name of prefab to the origrinal name so it can be found in the dictonary by its origrinal name instead of the name(clone) etc
        prefab.name = gameObject.name;


        return prefab;
    }
    /***************************************************
    *   Function        : ReturnObjectToPool
    *   Purpose         : Return object to pool and deactivate it  
    *   Parameters      : GameObject gameObject
    *   Returns         : Void 
    *   Date altered    : 23/06/2022
    *   Contributors    : JG
    *   Notes           : 
    *   See also        :    
   ******************************************************/
    public void ReturnObjectToPool( GameObject gameObject )
    {
        // Check Pool if queue exists for that object name
        if( m_assetPool.TryGetValue(gameObject.name,out Queue<GameObject> queue ))
        {
            // Add to queue of inactive objects of that type
            queue.Enqueue(gameObject);
           
        }
        else
        {
            // If no queue exists create one for that object
            Queue<GameObject> temp = new Queue<GameObject>();

            // Add new GameObject to its new list 
            temp.Enqueue( gameObject );

            // Add new pool to the dictonary with the key of the name of the gameobject it holds
            m_assetPool.Add( gameObject.name, temp );
        }

        // Set object to not active as its not currently used in the scene 
        gameObject.SetActive(false);
       
    }
}
 