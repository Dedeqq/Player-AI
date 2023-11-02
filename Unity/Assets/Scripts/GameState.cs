using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private Vector3 playerPosition;
    private string OBSTACLE_TAG = "Obstacle";
    private Socket clientSocket;
    private string nextPlayerMove;

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
        if (!string.IsNullOrEmpty(nextPlayerMove))
        {
            PerformAction(nextPlayerMove);
            nextPlayerMove = null; // Reset the action
        }

        GetGameState();
        SendGameState();
        ReceiveAction();
    }

    private void GetGameState()
    {
        playerPosition = transform.position;
    }

    private void SendGameState()
    {
        string gameStateJson = JsonUtility.ToJson(new GameStateData(playerPosition));
        byte[] data = Encoding.UTF8.GetBytes(gameStateJson);

        System.Threading.Thread networkThread = new System.Threading.Thread(() =>
        {
            try
            {
                clientSocket.Send(data);
            }
            catch (Exception e)
            {
                Debug.LogError("Socket error: " + e.Message);
            }
        });

        networkThread.Start();
    }

    private void ReceiveAction()
    {
        byte[] actionData = new byte[1024];
        int bytesRead = clientSocket.Receive(actionData);

        if (bytesRead > 0)
        {
            string actionJson = Encoding.UTF8.GetString(actionData, 0, bytesRead);
            nextPlayerMove = actionJson;
            Debug.Log(nextPlayerMove);
        }
    }

    private void PerformAction(string moveDirection)
    {
        float moveSpeed = 5f;
        Vector3 moveVector = Vector3.zero;

        switch (moveDirection)
        {
            case "move_forward":
                moveVector = Vector3.forward;
                break;
            case "move_left":
                moveVector = Vector3.left;
                break;
            case "move_right":
                moveVector = Vector3.right;
                break;
            case "move_back":
                moveVector = Vector3.back;
                break;
            default:
                Debug.LogWarning("Unknown move direction: " + moveDirection);
                break;
        }

        // Move the player based on the calculated move vector
        transform.Translate(moveVector * moveSpeed * Time.deltaTime);
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
}

[System.Serializable]
public class GameStateData
{
    public Vector3 position;

    public GameStateData(Vector3 pos)
    {
        position = pos;
    }
}
