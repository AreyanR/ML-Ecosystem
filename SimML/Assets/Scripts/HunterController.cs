using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // To use UI components
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HunterController : Agent
{
    [SerializeField] private float moveSpeed = 4f; // Speed at which the hunter moves
    private Rigidbody rb;

    // Environment variables
    Material envMaterial;
    public GameObject env;

    public GameObject prey;
    public AgentController AgentController;

    // Hunger timer variables
    [SerializeField] private float hunterHungerDuration;
    private float hunterHungerTimeLeft;

    // Slider UI component to display hunger level
    public Slider hungerSlider;

    // Initializes the hunter and environment settings
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
        hungerSlider.maxValue = hunterHungerDuration;
    }

    // Called at the start of each episode to reset the environment and hunter
    public override void OnEpisodeBegin()
    {
        SceneSetup sceneSetup = FindObjectOfType<SceneSetup>();
        if (sceneSetup != null)
        {
            sceneSetup.ResetEnvironment();
        }

        // Set the hunter's initial position
        Vector3 spawnLocation = Vector3.zero;
        bool positionValid = false;

        while (!positionValid)
        {
            spawnLocation = new Vector3(Random.Range(-8f, 8f), 0.31f, Random.Range(-8f, 8f));
            positionValid = !AgentController.CheckOverlap(prey.transform.localPosition, spawnLocation, 7f);
        }

        transform.localPosition = spawnLocation;

        // Start the hunger timer for the hunter
        StartHunterHungerTimer();
    }

    // Updates the hunter's hunger timer each frame
    private void Update()
    {
        CheckHunterHungerTime();
    }

    // Collects observations from the environment to feed to the hunter's neural network
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(prey.transform.localPosition);
    }

    // Processes actions received from the hunter's neural network
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    // Provides manual input for testing the hunter's behavior
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // Handles interactions with trigger colliders
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Agent")
        {
            // Hunter catches the prey
            AddReward(25f);
            AgentController.AddReward(-15f);
            hunterHungerTimeLeft += 30f;
            hunterHungerTimeLeft = Mathf.Clamp(hunterHungerTimeLeft, 0, hunterHungerDuration);
            AgentController.EndEpisode();
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            // Hunter hits a wall
            AddReward(-15f);
            AgentController.RemoveAllPellets();
            AgentController.EndEpisode();
            EndEpisode();
        }
    }

    // Starts the hunger timer for the hunter
    private void StartHunterHungerTimer()
    {
        hunterHungerTimeLeft = hunterHungerDuration;
        hungerSlider.value = hunterHungerTimeLeft;
    }

    // Checks the hunter's remaining hunger time and ends the episode if time runs out
    private void CheckHunterHungerTime()
    {
        hunterHungerTimeLeft -= Time.deltaTime;
        hungerSlider.value = hunterHungerTimeLeft;

        if (hunterHungerTimeLeft <= 0)
        {
            // Hunter starves
            AddReward(-15f);
            AgentController.RemoveAllPellets();
            AgentController.EndEpisode();
            EndEpisode();
        }
    }
}
