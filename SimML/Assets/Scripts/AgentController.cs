using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{

    //Pellet variables
    public int pelletCount;
    public GameObject food;
    [SerializeField] private List<GameObject> spawnedPelletList = new List<GameObject>();
    [SerializeField] private Transform target;


//Agent variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;


    // env variables
    [SerializeField] private Transform environmentLocation;
    Material envMaterial;
    public GameObject env;

    //Time keeping variables
    [SerializeField] private float timeForEpisode;
    private float timeLeft;

    //Enemy Hunter
    public HunterController HunterController;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
    }

    public override void OnEpisodeBegin()
    {
        //Agent
        transform.localPosition = new Vector3(Random.Range(-4f,4f),0.31f,Random.Range(-4f,4f));

        //pellet
        CreatePellet();

        //Timer for agent if taking too long and be punished
        EpisodeTimerNew();
        
    }


    private void Update()
    {
        CheckRemainingTime();
    }

    private void CreatePellet()
    {
        if(spawnedPelletList.Count != 0)
        {
            RemovePellet(spawnedPelletList);
        }

        for (int i = 0; i < pelletCount; i++)
        {

            int counter = 0;
            bool isOverlapping;
            bool beedDecramented = false;

            GameObject newPellet = Instantiate(food);
            //make pellet child of env
            newPellet.transform.parent = environmentLocation;
            //Give random spawn location
            Vector3 pelletLocation = new Vector3(Random.Range(-4f,4f),0.31f,Random.Range(-4f,4f));

            if(spawnedPelletList.Count != 0)
            {
                for(int j = 0; j < spawnedPelletList.Count; j++)
                {
                    if(counter <= 10)
                    {
                        isOverlapping = CheckOverlap(pelletLocation, spawnedPelletList[j].transform.localPosition, 5f);
                        if (isOverlapping == false)
                        {
                            pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.31f, Random.Range(-4f, 4f));
                            j--;
                            beedDecramented = true;
                        }

                        isOverlapping = CheckOverlap(pelletLocation, transform.localPosition, 5f);
                        if (isOverlapping == false)
                        {
                            pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.31f, Random.Range(-4f, 4f));
                            if(beedDecramented == false)
                            {
                                j--;
                            
                            }
                        }
                        counter++;
                    }
                    else
                    {
                        j = spawnedPelletList.Count;
                    }
                }
                
            }

            //Spawn in new location
            newPellet.transform.localPosition = pelletLocation;
            //Add to list
            spawnedPelletList.Add(newPellet);
        }
    }

    public bool CheckOverlap(Vector3 overlappingObj, Vector3 existingObj, float minDistance)
    {
        float distancebtw = Vector3.Distance(overlappingObj, existingObj);
        if (minDistance <= distancebtw)
        {
            return true;
        }
        return false;
    }

    private void RemovePellet(List<GameObject> spawnedPelletList)
    {
        foreach (GameObject i in spawnedPelletList)
        {
            Destroy(i.gameObject);
        }
        spawnedPelletList.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);
    }
    
        
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];
        
        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);

        /*
        Vector3 velocity = new Vector3(moveX,0f,moveZ) * Time.deltaTime * moveSpeed;
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;
        transform.localPosition += velocity;

        */
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Pellet")
        {
            //Remove from list
            spawnedPelletList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(10f);
            if(spawnedPelletList.Count == 0)
            {
                envMaterial.color = Color.green;
                RemovePellet(spawnedPelletList);
                AddReward(5f);
                HunterController.AddReward(-5f);
                HunterController.EndEpisode();
                EndEpisode();
            }
        }

        if(other.gameObject.tag == "Wall")
        {
            envMaterial.color = Color.red;
            RemovePellet(spawnedPelletList);
            AddReward(-15f);
            HunterController.EndEpisode();
            EndEpisode();
        }
    }

    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEpisode;

    }

    private void CheckRemainingTime()
    {
        if(Time.time >= timeLeft)
        {
            envMaterial.color = Color.blue;
            AddReward(-15f);
            HunterController.AddReward(-15f);
            RemovePellet(spawnedPelletList);
            HunterController.EndEpisode();
            EndEpisode();
        }
    }
}

