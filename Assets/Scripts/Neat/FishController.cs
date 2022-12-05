using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public NeatNetwork myNetwork;

    private float[] sensors;

    private float hitDivider = 20f;
    private float rayDistance = 40f;

    [Header("Energy Options")]
    public float totalEnergy;
    public float rewardEnergy;
    public float currentEnergy;

    [Header("FitnessOptions")]
    public float overallFitness = 0;
    public float foodMultiplier;
    public float foodSinceStart = 0f;
    
    [Header("Network Settings")]
    public int myBrainIndex;

    public int inputNodes,outputNodes,hiddenNodes;
 
    public float surviveTime = 0;
    public int bestTime = 0;


    // [Range(-1f,1f)]
    // public float a,t;

    void Start()
    {
        currentEnergy = totalEnergy;
        sensors = new float[inputNodes];
    }
    void Awake()
    {
        bestTime = GameObject.FindObjectOfType<NeatGManager>().bestTime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        InputSensors();
        float[] outputs = myNetwork.FeedForwardNetwork(sensors);
        MoveFish(outputs[0], outputs[1]);
        CalculateFitness();

        surviveTime += Time.deltaTime;
    }

    private void CalculateFitness()
    {
        UpdateEnergy();
        overallFitness = (foodSinceStart*foodMultiplier);

        if (currentEnergy <= 0)
        {
            Death();
        }

        if(surviveTime >= bestTime)
        {
            print("Save Fish");
            GameObject.FindObjectOfType<NeatGManager>().BestFound();
            NeatUtilities.SaveGenome(myNetwork.myGenome);
            Death();
        }
    }
    private void UpdateEnergy()
    {
        currentEnergy -= Time.deltaTime;
    }
    private void Death()
    {
        GameObject.FindObjectOfType<NeatGManager>().Death(overallFitness, myBrainIndex);
        Destroy(gameObject);
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.transform.tag == "Wall")
        {
            // Debug.Log("Fish Died");
            // overallFitness = 0;
            Death();
        }
        else if (collision.transform.tag == "Food")
        {
            // Debug.Log("Fish Ate Food");
            collision.gameObject.GetComponent<FoodController>().SpawnSignleFood();
            Destroy(collision.gameObject);
            currentEnergy += rewardEnergy;
            foodSinceStart += 1;
        }
    }

    private void InputSensors()
    {
        Ray r = new Ray(transform.position, transform.up);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, rayDistance))
        {
            if (hit.transform.tag == "Wall")
            {
                sensors[0] = hit.distance / hitDivider;
                Debug.DrawLine(r.origin, hit.point, Color.white);
            }
        }
        r.direction = (transform.up+transform.right);
        if (Physics.Raycast(r, out hit, rayDistance))
        {
            if (hit.transform.tag == "Wall")
            {
                sensors[1] = hit.distance / hitDivider;
                Debug.DrawLine(r.origin, hit.point, Color.white);
            }
        }
        r.direction = (transform.up-transform.right);
        if (Physics.Raycast(r, out hit, rayDistance))
        {
            if (hit.transform.tag == "Wall")
            {
                sensors[2] = hit.distance / hitDivider;
                Debug.DrawLine(r.origin, hit.point, Color.white);
            }
        }

        r.direction = (transform.up);
        if (Physics.Raycast(r, out hit, rayDistance))
        {
            if (hit.transform.tag == "Food")
            {
                sensors[3] = hit.distance / hitDivider;
                Debug.DrawLine(r.origin, hit.point, Color.yellow);
            }
        }
        r.direction = (transform.up+transform.right);
        if (Physics.Raycast(r, out hit, rayDistance))
        {
            if (hit.transform.tag == "Food")
            {
                sensors[4] = hit.distance / hitDivider;
                Debug.DrawLine(r.origin, hit.point, Color.yellow);
            }
        }
        r.direction = (transform.up-transform.right);
        if (Physics.Raycast(r, out hit, rayDistance))
        {
            if (hit.transform.tag == "Food")
            {
                sensors[5] = hit.distance / hitDivider;
                Debug.DrawLine(r.origin, hit.point, Color.yellow);
            }
        }
    }
    public void MoveFish(float v, float h)
    {
        // Getting Next Position
        Vector3 input = Vector3.Lerp(Vector3.zero, new Vector3(0, v*2f, 0), 0.1f);
        input = transform.TransformDirection(input);

        // Movement of Agent
        transform.position += input;

        // Rotation of Agent
        transform.eulerAngles += new Vector3(0, 0, (h*90)*0.1f);
    }
}
