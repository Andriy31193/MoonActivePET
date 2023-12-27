using UnityEngine;

public class VillageBuilder : MonoBehaviour
{
    public GameObject[] Build(Transform parent, GameObject prefab, int count)
    {
        GameObject[] result = new GameObject[count];
        for(int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab, parent, false);
            result[i] = go;
        }

        return result;
    }
}
