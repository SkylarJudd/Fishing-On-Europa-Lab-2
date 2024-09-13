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
                _Instance = FindFirstObjectByType<T>();
                if (_Instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    _Instance = singleton.AddComponent<T>();
                    Debug.LogError($"no Instance of {singleton.name} in scene creating this object");
                }
            }
            return _Instance;
        }
    }

    public virtual void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
            Debug.Log($" Instance of {typeof(T).Name} Attached to {gameObject.name} being set to this");
        }
        else
        {
            Debug.LogError($"Second Instance of {gameObject.name} in scene destroying this object");
            Destroy(gameObject);
        }
    }
}
