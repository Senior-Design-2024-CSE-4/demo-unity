using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Client c;
    int num = 0;

    private Vector3 goal;
    
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer("127.0.0.1", 12345);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = GetAngleToGoal();
        Debug.Log(angle.ToString());
        c.Send(angle.ToString());
    }

    void OnApplicationQuit()
    {
        c.Close();
    }

    public void ConnectToServer(string host, Int32 port)
    {
        this.c = new Client();
        this.c.Connect(host, port);
        this.c.Send("s:unity");
        this.c.Send("belt");
    }

    public void SetGoal(Transform t)
    {
        this.goal = t.position;
    }

    private float GetAngleToGoal()
    {
        Vector3 forward = this.gameObject.transform.forward;
        Vector3 targetDir = goal - this.gameObject.transform.position;
        float angle = Vector3.Angle(targetDir, forward);
        if (Vector3.Cross(forward, targetDir).y < 0)
        {
            Debug.Log("Left");
            return 360f - angle;
        } 
        else
        {
            Debug.Log("Right");
            return angle;
        }
    }
}
