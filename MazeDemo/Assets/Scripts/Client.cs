using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Class <c>Client</c> represents a client connection to the server.
/// Data gets updated by predefined hertz, and can then be called with <c>GetCurrentData()</c>.
/// <example>
/// For example:
/// <code>
/// Client c = new Client();
/// string message = c.GetCurrentData();
/// </code>
/// </example>
/// </summary>
public class Client
{

    private TcpClient tcpSocket;
    private Thread thread;
    // The rate at which the client listens to the server
    private double targetHz = 120.0;

    private string currentData = "";

    public void Close()
    {
        this.tcpSocket.Close();
    }

    public void SetHz(double hz)
    {
        this.targetHz = hz;
    }

    public double GetHz()
    {
        return this.targetHz;
    }

    public void Connect(string host, Int32 port)
    {
        try
        {
            tcpSocket = new TcpClient(host, port);
            thread = new Thread( new ThreadStart(Listen));
            thread.IsBackground = true;
            thread.Start();
            Send("b:unity");
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e); 	
        }
    }

    private void Listen()
    {
        Byte[] buffer = new Byte[1024];
        double update_time = 1.0/targetHz;
        DateTime last_time = DateTime.Now;
        while (true)
        {
            DateTime current_time = DateTime.Now;
            // Get a stream object for reading 				
            using (NetworkStream stream = tcpSocket.GetStream()) 
            { 					
                if ((current_time - last_time).TotalSeconds < update_time)
                {
                    Debug.Log("flush");
                    stream.Read(buffer, 0, buffer.Length);
                }
                else
                {
                    last_time = current_time;
                    
                    int length; 					
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(buffer, 0, buffer.Length)) != 0) 
                    { 						
                        var incommingData = new byte[length]; 						
                        Array.Copy(buffer, 0, incommingData, 0, length); 						
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData); 						
                        Debug.Log("server message received as: " + serverMessage); 		
                        lock (this.currentData)
                        {
                            this.currentData = serverMessage;
                        }			
                    } 	
                }			
            } 	
        }
    }

    public string GetCurrentData()
    {
        lock (this.currentData)
        {
            return this.currentData;
        }
    }

    public void Send(string message)
    {
        if (tcpSocket == null) {             
			return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = tcpSocket.GetStream(); 			
			if (stream.CanWrite) {                 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent: " + message);             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}  
    }
}
