using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : Agent
{
    [SerializeField] Perception perception;
    [SerializeField] Perception flockPerception;
    [SerializeField] ObstaclePerception obstaclePerception;
    [SerializeField] Steering steering;
    [SerializeField] AutonomousAgentData agentData;

    public float maxSpeed { get { return movement.maxSpeed; } }
    public float maxForce { get { return movement.maxForce; } }
    public Vector3 velocity { get; set; } = Vector3.zero;

    void Update()
    {
        GameObject[] gameObjects = perception.GetGameObjects();
        if(gameObjects.Length == 0)
        {
            movement.ApplyForce(steering.Wander(this));
        }
        //seek || flee
        if(gameObjects.Length != 0)
        {
            Debug.DrawLine(transform.position, gameObjects[0].transform.position);


            movement.ApplyForce(steering.Seek(this, gameObjects[0]) * agentData.seekWeight);
            movement.ApplyForce(steering.Flee(this, gameObjects[0]) * agentData.fleeWeight);
        }
        gameObjects = flockPerception.GetGameObjects();
        if (gameObjects.Length != 0)
        {
            movement.ApplyForce(steering.Cohesion(this, gameObjects) * agentData.cohesionWeight);
            movement.ApplyForce(steering.Seperation(this, gameObjects, agentData.separationRadius) *agentData.separationWeight);
            movement.ApplyForce(steering.Alignment(this, gameObjects) * agentData.alignmentWeight);
        }
        if(movement.acceleration.sqrMagnitude <= movement.maxForce * 0.1f)
        {
            movement.ApplyForce(steering.Wander(this));
        }        
    }
}
