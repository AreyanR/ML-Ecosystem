using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    public GameObject food;
    [SerializeField] private List<GameObject> spawnedPelletList = new List<GameObject>();
    [SerializeField] private Transform target;
    [SerializeField] private int pelletCount = 5;

    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    [SerializeField] private Transform environmentLocation;
    Material envMaterial;
    public GameObject env;
    public List<GameObject> shrubs;

    [SerializeField] private float agentHungerDuration;
    private float agentHungerTimeLeft;

    public Slider hungerSlider;

    public HunterController HunterController;

    // Initializes the agent and environment settings.
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
        hungerSlider.maxValue = agentHungerDuration;
    }

    // Called at the start of each episode to reset the environment and agent.
    public override void OnEpisodeBegin()
    {
        SceneSetup sceneSetup = transform.parent.GetComponentInChildren<SceneSetup>();
        if (sceneSetup != null)
        {
            sceneSetup.ResetEnvironment();
        }

        RespawnAgent();
        RemoveAllPellets();
        CreatePellets();
        StartAgentHungerTimer();
    }

    // Updates the agent's hunger timer each frame.
    private void Update()
    {
        CheckAgentHungerTime();
    }

    // Creates the specified number of pellets in the environment.
    private void CreatePellets()
    {
        CreatePellet(pelletCount);
    }

    // Creates pellets at random valid locations within the environment.
    // count: Number of pellets to create.
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
                    if (CheckOverlap(pelletLocation, shrub.transform.localPosition, 1.0f))
                    {
                        positionValid = false;
                        break;
                    }
                }

                foreach (GameObject pellet in spawnedPelletList)
                {
                    if (CheckOverlap(pelletLocation, pellet.transform.localPosition, 1.0f))
                    {
                        positionValid = false;
                        break;
                    }
                }

                if (CheckOverlap(pelletLocation, transform.localPosition, 1.0f))
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

    // Checks if two objects overlap within a certain distance.
    // overlappingObj: Position of the overlapping object.
    // existingObj: Position of the existing object.
    // minDistance: Minimum distance required to avoid overlap.
    public bool CheckOverlap(Vector3 overlappingObj, Vector3 existingObj, float minDistance)
    {
        float distancebtw = Vector3.Distance(overlappingObj, existingObj);
        return distancebtw < minDistance;
    }

    // Removes all pellets from the environment.
    public void RemoveAllPellets()
    {
        foreach (GameObject pellet in spawnedPelletList)
        {
            Destroy(pellet);
        }
        spawnedPelletList.Clear();
    }

    // Collects observations from the environment to feed to the agent's neural network.
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    // Processes actions received from the agent's neural network.
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    // Provides manual input for testing the agent's behavior.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // Handles interactions with trigger colliders.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            spawnedPelletList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(20f);
            agentHungerTimeLeft += 20f;
            agentHungerTimeLeft = Mathf.Clamp(agentHungerTimeLeft, 0, agentHungerDuration);
            hungerSlider.value = agentHungerTimeLeft;

            if (spawnedPelletList.Count == 0)
            {
                Debug.Log("Prey got all the food");
                AddReward(5f);
                HunterController.EndEpisode();
                EndEpisode();
            }
        }

        if (other.gameObject.tag == "Wall")
        {
            Debug.Log("Prey hit wall");
            AddReward(-15f);
            RemoveAllPellets();
            HunterController.EndEpisode();
            EndEpisode();
        }
    }

    // Starts the hunger timer for the agent.
    private void StartAgentHungerTimer()
    {
        agentHungerTimeLeft = agentHungerDuration;
        hungerSlider.value = agentHungerTimeLeft;
    }

    // Checks the agent's remaining hunger time and ends the episode if time runs out.
    private void CheckAgentHungerTime()
    {
        agentHungerTimeLeft -= Time.deltaTime;
        hungerSlider.value = agentHungerTimeLeft;

        if (agentHungerTimeLeft <= 0)
        {
            Debug.Log("Prey starved");
            AddReward(-15f);
            RemoveAllPellets();
            HunterController.EndEpisode();
            EndEpisode();
        }
    }

    // Respawns the agent at a random valid location in the environment.
    public void RespawnAgent()
    {
        Vector3 spawnLocation = Vector3.zero;
        bool positionValid = false;

        while (!positionValid)
        {
            spawnLocation = new Vector3(Random.Range(-8f, 8f), 0.31f, Random.Range(-8f, 8f));
            positionValid = !CheckOverlap(spawnLocation, HunterController.transform.localPosition, 7f);
        }

        transform.localPosition = spawnLocation;

        StartAgentHungerTimer();
        RemoveAllPellets();
        CreatePellets();
    }
}
