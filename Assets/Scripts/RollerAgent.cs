using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    private Rigidbody rbRollerBall;

    public Transform target;
    public float widthOfFloor = 8f;

    public Transform floor;

    public float speed = 100f;

    private void Start()
    {
        rbRollerBall = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        //Fell off platform
        if(transform.position.y < 0)
        {
            this.rbRollerBall.velocity = Vector3.zero;
            this.rbRollerBall.angularVelocity = Vector3.zero;
            transform.position = floor.position + new Vector3(0, 0.5f, 0);
        }

        //Reset target to Random pos
        target.position = floor.position + new Vector3(Random.value * widthOfFloor - 4, 0.5f, Random.value * widthOfFloor - 4);
    }

    public override void CollectObservations()
    {
        //Relative Position
        Vector3 relativePos = target.position - this.transform.position;
        AddVectorObs(relativePos.x);
        AddVectorObs(relativePos.z);

        //Distance to centre
        AddVectorObs(this.transform.position.x - floor.position.x);
        AddVectorObs(this.transform.position.z - floor.position.z);

////// 8 obs (8 observation/ 8x1 matrix)	//positions of target and player
	//AddVectorObs(target.position);
	//AddVectorObs(this.transform.position);

        //Velocity of Ball
        AddVectorObs(this.rbRollerBall.velocity.x);
        AddVectorObs(this.rbRollerBall.velocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rbRollerBall.AddForce(controlSignal * speed * Time.deltaTime);

        //Rewards
        float distanceToTarget = Vector3.Distance(target.position, this.transform.position);

        //Reached target
        if(distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            Done();
        }

        //Fell off platform
        if(this.transform.position.y < 0)
        {
            SetReward(-0.1f);
            Done();
        }
    }

}
