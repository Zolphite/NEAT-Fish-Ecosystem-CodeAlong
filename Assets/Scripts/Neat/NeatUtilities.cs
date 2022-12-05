using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NeatUtilities : MonoBehaviour
{
    public static void SaveGenome(NeatGenome genome)
    {
        NeatGenomeJson genomeJson = new NeatGenomeJson();
        foreach (NodeGene node in genome.nodeGenes)
        {
            NodeGeneJson nodeJson = new NodeGeneJson();
            nodeJson.id = node.id;
            nodeJson.type = (NodeGeneJson.TYPE)node.type;
            genomeJson.nodeGenes.Add(nodeJson);
        }
        foreach (ConGene con in genome.conGenes)
        {
            ConGeneJson conJson = new ConGeneJson();
            conJson.inputNode = con.inputNode;
            conJson.outputNode = con.outputNode;
            conJson.weight = con.weight;
            conJson.isActive = con.isActive;
            conJson.innovNum = con.innovNum;
            genomeJson.conGenes.Add(conJson);
        }

        string json = JsonUtility.ToJson(genomeJson);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
        print(json);
    }

    public static NeatGenome LoadGenome()
    {
        string genomeString = File.ReadAllText(Application.dataPath + "/save.txt");
        NeatGenomeJson savedGenome = JsonUtility.FromJson<NeatGenomeJson>(genomeString);
        NeatGenome loadedGenome = new NeatGenome();
        foreach(NodeGeneJson savedNode in savedGenome.nodeGenes)
        {
            NodeGene newNode = new NodeGene(savedNode.id, (NodeGene.TYPE)savedNode.type);
            loadedGenome.nodeGenes.Add(newNode);
        }
        foreach(ConGeneJson savedCon in savedGenome.conGenes)
        {
            ConGene newCon = new ConGene(savedCon.inputNode, savedCon.outputNode, savedCon.weight, savedCon.isActive, savedCon.innovNum);
            loadedGenome.conGenes.Add(newCon);
        }

        return loadedGenome;
    }
}


[System.Serializable]
public class NeatGenomeJson
{
    public List<NodeGeneJson> nodeGenes = new List<NodeGeneJson>();
    public List<ConGeneJson> conGenes = new List<ConGeneJson>();
}

[System.Serializable]
public class NodeGeneJson
{
    public int id;
    public enum TYPE
    {
        Input, Output, Hidden
    };
    public TYPE type;
}

[System.Serializable]
public class ConGeneJson
{
    public int inputNode;
    public int outputNode;
    public float weight;
    public bool isActive;
    public int innovNum;
}
