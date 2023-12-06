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
    private const float width = 50;
    public const float RayCastLength = 250;
    public float speed = 5f;

    private enum DISACTIONS
    {
        NOTHING = 0,
        LEFT = 1,
        RIGHT = 2
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Score"))
        {
            AddReward(10);
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
        /*
         DISACTIONS.NOTHING by default.
         */
        if (horizontal == -1)
        {
            actions[0] = (int)DISACTIONS.LEFT;
        }
        else if (horizontal == +1)
        {
            actions[0] = (int)DISACTIONS.RIGHT;
        }

        if (vertical == +1)
        {
            actions[1] = (int)DISACTIONS.LEFT;
        }
        else if (vertical == -1)
        {
            actions[1] = (int)DISACTIONS.RIGHT;
        } 

        if (rollleft == +1)
        {
            actions[2] = (int)DISACTIONS.LEFT;
        }
        else if (rollright == +1)
        {
            actions[2] = (int)DISACTIONS.RIGHT;
        }        
    }

    private int DoARaycast(Vector3 direction)
    {
        Ray landingRay = new Ray(transform.position, direction);

        if (Physics.Raycast(landingRay, out RaycastHit hit, RayCastLength) && hit.collider.CompareTag("Score"))
        {
            Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
            return 1;
        }
        else
        {
            //Debug.DrawLine(transform.localPosition, transform.localPosition + direction * RayCastLength, Color.green);
            return 0;
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {    
        var heading = (spawner.obj.transform.localPosition - transform.localPosition).normalized;
        var dot = Vector3.Dot(heading, transform.forward);

        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(spawner.obj.transform.localPosition);
        sensor.AddObservation(dot);
        sensor.AddObservation(Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition));
        //sensor.AddObservation(DoARaycast(transform.forward));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
        if (rb.velocity != Vector3.zero)
        {
            Debug.Log(rb.velocity);
        }
        rb.velocity = Vector3.zero;

        var yawAction = actions.DiscreteActions[0];
        var pitchAction = actions.DiscreteActions[1];
        var rollAction = actions.DiscreteActions[2];       

        switch (yawAction)
        {
            case (int)DISACTIONS.NOTHING: break;
            case (int)DISACTIONS.LEFT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.down);
                break;
            case (int)DISACTIONS.RIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.up);
                break;
        }

        switch (pitchAction)
        {
            case (int)DISACTIONS.NOTHING: break;
            case (int)DISACTIONS.LEFT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.right);
                break;
            case (int)DISACTIONS.RIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.left);
                break;
        }

        switch (rollAction)
        {
            case (int)DISACTIONS.NOTHING: break;
            case (int)DISACTIONS.LEFT:
                transform.Rotate(10 * Time.fixedDeltaTime * Vector3.forward);
                break;
            case (int)DISACTIONS.RIGHT:
                transform.Rotate(10 * Time.fixedDeltaTime * Vector3.back);
                break;
        }

        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);

        if (transform.localPosition.y > 100 || transform.localPosition.y < 0 || Mathf.Abs(transform.localPosition.x) > width || Mathf.Abs(transform.localPosition.z) > width) 
        {
            AddReward(-10);
            EndEpisode();
        }
        
        var heading = (spawner.obj.transform.localPosition - transform.localPosition).normalized;
        var dot = Vector3.Dot(heading, transform.forward);
        AddReward(dot);
        if(transform.up.y > 0)
        {
            AddReward(0.001f);
        }
        else
        {
            AddReward(-0.001f);
        }
    }

    public override void OnEpisodeBegin()
    {
        spawner.Spawn(new Vector3(40, 50, 40));
        transform.SetLocalPositionAndRotation(new Vector3(0, 40, -40), Quaternion.Euler(0,0,0));
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