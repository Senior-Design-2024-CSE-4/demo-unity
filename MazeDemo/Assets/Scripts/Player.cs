using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Client c;

    private Vector3 goal;
    private float maxDistance;

    // 0 is for single direction, 1 is for all direction
    private int surroundMode;
    // 0 is for compass, 1 is for angle
    private int beltMode;

    // 0: direction to goal
    // 1: distance from goal
    // 2: compass
    // 3: pathfinding
    private int navigationMode;
    
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer("127.0.0.1", 12345);
        this.surroundMode = 0;
        this.beltMode = 1;
        this.navigationMode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int angle = 0;
        int intensity = 100;
        switch (this.navigationMode)
        {
            case 0:
                angle = GetAngleToGoal();
                Send(surroundMode, beltMode, angle, intensity);
                break;
            case 1:
                intensity = GetDistanceIntensity();
                Send(surroundMode, beltMode, angle, intensity);
                break;
            case 2:
                angle = GetAngleToNorth();
                Send(surroundMode, beltMode, angle, intensity);
                break;
            case 3:
                break;
            default:
                Debug.Log("Invalid navigation mode.");
                break;
        }
    }

    void UnloadPlayer()
    {
        c.Close();
    }

    public void ConnectToServer(string host, Int32 port)
    {
        this.c = new Client();
        this.c.Connect(host, port);
        this.c.Send("s:unity");
        this.c.Send("ubelt");
    }

    public void SetGoal(Transform t)
    {
        this.goal = t.position;
    }

    public void SetMaxDistance(float d)
    {
        this.maxDistance = d;
    }
    
    public void SetSurroundMode(int surroundMode)
    {
        this.surroundMode = surroundMode;
    }
    private void SetBeltMode(int beltMode)
    {
        this.beltMode = beltMode;
    }
    public void SetModes(int surroundMode, int beltMode)
    {
        this.surroundMode = surroundMode;
        this.beltMode = beltMode;
    }

    private int GetAngleToGoal()
    {
        Vector3 forward = this.gameObject.transform.forward;
        Vector3 targetDir = goal - this.gameObject.transform.position;
        float angle = Vector3.Angle(targetDir, forward);
        if (Vector3.Cross(forward, targetDir).y < 0)
        {
            return (int)(360f - angle);
        } 
        else
        {
            return (int)angle;
        }
    }

    private int GetDistanceIntensity()
    {
        Vector3 difference = goal - this.gameObject.transform.position;
        float distance = difference.magnitude;
        return (int)(100f * distance / maxDistance);
    }

    private int GetAngleToNorth()
    {
        Vector3 forward = this.gameObject.transform.forward;
        Vector3 targetDir = Vector3.forward;
        float angle = Vector3.Angle(targetDir, forward);
        if (Vector3.Cross(forward, targetDir).y < 0)
        {
            return (int)(360f - angle);
        } 
        else
        {
            return (int)angle;
        }
    }

    private void Send(int surround, int mode, int angle, int intensity)
    {
        // Either 0 or 1; 0 is for single direction, 1 is for all directions
        if (surround < 0 || surround > 1)
        {
            Debug.Log("Invalid surround value " + surround + ". Nothing sent.");
            return;
        }

        // 0 for compass mode, 1 for angle mode
        if (mode < 0 || mode > 1)
        {
            Debug.Log("Invalid mode value " + mode + ". Nothing sent.");
            return;
        }

        if (angle < 0 || angle > 359)
        {
            Debug.Log("Invalid angle value " + angle + ". Nothing sent.");
            return;
        }

        if (intensity < 0 || intensity > 100)
        {
            Debug.Log("Invalid intensity value " + intensity + ". Nothing sent.");
            return;
        }

        Debug.Log("unity:" + surround + "," + mode + "," + angle.ToString("000") + "," + intensity.ToString("000"));
        c.Send("unity:" + surround + "," + mode + "," + angle.ToString("000") + "" + intensity.ToString("000"));

    }
}
