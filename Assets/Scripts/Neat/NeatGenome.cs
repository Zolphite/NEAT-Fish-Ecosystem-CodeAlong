using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeatGenome
{
    public List<NodeGene> nodeGenes;
    public List<ConGene> conGenes;

    public NeatGenome()
    {
        nodeGenes = new List<NodeGene>();
        conGenes = new List<ConGene>();
    }

    public NeatGenome(List<NodeGene> nodeGens, List<ConGene> conGens)
    {
        nodeGenes = nodeGens;
        conGenes = conGens;
    }

    public void MutateGenome()
    {
        float createEdgeChance = 90f;
        float createNodeChance = 10f;
        float chanceEdge = Random.Range(0f, 100f);
        float chanceNode = Random.Range(0f, 100f);

        if (chanceNode <= createNodeChance)
        {
            // Create Random New Node
            AddRandomNode();
        }
        if (chanceEdge <= createEdgeChance)
        {
            // Create Random New Edge
            AddRandomConnection();
        }
        // Mutate The Weights
        MutateWeights();
    }

    private void AddRandomNode()
    {
        if (conGenes.Count != 0)
        {
            int randomCon = Random.Range(0, conGenes.Count);
            ConGene mutatingCon = conGenes[randomCon];
            int firstNode = mutatingCon.inputNode;
            int secondNode = mutatingCon.outputNode;

            // Disable the mutating connection
            mutatingCon.isActive = false;

            int newId = GetNextNodeId();
            
            NodeGene newNode = new NodeGene(newId, NodeGene.TYPE.Hidden);
            nodeGenes.Add(newNode);

            int nextInovNum = GetNextInovNum();
            ConGene firstNewCon = new ConGene(firstNode, newNode.id, 1f, true, nextInovNum);
            conGenes.Add(firstNewCon);
            nextInovNum = GetNextInovNum();
            ConGene secondNewCon = new ConGene(newNode.id, secondNode, mutatingCon.weight, true, nextInovNum);
            conGenes.Add(secondNewCon);
        }
    }

    private int GetNextNodeId()
    {
        int nextID = 0;
        foreach(NodeGene node in nodeGenes)
        {
            if (nextID <= node.id)
            {
                nextID = node.id;
            }
        }
        nextID = nextID + 1;
        return nextID;
    }
    private bool AddRandomConnection()
    {
        int firstNode = Random.Range(0, nodeGenes.Count);
        int secondNode = Random.Range(0, nodeGenes.Count);
        NodeGene.TYPE firstType = nodeGenes[firstNode].type;
        NodeGene.TYPE secondType = nodeGenes[secondNode].type;


        if (firstType == secondType && firstType != NodeGene.TYPE.Hidden)
        {
            return AddRandomConnection();
        }

        foreach(ConGene con in conGenes)
        {
            if((firstNode == con.inputNode && secondNode == con.outputNode) ||
                (secondNode == con.inputNode && firstNode == con.outputNode))
            {
                return false;
            }
        }

        if (firstType == NodeGene.TYPE.Output || (firstType == NodeGene.TYPE.Hidden
            && secondType == NodeGene.TYPE.Input))
        {
            int tmp = firstNode;
            firstNode = secondNode;
            secondNode = tmp;

            firstType = nodeGenes[firstNode].type;
            secondType = nodeGenes[secondNode].type;
        }

        int innov = GetNextInovNum();
        float weight = Random.Range(-1f, 1f);
        bool act = true;
        ConGene newCon = new ConGene(firstNode, secondNode, weight, act, innov); 
        conGenes.Add(newCon);
        return true;
    }

    private int GetNextInovNum()
    {
        int nextInov = 0;
        foreach(ConGene con in conGenes)
        {
            if(nextInov <= con.innovNum)
            {
                nextInov = con.innovNum;
            }
        }
        nextInov += 1;
        return nextInov;
    }
    private void MutateWeights()
    {
        float randomWeightChance = 5f;
        float perturbWeightChance = 95f;
        float chanceRandom = Random.Range(0f, 100f);
        float chancePerturb = Random.Range(0f, 100f);

        if (chanceRandom <= randomWeightChance)
        {
            // Randomize Single Weight
            RandomizeSingleWeight();
        }
        if (chancePerturb <= perturbWeightChance)
        {
            // Perturb Group of Weight
            PerturbWeights();
        }
    }

    private void RandomizeSingleWeight()
    {
        if (conGenes.Count != 0)
        {
            int randomConIndex = Random.Range(0, conGenes.Count);
            ConGene connection = conGenes[randomConIndex];
            connection.weight = Random.Range(-1f, 1f);
        }
    }

    private void PerturbWeights()
    {
        foreach (ConGene con in conGenes)
        {
            con.weight = con.weight + (Random.Range(-0.5f, 0.5f) * 0.5f);
        }
    }

}

public class NodeGene
{
    public int id;
    public enum TYPE
    {
        Input, Output, Hidden
    };
    public TYPE type;

    public NodeGene(int givenID, TYPE givenType)
    {
        id = givenID;
        type = givenType;
    }
}

public class ConGene
{
    public int inputNode;
    public int outputNode;
    public float weight;
    public bool isActive;
    public int innovNum;

    public ConGene(int inNode, int outNode, float wei, bool active, int inov)
    {
        inputNode = inNode;
        outputNode = outNode;
        weight = wei;
        isActive = active;
        innovNum = inov;
    }
}
