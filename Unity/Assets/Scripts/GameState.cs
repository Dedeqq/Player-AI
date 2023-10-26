using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private Vector3 playerPosition;
    private Vector3 playerVelocity;
    private string OBSTACLE_TAG = "Obstacle";
    private Socket clientSocket;

    void Start()
    {   
        string serverIP = "127.0.0.1";  
        int serverPort = 12345;     
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        clientSocket.Connect(serverEndPoint);
    }

    private void Update()
    {
        playerPosition = transform.position;
        playerVelocity = GetComponent<Rigidbody>().velocity;
        DetectObstacles();
        SendGameState();
    }

    private void DetectObstacles()
    {
        float raycastDistance = 5f; 
        RaycastHit hit;

        Vector3 forward = transform.forward;
        Vector3 left = -transform.right;  
        Vector3 right = transform.right;
        Vector3 frontLeft = Quaternion.Euler(0, -45, 0) * forward; 
        Vector3 frontRight = Quaternion.Euler(0, 45, 0) * forward;

        if (Physics.Raycast(transform.position, forward, out hit, raycastDistance) && hit.collider.CompareTag(OBSTACLE_TAG))
        {
            Debug.Log("Obstacle detected in front at " + hit.point);
        }

        if (Physics.Raycast(transform.position, left, out hit, raycastDistance) && hit.collider.CompareTag(OBSTACLE_TAG))
        {
            Debug.Log("Obstacle detected to the left at " + hit.point);
        }

        if (Physics.Raycast(transform.position, right, out hit, raycastDistance) && hit.collider.CompareTag(OBSTACLE_TAG))
        {
            Debug.Log("Obstacle detected to the right at " + hit.point);
        }
        if (Physics.Raycast(transform.position, frontLeft, out hit, raycastDistance) && hit.collider.CompareTag(OBSTACLE_TAG))
        {
            Debug.Log("Obstacle detected between front and left at " + hit.point);
        }

        if (Physics.Raycast(transform.position, frontRight, out hit, raycastDistance) && hit.collider.CompareTag(OBSTACLE_TAG))
        {
            Debug.Log("Obstacle detected between front and right at " + hit.point);
        }
    }
    private void SendGameState()
    {
        string gameStateJson = JsonUtility.ToJson(new GameStateData(playerPosition, playerVelocity));
        byte[] data = Encoding.UTF8.GetBytes(gameStateJson);     

        // Use a separate thread for network communication
        System.Threading.Thread networkThread = new System.Threading.Thread(() =>
        {
            try
            {
                clientSocket.Send(data);
                // clientSocket.Close();
            }
            catch (Exception e)
            {
                Debug.LogError("Socket error: " + e.Message);
            }
        });

        // Start the network thread
        networkThread.Start();
    }
}

[System.Serializable]
public class GameStateData
{
    public Vector3 position;
    public Vector3 velocity;

    public GameStateData(Vector3 pos, Vector3 vel)
    {
        position = pos;
        velocity = vel;
    }
}