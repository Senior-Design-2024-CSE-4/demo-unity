using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Client c;
    int num = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer("127.0.0.1", 12345);
    }

    // Update is called once per frame
    void Update()
    {
        string data = c.GetCurrentData();
        Debug.Log(data);
        c.Send("hello:" + num.ToString());
        this.num++;
    }

    public void ConnectToServer(string host, Int32 port)
    {
        this.c = new Client();
        this.c.Connect(host, port);
    }
}
