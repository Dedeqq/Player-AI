using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameState : MonoBehaviour
{
    private Vector3 playerPosition;
    private string OBSTACLE_TAG = "Obstacle";
    private Socket clientSocket;
    private bool toMove;
    private float gameResetDepth = -2f;
    MovementData receivedMovement;

    void Start()
    {
        string serverIP = "127.0.0.1";
        int serverPort = 12345;
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        clientSocket.Connect(serverEndPoint);
        toMove = false;
    }

    private void Update()
    {   
        if (transform.position.y < gameResetDepth)
        {
            ResetGame();
        }
        if (toMove)
        {
            PerformAction();
            toMove = false; 
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

    private void SendMessage(string message)
    {
        byte[] messageData = Encoding.UTF8.GetBytes(message);

        try
        {
            clientSocket.Send(messageData);
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error: " + e.Message);
        }
    }


    private void ReceiveAction()
    {
        byte[] movementData = new byte[1024];
        int bytesRead = clientSocket.Receive(movementData);

        if (bytesRead > 0)
        {
            string movementJson = Encoding.UTF8.GetString(movementData, 0, bytesRead);
            receivedMovement = JsonConvert.DeserializeObject<MovementData>(movementJson);
            toMove = true;

            // Now you can use the values of verticalMovement and horizontalMovement
            Debug.Log("Received Vertical Movement: " + receivedMovement.verticalMovement);
            Debug.Log("Received Horizontal Movement: " + receivedMovement.horizontalMovement);
        }
    }

    private void PerformAction()
    {
        float moveSpeed = 5f;
        Vector3 moveVector = new Vector3(receivedMovement.horizontalMovement * moveSpeed, 0, receivedMovement.verticalMovement * moveSpeed);
        transform.position += moveVector * Time.deltaTime;
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

    private void ResetGame()
    {   
        byte[] data = Encoding.UTF8.GetBytes("GAME_END");

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
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        });

        networkThread.Start();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(OBSTACLE_TAG))
        {
            ResetGame();
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

[Serializable]
public class MovementData
{
    public float verticalMovement;
    public float horizontalMovement;
}
