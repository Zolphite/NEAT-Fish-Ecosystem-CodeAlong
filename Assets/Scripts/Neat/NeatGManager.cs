using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeatGManager : MonoBehaviour
{
    public GameObject NeatFishPrefab;
    public GameObject[] allNeatFish;
    public NeatNetwork[] allNeatNetworks;

    public int inputNodes,outputNodes,hiddenNodes;

    [SerializeField] private int currentGeneration = 0;
    
    public int startingPopulation;

    public int keepBest, leaveWorst;

    public int currentAlive;
    private bool repoping = false;

    public bool spawnFromSave = false;
    public int bestTime = 100;
    public int addToBest = 50;

    void Start()
    {
        allNeatFish = new GameObject[startingPopulation];
        allNeatNetworks = new NeatNetwork[startingPopulation];

        if(spawnFromSave == true)
        {
            StartingSavedNetwork();
        }
        else
        {
            StartingNetworks();
        }

        MutatePopulation();
        SpawnBody();
        currentGeneration += 1;
    }

    void FixedUpdate()
    {
        currentAlive = CurrentAlive();
        if (repoping == false && currentAlive <= 0)
        {
            repoping = true;
            // Repopulate for next generation.
            RePopulate();
            repoping = false;
        }
    }

    public int CurrentAlive()
    {
        int alive = 0;
        for (int i = 0; i < allNeatFish.Length; i++)
        {
            if (allNeatFish[i].gameObject)
            {
                alive++;
            }
        }
        return alive;
    }

    private void RePopulate()
    {
        if (spawnFromSave == true)
        {
            bestTime = bestTime + addToBest;
            StartingSavedNetwork();
        }
        else
        {
            SortPopulation();
            SetNewPopulationNetworks();
        }
        MutatePopulation();
        GameObject.FindObjectOfType<FoodManager>().DestroyFood();
        GameObject.FindObjectOfType<FoodManager>().SpawnFood();
        SpawnBody();
        currentGeneration += 1;
    }

    private void MutatePopulation()
    {
        for (int i = keepBest; i < startingPopulation; i++)
        {
            allNeatNetworks[i].MutateNetwork();
        }
    }
    private void SortPopulation()
    {
        for (int i = 0; i < allNeatNetworks.Length; i++)
        {
            for (int j = i; j < allNeatNetworks.Length; j++)
            {
                if(allNeatNetworks[i].fitness < allNeatNetworks[j].fitness)
                {
                    NeatNetwork temp = allNeatNetworks[i];
                    allNeatNetworks[i] = allNeatNetworks[j];
                    allNeatNetworks[j] = temp;
                }
            }
        }
    }

    public void BestFound()
    {
        spawnFromSave = true;

    }

    private void SetNewPopulationNetworks()
    {
        NeatNetwork[] newPopulation = new NeatNetwork[startingPopulation];
        for(int i = 0; i < startingPopulation-leaveWorst; i++)
        {
            newPopulation[i] = allNeatNetworks[i];
        }
        for(int i = startingPopulation-leaveWorst; i < startingPopulation; i++)
        {
            newPopulation[i] = new NeatNetwork(inputNodes, outputNodes, hiddenNodes);
        }
        allNeatNetworks = newPopulation;
    }

    private void StartingNetworks()
    {
        /*
            Creates Initial Group of Networks from StartingPopulation integer.
        */
        for(int i =0; i < startingPopulation; i++)
        {
            allNeatNetworks[i] = new NeatNetwork(inputNodes,outputNodes,hiddenNodes);
        }
    }

    private void StartingSavedNetwork()
    {
        /*
            Creates initial Group of Networks from Saved Network.
        */
        spawnFromSave = false;
        for(int i=0; i < startingPopulation; i++)
        {
            allNeatNetworks[i] = new NeatNetwork(NeatUtilities.LoadGenome());
        }
    }

    private void SpawnBody()
    {
        /* Creates Initial Group of Fish GameObjects from StartingPopulation integer 
        and matches fishObjects to their NetworkBrains. */

        for (int i = 0; i < startingPopulation; i++)
        {
            Vector3 pos = new Vector3(Random.value, Random.value, 20);
            pos = Camera.main.ViewportToWorldPoint(pos);

            allNeatFish[i] = Instantiate(NeatFishPrefab, pos, transform.rotation);
            allNeatFish[i].gameObject.GetComponent<FishController>().myBrainIndex = i;
            allNeatFish[i].gameObject.GetComponent<FishController>().myNetwork = allNeatNetworks[i];
            allNeatFish[i].gameObject.GetComponent<FishController>().inputNodes = inputNodes;
            allNeatFish[i].gameObject.GetComponent<FishController>().outputNodes = outputNodes;
            allNeatFish[i].gameObject.GetComponent<FishController>().hiddenNodes = hiddenNodes;
        }
    }

    public void Death (float fitness, int index)
    {
        allNeatNetworks[index].fitness = fitness;
    }
}
