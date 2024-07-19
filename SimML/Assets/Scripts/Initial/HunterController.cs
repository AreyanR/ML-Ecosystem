using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HunterController : Agent
{
  

//Hunter variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

// env variables
    Material envMaterial;
    public GameObject env;

    public GameObject prey;
    public AgentController AgentController;




    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
    }

    public override void OnEpisodeBegin()
    {

        //Hunter 
        Vector3 spawnlocation = new Vector3(Random.Range(-4f,4f),0.31f,Random.Range(-4f,4f));
        
        bool distanceGood = AgentController.CheckOverlap(prey.transform.localPosition, spawnlocation, 5f);

        while(!distanceGood)
        {
            spawnlocation = new Vector3(Random.Range(-4f,4f),0.31f,Random.Range(-4f,4f));
            distanceGood = AgentController.CheckOverlap(prey.transform.localPosition, spawnlocation, 5f);
        }
        
        transform.localPosition = spawnlocation;
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
        if(other.gameObject.tag == "Agent")
        {
            //Remove from list
            AddReward(10f);
            AgentController.AddReward(-13f);
            envMaterial.color = Color.yellow;
            AgentController.EndEpisode();
            EndEpisode();
            
        }

        if(other.gameObject.tag == "Wall")
        {
            envMaterial.color = Color.red;
            AddReward(-15f);
            AgentController.EndEpisode();
            EndEpisode();
        }
    }


}
