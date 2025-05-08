using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjetsDB", menuName = "ScriptableObjects/DB/ObjetsDB", order = 1)]
public class ObjectsDBSO : ScriptableObject
{
    public SerializedDictionary<int, ObjectsDataSO> objects;

    public ObjectsDataSO GetObject(int id)
    {
        if (objects.TryGetValue(id, out ObjectsDataSO objectsDataSO))
        {
            return objectsDataSO;
        }
        return null;
    }
}