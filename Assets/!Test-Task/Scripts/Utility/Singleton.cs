using System;
using System.Collections;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Default;
    [SerializeField] private bool dontDestroyOnLoad = true;

    void Awake()
    {
        if (Default != null)
        {
            Destroy(gameObject);
            return;
        }

        Default = this as T;
        if(dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
        AwakeSingleton();
    }

    public virtual void AwakeSingleton()
    { }
}