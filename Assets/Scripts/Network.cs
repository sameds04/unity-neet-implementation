using System.Collections.Generic;
using UnityEngine;

public class Network
{
    public Genome genome;
    public List<Node> nodes;
    public float fitness;

    public Network()
    {
        nodes = new List<Node>();
    }

    public Genome CreateGenome(int id, int inputs, int hiddens, int outputs)
    {
        return new Genome(id, CreateGenomeNodes(inputs, hiddens, outputs), new List<GenomeConnection>());
    }

    private List<GenomeNode> CreateGenomeNodes(int inputs, int hiddens, int outputs)
    {
        List<GenomeNode> nodes = new List<GenomeNode>();
        int index = 0;

        for (int i = 0; i < inputs; i++)
        {
            nodes.Add(new GenomeNode(index, NodeType.Input));
            index++;
        }

        for (int i = 0; i < outputs; i++)
        {
            nodes.Add(new GenomeNode(index, NodeType.Output));
            index++;
        }

        for (int i = 0; i < hiddens; i++)
        {
            nodes.Add(new GenomeNode(index, NodeType.Hidden));
            index++;
        }

        return nodes;
    }

    public void MutateNetwork()
    {
        genome.MutateGenome();
        ResetNetwork();
        CreateNodes(genome);
        CreateConnections(genome);
    }

    private void ResetNetwork()
    {
        fitness = 0;
        nodes.Clear();
    }

    public void CreateNetwork(int id, int inputs, int hiddens, int outputs) // Create a network from a given genome
    {
        genome = CreateGenome(id, inputs, hiddens, outputs);
        CreateNodes(genome);
        CreateConnections(genome);
    }

    public void CreateNodes(Genome genome)
    {
        foreach (GenomeNode genome_node in genome.nodes) // For each genome node in this genome
        {
            Node node = new Node(genome_node.ID, 0, genome_node.type, new List<Connection>(), new List<Connection>()); // Create a new node
            nodes.Add(node);
        }
    }

    public void CreateConnections(Genome genome)
    {
        foreach (GenomeConnection genome_connection in genome.connections)
        {
            Node input = nodes.Find(x => x.ID == genome_connection.input.ID);
            Node output = nodes.Find(x => x.ID == genome_connection.output.ID);
            Connection connection = new Connection(input, output, genome_connection.weight, genome_connection.active);

            if (nodes.Find(x => x.ID == genome_connection.input.ID) == null)
            {
                Debug.Log("Failed to find node: " + genome_connection.input.ID + " for Creature " + genome.id);
            }

            if (nodes.Find(x => x.ID == genome_connection.output.ID) == null)
            {
                Debug.Log("Failed to find node: " + genome_connection.output.ID + " for Creature " + genome.id);
            }

            nodes.Find(x => x.ID == genome_connection.input.ID).outputs.Add(connection);
            nodes.Find(x => x.ID == genome_connection.output.ID).inputs.Add(connection);
        }
    }

    public void SetInputValues(float[] inputs)
    {
        int count = 0;
        foreach (Node node in nodes)
        {
            if (node.type == NodeType.Input)
            {
                node.value = Node.Tanh(inputs[count]);
                count++;
            }
        }
    }

    public void FeedForward()
    {
        foreach (Node node in nodes.FindAll(x => x.type == NodeType.Hidden))
        {
            node.FeedNodeForward();
        }
        foreach (Node node in nodes.FindAll(x => x.type == NodeType.Output))
        {
            node.FeedNodeForward();
        }
    }

    public void ResetNodes()
    {
        foreach (Node node in nodes)
        {
            node.ResetNode();
        }
    }

    public List<float> GetOutputValues()
    {
        List<float> output_values = new List<float>();

        foreach (Node node in nodes.FindAll(x => x.type == NodeType.Output)) // For each output node
        {
            output_values.Add(node.value); // Add the value of the node to the list
        }

        return output_values; // Return list
    }

    public void SetFitness(float health_gained)
    {
        fitness = (health_gained);
    }
}

public class Node
{
    public int ID;
    public float value;
    public NodeType type;
    public List<Connection> inputs;
    public List<Connection> outputs;

    public Node(int given_ID, float given_value, NodeType given_type, List<Connection> given_inputs, List<Connection> given_outputs)
    {
        ID = given_ID;
        inputs = given_inputs;
        type = given_type;
        outputs = given_outputs;
        value = given_value;
    }
    public void FeedNodeForward()
    {
        foreach (Connection connection in inputs)
        {
            if (connection.active)
            {
                value += connection.input_node.value * connection.weight;
            }
        }

        value = Tanh(value);
    }

    public void ResetNode()
    {
        value = 0;
    }

    public static float Tanh(float x)
    {
        return ((2 / (1 + Mathf.Exp(-2f * x))))-1;
    }

}

public class Connection
{
    public Node input_node;
    public Node output_node;
    public float weight;
    public bool active;

    public Connection(Node given_input_node, Node given_output_node, float given_weight, bool given_active)
    {
        input_node = given_input_node;
        output_node = given_output_node;
        weight = given_weight;
        active = given_active;
    }
}
