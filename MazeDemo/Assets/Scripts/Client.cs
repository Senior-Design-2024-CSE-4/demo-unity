using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{

    private TcpClient tcpSocket;
    private Thread thread;


    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Connect()
    {
        try
        {
            tcpSocket = new TcpClient("127.0.0.1", 12345);
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
        Byte[] bytes = new Byte[1024];
        while (true)
        {
            // Get a stream object for reading 				
            using (NetworkStream stream = tcpSocket.GetStream()) { 					
                int length; 					
                // Read incomming stream into byte arrary. 					
                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
                    var incommingData = new byte[length]; 						
                    Array.Copy(bytes, 0, incommingData, 0, length); 						
                    // Convert byte array to string message. 						
                    string serverMessage = Encoding.ASCII.GetString(incommingData); 						
                    Debug.Log("server message received as: " + serverMessage); 					
                } 				
            } 	
        }
    }

    private void Send(string message)
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
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}  
    }
}
