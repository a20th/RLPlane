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
    private float MAX_DISTANCE = Vector3.Distance(new Vector3(0, 40, -130), new Vector3(100, 50, 40));
    public const float RayCastLength = 250;
    
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

    private enum DISACTIONS
    {
        NOTHING = 0,
        LEFT = 1,
        RIGHT = 2
    }

    public float speed = 5f;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Score"))
        {
            AddReward(10);
            Debug.Log("Betalált");
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
        
        //Debug.Log(Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition));
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
        rb.velocity = Vector3.zero;
        if (rb.velocity != Vector3.zero)
        {
            Debug.Log(rb.velocity);
        }

        //var actionTaken = actions.DiscreteActions[0];
        var yawAction = actions.DiscreteActions[0];
        var pitchAction = actions.DiscreteActions[1];
        var rollAction = actions.DiscreteActions[2];
        

        switch (yawAction)
        {
            case (int)DISACTIONS.NOTHING: break;
            case (int)ACTIONS.LEFT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.down);
                break;
            case (int)ACTIONS.RIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.up);
                break;
        }

        switch (pitchAction)
        {
            case (int)DISACTIONS.NOTHING: break;
            case (int)ACTIONS.LEFT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.right);
                break;
            case (int)ACTIONS.RIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.left);
                break;
        }

        /*
        switch (rollAction)
        {
            case (int)DISACTIONS.NOTHING: break;
            case (int)ACTIONS.LEFT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.forward);
                break;
            case (int)ACTIONS.RIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.back);
                break;
        }
        */

        /*
        switch (actionTaken)
        {
            case (int)ACTIONS.FORWARD:
                break;
            case (int)ACTIONS.LEFT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.down);
                break;
            case (int)ACTIONS.RIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.up);
                break;
            case (int)ACTIONS.UP:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.right);
                break;
            case (int)ACTIONS.DOWN:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.left);
                break;
            case (int)ACTIONS.ROLLLEFT: 
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.forward);
                break;
            case (int)ACTIONS.ROLLRIGHT:
                transform.Rotate(20 * Time.fixedDeltaTime * Vector3.back);
                break;
        }
        */

        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);

        if (transform.localPosition.y > 100 || transform.localPosition.y < 0 || Mathf.Abs(transform.localPosition.x) > width || Mathf.Abs(transform.localPosition.z) > width) 
        {
            AddReward(-10);
            EndEpisode();
        }
        
        var heading = (spawner.obj.transform.localPosition - transform.localPosition).normalized;
        var dot = Vector3.Dot(heading, transform.forward);
        AddReward(dot);
        /*float scaledDistance = Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition) / MAX_DISTANCE;
        Debug.Log(scaledDistance);
        AddReward(scaledDistance / 1000);*/
        //Debug.Log(dot);
        /*if (DoARaycast(transform.forward) == 1)
        {
            AddReward(5);
        }
        AddReward(dot);
        */
        /*
        else
        {
            if(dot > 0.8)
            {
                AddReward(dot + 4);
            }
            else if(dot > 0)
            {
                AddReward(-dot / 2);
            }
            else
            {
                AddReward(dot);
            }
        //}*/
        /*
        float scaledDistance = Vector3.Distance(transform.localPosition, spawner.obj.transform.localPosition) / MAX_DISTANCE;
        AddReward(scaledDistance / 1000 * dot);*/
        /*
        if(dot > 0.9999)
        {
            AddReward(7);
        }
        else if(dot > 0.99)
        {
            AddReward(2);
        }
        else
        {
            AddReward(dot);
        }
        */
        //AddReward(-0.01f);
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