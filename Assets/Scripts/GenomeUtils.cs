using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenomeUtils
{
    public static Genome Crossover(int id, Genome parent1, Genome parent2, float fitness1, float fitness2)
    {
        if (parent1 == parent2 || (parent1.connections.Count == 0 && parent2.connections.Count == 0))
            return new Genome(id, parent1.nodes, parent1.connections);

        if (parent1.connections.Count == 0)
            return new Genome(id, parent2.nodes, parent2.connections);

        if (parent2.connections.Count == 0)
            return new Genome(id, parent1.nodes, parent1.connections);

        var nodeDict = new Dictionary<int, GenomeNode>();
        foreach (var node in parent1.nodes) nodeDict[node.ID] = node;
        foreach (var node in parent2.nodes) nodeDict[node.ID] = node;
        var newNodes = nodeDict.Values.ToList();

        var connectionDict = new Dictionary<int, GenomeConnection>();

        Genome fitter = fitness1 >= fitness2 ? parent1 : parent2;
        Genome weaker = fitness1 >= fitness2 ? parent2 : parent1;

        foreach (var conn in fitter.connections)
        {
            var matching = weaker.connections.FirstOrDefault(c => c.innovation == conn.innovation);
            if (matching != null)
            {
                var selected = Random.Range(0, 2) == 0 ? conn : matching;
                connectionDict[conn.innovation] = new GenomeConnection(selected.input, selected.output, selected.weight, selected.active, selected.innovation);
            }
            else
            {
                connectionDict[conn.innovation] = new GenomeConnection(conn.input, conn.output, conn.weight, conn.active, conn.innovation);
            }
        }

        return new Genome(id, newNodes, connectionDict.Values.ToList());
    }
}
