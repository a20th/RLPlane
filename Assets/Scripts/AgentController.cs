using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class AgentController : Agent
{
    public Spawner spawner;
    public Rigidbody rb;
    private const float width = 150;
    private float MAX_DISTANCE = Vector3.Distance(new Vector3(-width, 20, -width), new Vector3(width, 60, width));

    private enum ACTIONS
    {
        FORWARD = 0,
        LEFT = 1,
        RIGHT = 2,
        UP = 3,
        DOWN = 4,
        ROLLLEFT = 5, 
        ROLLRIGHT = 6
    }
    public float speed = 5f;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Score"))
        {
            AddReward(50);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var rollleft = Input.GetAxisRaw("Fire1");
        var rollright = Input.GetAxisRaw("Fire2");


        if (horizontal == -1)
        {
            actions[0] = (int)ACTIONS.LEFT;
        }
        else if (vertical == +1)
        {
            actions[0] = (int)ACTIONS.UP;
        }
        else if (vertical == -1)
        {
            actions[0] = (int)ACTIONS.DOWN;
        }
        else if (horizontal == +1)
        {
            actions[0] = (int)ACTIONS.RIGHT;
        }
        else if (rollleft == +1)
        {
            actions[0] = (int)ACTIONS.ROLLLEFT;
        }
        else if (rollright == +1)
        {
            actions[0] = (int)ACTIONS.ROLLRIGHT;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition));
        //Debug.Log(Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition));
        var heading = (spawner.obj.transform.localPosition - transform.localPosition).normalized;
        var dot = Vector3.Dot(heading, transform.forward);

        //a
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(spawner.obj.transform.localPosition);
        sensor.AddObservation(dot);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.DiscreteActions[0];
        if(rb.velocity != Vector3.zero)
        {
            Debug.Log("Force:" + rb.velocity.ToString());
        }
        switch (actionTaken)
        {
            case (int)ACTIONS.FORWARD:
                break;
            case (int)ACTIONS.LEFT:
                transform.Rotate(Vector3.down * Time.fixedDeltaTime * 20);
                break;
            case (int)ACTIONS.RIGHT:
                transform.Rotate(Vector3.up * Time.fixedDeltaTime * 20);
                break;
            case (int)ACTIONS.UP:
                transform.Rotate(Vector3.right * Time.fixedDeltaTime * 20);
                break;
            case (int)ACTIONS.DOWN:
                transform.Rotate(Vector3.left * Time.fixedDeltaTime * 20);
                break;
            case (int)ACTIONS.ROLLLEFT: 
                transform.Rotate(Vector3.forward * Time.fixedDeltaTime * 20);
                break;
            case (int)ACTIONS.ROLLRIGHT:
                transform.Rotate(Vector3.back * Time.fixedDeltaTime * 20);
                break;
        }
        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);

        if (transform.localPosition.y > 100 || transform.localPosition.y < 0 || Mathf.Abs(transform.localPosition.x) > width || Mathf.Abs(transform.localPosition.z) > width) 
        {
            AddReward(-1);
            EndEpisode();
        }
        var heading = (spawner.obj.transform.localPosition - transform.localPosition).normalized;
        var dot = Vector3.Dot(heading, transform.forward);
        /*float scaledDistance = Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition) / MAX_DISTANCE;
        AddReward(scaledDistance / 1000 * dot);*/
        AddReward(dot / 10);
        Debug.Log(dot);
        //AddReward(-0.01f);
    }

    public override void OnEpisodeBegin()
    {
        spawner.Spawn(new Vector3(100, 40, 60));
        transform.SetLocalPositionAndRotation(new Vector3(0, 50, -130), Quaternion.Euler(0,0,0));
        rb.velocity = Vector3.zero;
    }
}

/* Continous implementation
        float roll = actionTaken[0];
        float pitch = actionTaken[1];
        float yaw = actionTaken[2];

        transform.Rotate(rollSpeed * roll * Time.fixedDeltaTime * Vector3.forward);
        transform.Rotate(pitchSpeed * pitch * Time.fixedDeltaTime * Vector3.right);
        transform.Rotate(yawSpeed * yaw * Time.fixedDeltaTime * Vector3.up);
        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);
*/