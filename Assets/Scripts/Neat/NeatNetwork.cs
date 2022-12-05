using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeatNetwork
{
    public NeatGenome myGenome;
    public List<Node> nodes;
    public List<Node> inputNodes;
    public List<Node> outputNodes;
    public List<Node> hiddenNodes;
    public List<Connection> connections;
    public float fitness;
    
    public NeatNetwork(int inp, int outp, int hid)
    {
        myGenome = CreateInitialGenome(inp, outp, hid);
        nodes = new List<Node>();
        inputNodes = new List<Node>();
        outputNodes = new List<Node>();
        hiddenNodes = new List<Node>();
        connections = new List<Connection>();
        CreateNetwork();
    }

    public NeatNetwork(NeatGenome genome)
    {
        myGenome = genome;
        nodes = new List<Node>();
        inputNodes = new List<Node>();
        outputNodes = new List<Node>();
        hiddenNodes = new List<Node>();
        connections = new List<Connection>();
        CreateNetwork();
    }

    private NeatGenome CreateInitialGenome(int inp, int outp, int hid)
    {
        List<NodeGene> newNodeGenes = new List<NodeGene>();
        List<ConGene> newConGenes = new List<ConGene>();
        int nodeId = 0;

        for (int i = 0; i < inp; i++)
        {
            NodeGene newNodeGene = new NodeGene(nodeId, NodeGene.TYPE.Input);
            newNodeGenes.Add(newNodeGene);
            nodeId += 1;
        }

        for (int i = 0; i < outp; i++)
        {
            NodeGene newNodeGene = new NodeGene(nodeId, NodeGene.TYPE.Output);
            newNodeGenes.Add(newNodeGene);
            nodeId += 1;
        }

        for (int i = 0; i < hid; i++)
        {
            NodeGene newNodeGene = new NodeGene(nodeId, NodeGene.TYPE.Hidden);
            newNodeGenes.Add(newNodeGene);
            nodeId += 1;
        }

        NeatGenome newGenome = new NeatGenome(newNodeGenes, newConGenes);
        return newGenome;
    }

    private void CreateNetwork()
    {
        ResetNetwork();
        // Creation of Network Structure: Nodes
        foreach(NodeGene nodeGene in myGenome.nodeGenes)
        {
            Node newNode = new Node(nodeGene.id);
            nodes.Add(newNode);
            if(nodeGene.type == NodeGene.TYPE.Input)
            {
                inputNodes.Add(newNode);
            }
            else if(nodeGene.type == NodeGene.TYPE.Hidden)
            {
                hiddenNodes.Add(newNode);
            }
            else if(nodeGene.type == NodeGene.TYPE.Output)
            {
                outputNodes.Add(newNode);
            }
        }

        // Creation of Network Structure: Edges
        foreach(ConGene conGene in myGenome.conGenes)
        {
            if (conGene.isActive == true)
            {
                Connection newCon = new Connection(conGene.inputNode, conGene.outputNode, conGene.weight, conGene.isActive);
                connections.Add(newCon);
            }
        }

        // Creation of Network Structure: Node Neighbors
        foreach(Node node in nodes)
        {
            foreach(Connection con in connections)
            {
                if(con.inputNode == node.id)
                {
                    node.outputConnections.Add(con);
                }
                else if(con.outputNode == node.id)
                {
                    node.inputConnections.Add(con);
                }
            }
        }
    }

    private void ResetNetwork()
    {
        nodes.Clear();
        inputNodes.Clear();
        outputNodes.Clear();
        hiddenNodes.Clear();
        connections.Clear();
    }
    public void MutateNetwork()
    {
        myGenome.MutateGenome();
        CreateNetwork();
    }
    // Main Driver Function for the NeuralNet
    public float[] FeedForwardNetwork(float[] inputs)
    {
        float[] outputs = new float[outputNodes.Count];
        for(int i = 0; i < inputNodes.Count; i++)
        {
            inputNodes[i].SetInputNodeValue(inputs[i]);
            inputNodes[i].FeedForwardValue();
            inputNodes[i].value = 0;
        }
        for (int i = 0; i < hiddenNodes.Count; i++)
        {
            hiddenNodes[i].SetHiddenNodeValue();
            hiddenNodes[i].FeedForwardValue();
            hiddenNodes[i].value = 0;
        }
        for(int i = 0; i < outputNodes.Count; i++)
        {
            outputNodes[i].SetOutputNodeValue();
            outputs[i] = outputNodes[i].value;
            outputNodes[i].value = 0;
        }

        return outputs;
    }
}

public class Node
{
    public int id;
    public float value;
    public List<Connection> inputConnections;
    public List<Connection> outputConnections;

    public Node(int ident)
    {
        id = ident;
        inputConnections = new List<Connection>();
        outputConnections = new List<Connection>();
    }

    public void SetInputNodeValue(float val)
    {
        val = Sigmoid(val);
        value = val;
    }
    public void SetHiddenNodeValue()
    {
        float val = 0;
        foreach (Connection con in inputConnections)
        {
            val +=  (con.weight*con.inputNodeValue);
        }
        value = TanH(val);
    }
    public void SetOutputNodeValue()
    {
        float val = 0;
        foreach (Connection con in inputConnections)
        {
            val +=  (con.weight*con.inputNodeValue);
        }
        value = TanH(val);
    }

    public void FeedForwardValue()
    {
        foreach(Connection con in outputConnections)
        {
            con.inputNodeValue = value;
        }
    }

    // Activation Functons
    private float Sigmoid(float x)
    {
        return (1 / (1 + Mathf.Exp(-x)));
    }

    private float TanH(float x)
    {
        return ((2 / (1 + Mathf.Exp(-2*x))) - 1);
    }

    private float TanHMod1(float x)
    {
        return ((2 / (1 + Mathf.Exp(-4*x))) - 1);
    }
}

public class Connection
{
    public int inputNode;
    public int outputNode;
    public float weight;
    public bool isActive;
    public float inputNodeValue;
    public Connection(int inNode, int outNode, float wei, bool active)
    {
        inputNode = inNode;
        outputNode = outNode;
        weight = wei;
        isActive = active;
    }
}

