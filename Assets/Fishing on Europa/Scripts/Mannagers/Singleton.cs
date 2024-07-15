using UnityEngine;

public class Singleton <T> : GameBehaviour where T : GameBehaviour
{
    private static T _Instance;
    public static T instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindFirstObjectByType<T>();
                if (_Instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    singleton.AddComponent<T>();
                }
            }
            return _Instance;
        }
    }

    protected virtual void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
