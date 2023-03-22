
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField]
    private List<ActionBase> prefabs;

    private Dictionary<string, ObjectPool<ActionBase>> pooledObjects;

    #region Unity

    private void Awake()
    {
        pooledObjects = new Dictionary<string, ObjectPool<ActionBase>>();

        foreach (var prefab in prefabs)
        {
            var pool = new ObjectPool<ActionBase>(
                () => OnCreate(prefab), 
                (action) => action.OnGet(), 
                (action) => action.OnRelease());

            pooledObjects.Add(prefab.name, pool);
        }
    }

    #endregion

    #region Public

    public ActionBase Get(ActionBase prefab, Vector3 position, Quaternion rotation)
    {
        if (pooledObjects.TryGetValue(prefab.name, out ObjectPool<ActionBase> pooledObject))
        {
            var obj = pooledObject.Get();

            obj.transform.position = position;

            obj.transform.rotation = rotation;

            obj.gameObject.SetActive(true);

            return obj;
        }

        return null;
    }

    public void Release(ActionBase obj)
    {
        if (pooledObjects.TryGetValue(obj.name, out ObjectPool<ActionBase> pooledObject))
        {
            obj.gameObject.SetActive(false);

            pooledObject.Release(obj);
        }
    }

    #endregion

    #region Private

    private ActionBase OnCreate(ActionBase prefab)
    {
        var obj = Instantiate(prefab);

        obj.name = prefab.name;

        return obj;
    }

    #endregion
}