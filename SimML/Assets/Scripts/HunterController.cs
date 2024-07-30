using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Add this to use the UI components
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HunterController : Agent
{
    // Hunter variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    // Env variables
    Material envMaterial;
    public GameObject env;

    public GameObject prey;
    public AgentController AgentController;

    // Hunger timer variables
    [SerializeField] private float hunterHungerDuration;
    private float hunterHungerTimeLeft;

    // Slider UI component
    public Slider hungerSlider;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
        hungerSlider.maxValue = hunterHungerDuration;
    }

   public override void OnEpisodeBegin()
{
    SceneSetup sceneSetup = FindObjectOfType<SceneSetup>();
    if (sceneSetup != null)
    {
        sceneSetup.ResetEnvironment();
    }

    // Hunter
    Vector3 spawnLocation = Vector3.zero;
    bool positionValid = false;

    while (!positionValid)
    {
        spawnLocation = new Vector3(Random.Range(-8f, 8f), 0.31f, Random.Range(-8f, 8f));
        positionValid = !AgentController.CheckOverlap(prey.transform.localPosition, spawnLocation, 7f); // Ensure it's at least 5 units away from the prey
    }

    transform.localPosition = spawnLocation;

    // Hunger timer for hunter
    StartHunterHungerTimer();
}


    private void Update()
    {
        CheckHunterHungerTime();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(prey.transform.localPosition);
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
        if (other.gameObject.tag == "Agent")
        {
            // Remove from list
            AddReward(25f);
            AgentController.AddReward(-15f);
            // Add 2 seconds to hunter's hunger timer
            hunterHungerTimeLeft += 30f;
            hunterHungerTimeLeft = Mathf.Clamp(hunterHungerTimeLeft, 0, hunterHungerDuration); // Clamp the hunger time
            Debug.Log("Hunter ate prey");
            AgentController.EndEpisode();
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            Debug.Log("Hunter hit wall");
            AddReward(-15f);
            AgentController.RemoveAllPellets(); // Remove all pellets when hitting a wall
            AgentController.EndEpisode();
            EndEpisode();
        }
    }

    private void StartHunterHungerTimer()
    {
        hunterHungerTimeLeft = hunterHungerDuration;
        hungerSlider.value = hunterHungerTimeLeft;
    }

    private void CheckHunterHungerTime()
    {
        hunterHungerTimeLeft -= Time.deltaTime;
        hungerSlider.value = hunterHungerTimeLeft;

        if (hunterHungerTimeLeft <= 0)
        {
            Debug.Log("Hunter starved");
            AddReward(-15f);
            AgentController.RemoveAllPellets(); // Remove all pellets when starving
            AgentController.EndEpisode();
            EndEpisode();
        }
    }

}
