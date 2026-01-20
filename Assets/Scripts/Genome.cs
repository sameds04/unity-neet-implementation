using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Genome
{
    public int id;
    public List<GenomeNode> nodes;
    public List<GenomeConnection> connections;

    public Genome(int given_id, List<GenomeNode> given_nodes, List<GenomeConnection> given_connections)
    {
        id = given_id;
        nodes = given_nodes;
        connections = given_connections;
    }

    public void MutateGenome()
    {
        int tweak_genome_chance = Random.Range(1, 101);
        int add_connection_chance = Random.Range(1, 101);
        int insert_node_chance = Random.Range(1, 101);

        if (tweak_genome_chance < 20)
        {
            TweakGenome();
        }

        if (add_connection_chance < 20)
        {
            AddConnectionBetweenRandomNodes();
        }

        if (insert_node_chance < 5)
        {
            InsertNodeIntoRandomConnection();
        }
    }

    private void InsertNodeIntoRandomConnection()
    {
        GenomeNode new_node;
        int new_id;

        if (connections.Count == 0)
        {
            new_id = GetNextNodeID();
            new_node = new GenomeNode(new_id, NodeType.Hidden);
            nodes.Add(new_node);
            return;
        }

        GenomeConnection connection = connections[Random.Range(0, connections.Count)];
        connection.active = false;
        GenomeNode input_node = connection.input;
        GenomeNode output_node = connection.output;

        new_id = GetNextNodeID();
        new_node = new GenomeNode(new_id, NodeType.Hidden);
        nodes.Add(new_node);

        float weight = connection.weight;

        GenomeConnection new_input_connection = new GenomeConnection(input_node, new_node, weight, true, GlobalVariables.GetLatestInnovationNumber());
        GenomeConnection new_output_connection = new GenomeConnection(new_node, output_node, weight, true, GlobalVariables.GetLatestInnovationNumber() + 1);

        if (GlobalVariables.innovation_table.Find(x => x.input.ID == input_node.ID && x.output.ID == new_node.ID) != null)
        {
            new_input_connection.innovation = GlobalVariables.innovation_table.Find(x => x.input.ID == input_node.ID && x.output.ID == new_node.ID).innovation;
            connections.Add(new_input_connection);
        }
        else
        {
            connections.Add(new_input_connection);
            GlobalVariables.innovation_table.Add(new_input_connection);
            GlobalVariables.IncrementLatestInnovationNumber();
        }
        
        
        if (GlobalVariables.innovation_table.Find(x => x.input.ID == new_node.ID && x.output.ID == output_node.ID) != null)
        {
            new_output_connection.innovation = GlobalVariables.innovation_table.Find(x => x.input.ID == new_node.ID && x.output.ID == output_node.ID).innovation;
            connections.Add(new_output_connection);
        }
        else
        {
            connections.Add(new_output_connection);
            GlobalVariables.innovation_table.Add(new_output_connection);
            GlobalVariables.IncrementLatestInnovationNumber();
        }
    }

    private int GetNextNodeID()
    {
        int id = 0;

        foreach (GenomeNode node in nodes)
        {
            if (node.ID >= id)
            {
                id = node.ID;
            }
        }

        return id + 1;
    }

    private void AddConnectionBetweenRandomNodes()
    {
        int node_1_id;
        int node_2_id;
        NodeType type_1;
        NodeType type_2;

        while (true)
        {
            node_1_id = Random.Range(0, nodes.Count);
            node_2_id = Random.Range(0, nodes.Count);
            type_1 = nodes[node_1_id].type;
            type_2 = nodes[node_2_id].type;

            if ((type_1 != type_2) || (type_1 == NodeType.Hidden && type_2 == NodeType.Hidden && node_1_id != node_2_id))
            {
                break;
            }

        }

        foreach (GenomeConnection connection in connections)
        {
            if (node_1_id == connection.input.ID && node_2_id == connection.output.ID || node_1_id == connection.output.ID && node_2_id == connection.input.ID) // If a connection with input and outputs nodes matching the random nodes is found
            {
                return;
            }
        }

        if (type_1 == NodeType.Output || type_2 == NodeType.Input) // Switch nodes if node_1 is an output node or node_2 is an input node
        {
            (node_1_id, node_2_id) = (node_2_id, node_1_id);
        }

        GenomeConnection new_connection = new GenomeConnection(nodes[node_1_id], nodes[node_2_id], Random.Range(-1f, 1f), RandomBool(), 0);

        foreach (GenomeConnection connection in GlobalVariables.innovation_table)
        {
            if ((connection.input.ID == node_1_id && connection.output.ID == node_2_id) || (connection.input.ID == node_2_id && connection.output.ID == node_1_id)) // If this connection exists in the innovation table
            {
                new_connection.innovation = connection.innovation;
                connections.Add(new_connection);
                return;
            }
        }

        new_connection.innovation = GlobalVariables.GetLatestInnovationNumber(); // If this connection does not exist in the innovation table
        connections.Add(new_connection);
        GlobalVariables.innovation_table.Add(new_connection);
        GlobalVariables.IncrementLatestInnovationNumber();
    }

    private void TweakGenome()
    {
        int connections_to_be_altered = Random.Range(0, connections.Count);

        for (int i = 0; i < connections_to_be_altered; i++)
        {
            GenomeConnection random_connection = connections[Random.Range(0, connections.Count)];
            random_connection.weight += Random.Range(-0.05f, 0.05f);
            random_connection.active = RandomBool();
        }
    }

    private bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }

    public void SaveGenomeToTextFile(int generation)
    {
        string file_path = "Genome_" + id + "_genome.txt";

        File.AppendAllText(file_path, "Connections at generation: " + generation + "\n");

        foreach (GenomeConnection connection in connections)
        {
            File.AppendAllText(file_path, "Connection " + connection.innovation + ": " + connection.input.type + " Node " + connection.input.ID + " to " + connection.output.type + " node " + connection.output.ID + " with weight: " + connection.weight + " and activation: " + connection.active +"\n");
        }
    }

}

public class GenomeNode
{
    public int ID;
    public NodeType type;

    public GenomeNode(int given_ID, NodeType given_type)
    {
        ID = given_ID;
        type = given_type;
    }
}

public class GenomeConnection
{
    public GenomeNode input;
    public GenomeNode output;
    public float weight;
    public bool active;
    public int innovation;

    public GenomeConnection(GenomeNode given_input, GenomeNode given_output, float given_weight, bool given_active, int given_innovation)
    {
        input = given_input;
        output = given_output;
        weight = given_weight;
        active = given_active;
        innovation = given_innovation;
    }
}
