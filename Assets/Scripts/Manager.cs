using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject creature_prefab;
    [SerializeField] GameObject ray_origin_prefab;
    [SerializeField] GameObject current_generation_text_prefab;
    [SerializeField] GameObject live_creature_count_text_prefab;
    [SerializeField] GameObject average_fitness_text_prefab;
    [SerializeField] GameObject highest_average_fitness_text_prefab;

    private GameObject creature;
    public static RawImage input_view_image;

    public List<Network> all_networks;
    public List<GameObject> all_creatures;
    public int starting_population_count = 30;
    public int current_generation;
    public int live_creature_count;
    public int latest_genome;
    public float average_fitness = 0;
    public float highest_average_fitness = 0;

    Text current_generation_text;
    Text live_creature_count_text;
    Text average_fitness_text;

    private void Start()
    {
        current_generation = 0;
        InvokeRepeating("SpawnFood", 0, 1);
        InstantiateLists();
        CreateStartingNetworks();
        CreateStartingCreatures();
        GetComponent<SpawnFood>().SpawnFoodGamobjects();


        current_generation_text = current_generation_text_prefab.GetComponent<Text>();
        live_creature_count_text = live_creature_count_text_prefab.GetComponent<Text>();
        average_fitness_text = average_fitness_text_prefab.GetComponent<Text>();
    }

    private void SpawnFood()
    {
        GetComponent<SpawnFood>().SpawnFoodOnce();
    }

    private void UpdateText()
    {
        current_generation_text.text = "Current generation: " + current_generation;
        live_creature_count_text.text = "Live Creature Count: " + live_creature_count;
        average_fitness_text.text = "Average Fitness: " + (int) average_fitness;
    }

    private void InstantiateLists()
    {
        all_networks = new List<Network>();
        all_creatures = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        live_creature_count = CountLiveCreatures();

        if (live_creature_count == 0)
        {
            Repopulate();
        }

        UpdateText();
    }

    private void CreateStartingNetworks()
    {
        for (int i = 0; i < starting_population_count; i++)
        {
            Network network = new Network();
            network.CreateNetwork(i, GlobalVariables.inputs, GlobalVariables.hiddens, GlobalVariables.outputs);
            all_networks.Add(network);
        }

        latest_genome = starting_population_count;
    }

    private int CountLiveCreatures()
    {
        return all_creatures.Count;
    }

    private void MutatePopulation()
    {
        int best_count = 3;

        for (int i = best_count; i < starting_population_count; i++)
        {
            all_networks[i].MutateNetwork();
        }
    }

    private void Repopulate()
    {
        current_generation++;
        SortAllNetworksByFitness();
        average_fitness = GetAverageFitness();
        all_networks = RepopulateNetworks();
        MutatePopulation();
        CreateStartingCreatures();
        GetComponent<SpawnFood>().DestroyFoodGameObjects();
        GetComponent<SpawnFood>().SpawnFoodGamobjects();
        GlobalVariables.SaveTableToFile();
    }

    public float GetAverageFitness()
    {
        float fitness = 0;

        foreach (Network network in all_networks)
        {
            fitness += network.fitness;
        }

        fitness /= all_networks.Count;

        if (fitness > highest_average_fitness)
        {
            highest_average_fitness = fitness;
        }

        return fitness;
    }

    public void Death(GameObject creature)
    {
        all_creatures.Remove(creature);
    }

    private void SortAllNetworksByFitness()
    {
        for (int i = 0; i < all_networks.Count; i++)
        {
            for (int j = 0; j < all_networks.Count; j++)
            {
                if (all_networks[i].fitness > all_networks[j].fitness)
                {
                    Network temp = all_networks[i];
                    all_networks[i] = all_networks[j];
                    all_networks[j] = temp;
                }
            }
        }
    }

    private List<Network> RepopulateNetworks()
    {
        int best_count = GlobalVariables.best_percentage;
        int worst_count = GlobalVariables.worst_percentage;
        List<Network> new_all_networks = new List<Network>();

        for (int i = 0; i < starting_population_count - worst_count; i++)
        {
            new_all_networks.Add(all_networks[i]);
        }
        
        for (int i = starting_population_count - worst_count; i < starting_population_count; i++)
        {
            Network network = new Network();
            Network parent_1 = all_networks[Random.Range(0, best_count)];
            Network parent_2 = all_networks[Random.Range(0, best_count)];
            network.genome = GenomeUtils.Crossover(latest_genome, parent_1.genome, parent_2.genome, parent_1.fitness, parent_2.fitness);

            int input_node_count = 0;
            foreach (GenomeNode node in network.genome.nodes)
            {
                if (node.type == NodeType.Input)
                {
                    input_node_count++;
                }
            }

            network.CreateNodes(network.genome);
            network.CreateConnections(network.genome);
            new_all_networks.Add(network);
            latest_genome++;
        }

        return new_all_networks;
    }

    private void CreateStartingCreatures()
    {
        for (int i = 0; i < starting_population_count; i++)
        {
            creature = Instantiate(creature_prefab, new Vector3(Random.Range(-100, 100), 0.5f, Random.Range(-100, 100)), Quaternion.identity);
            creature.name = "" + i;
            creature.GetComponent<Creature>().network = all_networks[i];
            creature.GetComponent<Creature>().generation = current_generation;
            creature.GetComponent<Creature>().ray_origin = Instantiate(ray_origin_prefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(90, 0, 0));
            all_creatures.Add(creature);
        }
    }

    public void DebugStatements()
    {
        foreach (GameObject creature in all_creatures)
        {
            Debug.Log("Creature " + creature.name + " has a network with genome id: " + creature.GetComponent<Creature>().network.genome.id);
        }
    }

    private void SaveGenomes()
    {
        foreach (Network network in all_networks)
        {
            network.genome.SaveGenomeToTextFile(current_generation);
        }
    }
}
