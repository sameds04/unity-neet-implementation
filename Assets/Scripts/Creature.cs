using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private Vector3 direction_of_nearest_food;
    private Vector3 direction_of_nearest_creature;
    private Vector3 direction_of_nearest_wall;
    private GameObject nearest_creature;
    private float[] inputs;

    public GameObject ray_origin;
    public Network network;
    public int id;
    public int generation;
    public int age;
    public float health;
    public float health_gained;
    public float wall_penalty = 1f;

    private void Start()
    {
        inputs = new float[GlobalVariables.inputs];
        id = int.Parse(gameObject.name);
        health = GlobalVariables.starting_health;
        InvokeRepeating("DecreaseHealth", 0f, 1f);
        InvokeRepeating("Age", 0f, 1f);
    }

    private void Update()
    {
        UpdateComponentPositions();
        GetInputs();
        List<float> outputs = UpdateNetwork();
        MoveCreature(GetMaxOutput(outputs));
        CheckForWalls();

    }

    private int GetMaxOutput(List<float> outputs)
    {
        int max = 0;

        for (int i = 0; i < GlobalVariables.outputs; i++)
        {
            if (outputs[i] > outputs[max])
            {
                max = i;
            }
        }

        return max;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            health += 5;
            health_gained += 5;
            FindAnyObjectByType<Manager>().GetComponent<SpawnFood>().RemoveFood(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    private void Age()
    {
        age++;
    }

    private void DecreaseHealth()
    {
        health -= 1;

        if (health < 1)
        {
            Death();
        }
    }

    public void Death()
    {
        CancelInvoke("DecreaseHealth");
        CancelInvoke("Age");
        network.SetFitness(age);
        FindAnyObjectByType<Manager>().Death(gameObject);
        Destroy(ray_origin);
        Destroy(gameObject);
    }

    private void GetInputs()
    {
        Vector3[] directions = {
        ray_origin.transform.up,                 // Forward
        -ray_origin.transform.up,                // Backward
        ray_origin.transform.right,              // Right
        -ray_origin.transform.right              // Left
    };

        for (int i = 0; i < directions.Length; i++)
        {
            Ray ray = new Ray(ray_origin.transform.position, directions[i]);
            if (Physics.Raycast(ray, out RaycastHit hit, 440))
            {
                inputs[i] = hit.distance / 440f;

                int objectTypeIndex = i + 4;
                Color debugColor = Color.white;

                switch (hit.transform.tag)
                {
                    case "Food":
                        inputs[objectTypeIndex] = 1f;
                        debugColor = Color.green;
                        break;
                    case "Creature":
                        inputs[objectTypeIndex] = -0.5f;
                        debugColor = Color.gray;
                        break;
                    case "Wall":
                        inputs[objectTypeIndex] = -1f;
                        debugColor = Color.red;
                        break;
                    default:
                        inputs[objectTypeIndex] = 0;
                        break;
                }

                Debug.DrawLine(ray_origin.transform.position, hit.point, debugColor);
            }
        }
    }


    private void UpdateComponentPositions()
    {
        ray_origin.transform.position = transform.position;
    }

    private List<float> UpdateNetwork()
    {
        network.SetInputValues(inputs);
        network.FeedForward();
        return network.GetOutputValues();
    }

    private void CheckForWalls()
    {
        if (transform.position.x > 215 || transform.position.x < -215)
        {
            wall_penalty = 3f;
            Death();
        }

        if (transform.position.z > 215 || transform.position.z < -215)
        {
            wall_penalty = 3f;
            Death();
        }
    }

    private void MoveCreature(int output)
    {
        if (output == 0)
        {
            transform.position += transform.forward * GlobalVariables.speed * Time.deltaTime;
        }
        else if (output == 1)
        {
            transform.position += -transform.forward * GlobalVariables.speed * Time.deltaTime;
        }
        else if (output == 2)
        {
            transform.position += -transform.right * GlobalVariables.speed * Time.deltaTime;
        }
        else if (output == 3)
        {
            transform.position += transform.right * GlobalVariables.speed * Time.deltaTime;
        }
    }

}
