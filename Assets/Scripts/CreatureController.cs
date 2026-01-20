using UnityEngine;

public class Controller
{
    public static float speed = GlobalVariables.speed;
    public static float wall_boundary = 107.5f;

    public static void Move(GameObject obj, float x, float z)
    {
        Vector3 position = obj.transform.position;
        Vector3 direction = new Vector3(x, 0.0f, z);
        MoveWithinWalls(obj, position, direction);
    }

    private static void MoveWithinWalls(GameObject obj, Vector3 position, Vector3 direction)
    {
        Vector3 new_position = position + (direction * speed);

        if ((new_position.x > wall_boundary || new_position.x < -wall_boundary) && (new_position.z > wall_boundary || new_position.z < -wall_boundary))
        {
            obj.GetComponent<Creature>().health = 0;
            obj.GetComponent<Creature>().health_gained = 0;
            return;
        }

        if ((new_position.x <= wall_boundary && new_position.x >= -wall_boundary) && (new_position.z <= wall_boundary && new_position.z >= -wall_boundary))
        {
            obj.transform.Translate(direction * speed);
            return;
        }

        if (new_position.x > wall_boundary || new_position.x < -wall_boundary)
        {
            obj.transform.Translate(new Vector3(0, 0, direction.z) * speed);
            obj.GetComponent<Creature>().health_gained = 0;
            obj.GetComponent<Creature>().health = 0;
        }

        if (new_position.z > wall_boundary || new_position.z < -wall_boundary)
        {
            obj.transform.Translate(new Vector3(direction.x, 0, 0) * speed);
            obj.GetComponent<Creature>().health = 0;
            obj.GetComponent<Creature>().health_gained = 0;
        }
    }

}
