using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static int inputs = 8;
    public static int hiddens = 0;
    public static int outputs = 4;
    public static int best_percentage = 4;
    public static int worst_percentage = 23;
    public static float speed = 30f;
    public static float starting_health = 25;

    public static List<GenomeConnection> innovation_table { get; private set; }
    private static int latest_innovation_number;

    private void Start()
    {
        innovation_table = new List<GenomeConnection>();
        latest_innovation_number = 0;
    }

    public static void SaveTableToFile()
    {
        string file_path = "innovation_table.txt";

        foreach (GenomeConnection connection in innovation_table)
        {
            File.AppendAllText(file_path, "Connection " + connection.innovation + ": " + connection.input.type + " Node " + connection.input.ID + " to " + connection.output.type + " node " + connection.output.ID + "\n");
        }
    }

    public static void IncrementLatestInnovationNumber()
    {
        latest_innovation_number++;
    }

    public static int GetLatestInnovationNumber()
    {
        return latest_innovation_number;
    }

}
