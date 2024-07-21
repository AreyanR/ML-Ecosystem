using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    // Pellet variables
    public GameObject food;
    [SerializeField] private List<GameObject> spawnedPelletList = new List<GameObject>();
    [SerializeField] private Transform target;

    // Agent variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    // Env variables
    [SerializeField] private Transform environmentLocation;
    Material envMaterial;
    public GameObject env;
    public List<GameObject> shrubs;

    // Hunger timer variables
    [SerializeField] private float agentHungerDuration;
    private float agentHungerTimeLeft;

    // Slider UI component
    public Slider hungerSlider;

    // Enemy Hunter
    public HunterController HunterController;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
        hungerSlider.maxValue = agentHungerDuration;
    }

    public override void OnEpisodeBegin()
    {
        SceneSetup sceneSetup = transform.parent.GetComponentInChildren<SceneSetup>();
        if (sceneSetup != null)
        {
            sceneSetup.ResetEnvironment();
        }

        // Agent
        transform.localPosition = new Vector3(Random.Range(-8f, 8f), 0.31f, Random.Range(-8f, 8f));

        // Pellet
        CreateRandomPellets();

        // Hunger timer for agent
        StartAgentHungerTimer();
    }

    private void Update()
    {
        CheckAgentHungerTime();
    }

    private void CreateRandomPellets()
    {
        int pelletCount = Random.Range(1, 6); // Random number of pellets between 1 and 5
        CreatePellet(pelletCount);
    }

    private void CreatePellet(int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool positionValid = false;
            Vector3 pelletLocation = Vector3.zero;

            while (!positionValid)
            {
                pelletLocation = new Vector3(Random.Range(-8f, 8f), 0.31f, Random.Range(-8f, 8f));
                positionValid = true;

                foreach (GameObject shrub in shrubs)
                {
                    if (CheckOverlap(pelletLocation, shrub.transform.localPosition, 1.0f)) // Check overlap with shrubs
                    {
                        positionValid = false;
                        break;
                    }
                }

                foreach (GameObject pellet in spawnedPelletList)
                {
                    if (CheckOverlap(pelletLocation, pellet.transform.localPosition, 1.0f)) // Check overlap with other pellets
                    {
                        positionValid = false;
                        break;
                    }
                }

                if (CheckOverlap(pelletLocation, transform.localPosition, 1.0f)) // Check overlap with agent
                {
                    positionValid = false;
                }
            }

            GameObject newPellet = Instantiate(food);
            newPellet.transform.parent = environmentLocation;
            newPellet.transform.localPosition = pelletLocation;
            spawnedPelletList.Add(newPellet);
        }
    }

    public bool CheckOverlap(Vector3 overlappingObj, Vector3 existingObj, float minDistance)
    {
        float distancebtw = Vector3.Distance(overlappingObj, existingObj);
        return distancebtw < minDistance; // Overlap condition
    }

    public void RemoveAllPellets()
    {
        foreach (GameObject pellet in spawnedPelletList)
        {
            Destroy(pellet);
        }
        spawnedPelletList.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            spawnedPelletList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(15f);
            agentHungerTimeLeft += 10f;
            hungerSlider.value = agentHungerTimeLeft;

            // Check if all pellets are collected
            if (spawnedPelletList.Count == 0)
            {
                Debug.Log("Prey got all the food");
                AddReward(5f);
                CreateRandomPellets(); // Spawn new set of random pellets
            }
        }

        if (other.gameObject.tag == "Wall")
        {
            Debug.Log("Prey hit wall");
            AddReward(-15f);
            RemoveAllPellets(); // Remove all pellets when hitting a wall
            HunterController.EndEpisode();
            EndEpisode(); // End agent's episode only
        }
    }

    private void StartAgentHungerTimer()
    {
        agentHungerTimeLeft = agentHungerDuration;
        hungerSlider.value = agentHungerTimeLeft;
    }

    private void CheckAgentHungerTime()
    {
        agentHungerTimeLeft -= Time.deltaTime;
        hungerSlider.value = agentHungerTimeLeft;

        if (agentHungerTimeLeft <= 0)
        {
            Debug.Log("Prey starved");
            AddReward(-15f);
            RemoveAllPellets(); // Remove all pellets when starving
            HunterController.EndEpisode();
            EndEpisode(); // End agent's episode only
        }
    }
}
