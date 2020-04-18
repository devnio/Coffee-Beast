using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{


    public bool dontDestroyBetweenScenes = false;

    static T m_instance;

    public static T Instance
    {
        get 
        {
            // When trying to get instance there is none (maybe Awake wasn't called), then search in the scene
            // If in the scene none was found create a new object for this and return it.
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<T>();

                if (m_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    m_instance = singleton.AddComponent<T>();
                }
            }

            return m_instance;
        }
    }
     
    // For populating the first m_intance.
    public virtual void Awake() 
    {
        if (m_instance == null)
        {
            m_instance = this as T;

            if (dontDestroyBetweenScenes)
            {
                transform.parent = null;
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
	
}
