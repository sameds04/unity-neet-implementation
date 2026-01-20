using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    [SerializeField] GameObject food_prefab;
    List<GameObject> food_list;

    public void SpawnFoodGamobjects()
    {
        food_list = new List<GameObject>();

        for (int i = 0; i < 200; i++)
        {
            Vector3 position = new Vector3(Random.Range(-215f, 215f), 0.25f, Random.Range(-215f, 215f));
            GameObject food = Instantiate(food_prefab, position, Quaternion.identity);
            food_list.Add(food);
        }
    }

    public void SpawnFoodOnce()
    {
        Vector3 position = new Vector3(Random.Range(-215f, 215f), 0.25f, Random.Range(-215f, 215f));
        GameObject food = Instantiate(food_prefab, position, Quaternion.identity);
        food_list.Add(food);
    }

    public void DestroyFoodGameObjects()
    {
        foreach (GameObject food in food_list)
        {
            Destroy(food);
        }

        food_list.Clear();
    }

    public void RemoveFood(GameObject food)
    {
        food_list.Remove(food);
    }

}
