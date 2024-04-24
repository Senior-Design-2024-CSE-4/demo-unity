using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeContainer : MonoBehaviour
{
    /// <summary>
    /// Statics - variables pertaining to the entire mazecontainer class
    /// </summary>

    // This is so that certain activity is paused if the maze is still generating
    public static bool MAZE_GENERATED;
    /// <summary>
    /// Prefabs - templates that the maze uses to render objects
    /// </summary>
    
    // These are all set to public so that the prefabs can be dragged in via unity editor
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject postPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab;

    /// <summary>
    /// Objects - physical instances of everything in the maze
    /// </summary>
    
    // Private; only the maze container should manipulate its own contents
    private GameObject floor;
    private GameObject ceiling;
    private GameObject[] posts;
    private GameObject[] verticals;
    private GameObject[] horizontals;
    private GameObject player;
    private GameObject goal;

    /// <summary>
    /// Dimensions - describes the size of the maze/components
    /// </summary>
    
    // These are instantiated in the unity editor - alternatively, these are public so another function can change them manually
    public int width;
    public int height;

    // These settings made for the best looking maze - feel free to manually change. Set to public if you want another function to change these
    private float cellSize = 10.0f;
    private float wallWidth = 1.0f;
    private float wallHeight = 7.0f;
    private float squareWidth
    {
        get
        {
            return this.cellSize + this.wallWidth;
        }
    }

    /// <summary>
    /// Data variables - holds relevant information regarding the maze state
    /// </summary>

    // Feel free to try different types of mazes with this, so long as it's grid-like
    private MazeData data;

    // You can subclass from MazeGenerationFunction if you want to experiment with different maze generations
    private MazeGenerationFunction gen;

    // An array that contains the distances from every square to the goal.
    private int[] distanceArray; 
    private (int, int) goalLocation;
    private (int, int) playerPosition;
    
    /// <summary>
    /// Unity override functions
    /// </summary>
    /// 
    
    // Start is called at creation
    void Start()
    {
        MazeContainer.MAZE_GENERATED = false;
        InitializeMazeContainer();
        GenerateMaze();
        MazeContainer.MAZE_GENERATED = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (MazeContainer.MAZE_GENERATED)
        {
            SetNextCell();
        }
    }

    /// <summary>
    /// Navigation Functions - functions that help the player navigate around
    /// </summary>

    // Sets the navigation settings for the player - see Player.cs for more information
    public void SetPlayerNavigationMode(int mode)
    {
        this.player.GetComponent<Player>().SetNavigationMode(mode);
    }

    // Returns the grid coordinates of the player
    (int, int) GetPlayerPosition()
    {
        float squaresize = this.cellSize + this.wallWidth;
        int x = Mathf.FloorToInt(this.player.transform.position.x / squaresize);
        int y = Mathf.FloorToInt(this.player.transform.position.z / squaresize);
        return (x, y);
    }

    // Gets the exact center of a square in the maze
    public Vector3 GetSquareCenter((int, int) coords)
    {
        float x = (coords.Item1 + 0.5f) * this.squareWidth;
        float z = (coords.Item2 + 0.5f) * this.squareWidth;
        return new Vector3(x, 0, z);
    }

    // Sets the cell for the player to navigate to - only useful for pathfinding mode
    void SetNextCell()
    {
        (int, int) playerCoords = GetPlayerPosition();
        int player_x = playerCoords.Item1;
        int player_y = playerCoords.Item2;
        List<int> neighbors = this.gen.GetOpenNeighbors(this.gen.Point2Index(player_x, player_y));
        int current_distance = this.distanceArray[this.gen.Point2Index(player_x, player_y)];
        Debug.Log("PLAYER DISTANCE: " + current_distance);
        foreach(int neighbor in neighbors)
        {
            if (this.distanceArray[neighbor] == current_distance - 1)
            {
                Debug.Log("GO TO: " + this.gen.Index2Point(neighbor));
                this.player.GetComponent<Player>().SetNearestSquare(GetSquareCenter(this.gen.Index2Point(neighbor)));
            }
        }
    }

    /// <summary>
    /// Generation functions - functions that help with creating and manipulating mazes
    /// </summary>

    void DestroyMaze()
    {
        for (int i = 0; i < posts.Length; i++)
        {
            Destroy(posts[i]);
        }
        for (int i = 0; i < verticals.Length; i++)
        {
            Destroy(verticals[i]);
        }
        for (int i = 0; i < horizontals.Length; i++)
        {
            Destroy(horizontals[i]);
        }

        Destroy(this.floor);
        Destroy(this.ceiling);
    }

    public void RestartMaze()
    {
        MazeContainer.MAZE_GENERATED = false;
        DestroyMaze();
        GenerateMaze();
        MazeContainer.MAZE_GENERATED = true;
    }

    void GenerateMaze() 
    {
        this.data = new MazeData(this.width, this.height);
        this.gen = new MazeGenerationFunction();
        gen.Generate(this.data, 0, 0);
        this.distanceArray = this.gen.GetDistanceArray(this.goalLocation.Item1, this.goalLocation.Item2);
        RenderFloor();
        RenderCeiling();
        RenderPosts();
        RenderWalls();
        MovePlayer((0, 0));
        MoveGoal((this.width - 1, this.height - 1));
        this.player.GetComponent<Player>().SetGoal(this.goal.transform.position);
        Vector3 distance = new Vector3(this.squareWidth * this.width + wallWidth, 0, this.squareWidth * this.height + wallWidth);
        this.player.GetComponent<Player>().SetMaxDistance(distance.magnitude);
    }

    public void IncreaseLevel()
    {
        this.width += 1;
        this.height += 1;
        // Restarts so that player resets to (0, 0)
        RestartMaze();
    }

    /// <summary>
    /// Functions regarding the manipulation/creation of objects 
    /// </summary>
    
    void InitializeMazeContainer()
    {
        SpawnPlayer((0, 0));
        SpawnGoal((this.width - 1, this.height - 1));
    }

    void SpawnPlayer((int, int) coords)
    {
        this.playerPosition = coords;
        this.player = Instantiate(this.playerPrefab, new Vector3((coords.Item1 + 0.5f) * this.squareWidth, 1f, (coords.Item2 + 0.5f) * this.squareWidth), Quaternion.identity);
    }

    void MovePlayer((int, int) coords)
    {   
        this.playerPosition = coords;
        Vector3 newPosition = GetSquareCenter(coords);
        newPosition.y += 1f;
        // Character controller has to be disabled, otherwise it will not teleport correctly.
        // Otherwise, the controller has priority over player position.
        // This means that the MazeContainer would not be able to change positions.
        CharacterController cc = this.player.GetComponent<CharacterController>();
        cc.enabled = false;
        this.player.transform.position = newPosition;
        // Resets rotation as well
        this.player.transform.rotation = Quaternion.identity;
        cc.enabled = true;
    }
    void SpawnGoal((int, int) coords)
    {
        this.goalLocation = coords;
        this.goal = Instantiate(this.goalPrefab, new Vector3((coords.Item1 + 0.5f) * this.squareWidth, 2f, (coords.Item2 + 0.5f) * this.squareWidth), Quaternion.identity);
    }
    void MoveGoal((int, int) coords)
    {
        this.goalLocation = coords;
        Vector3 newPosition = GetSquareCenter(coords);
        newPosition.y += 2f;
        this.player.GetComponent<Player>().SetGoal(this.goal.transform.position);
    }

    void RenderFloor()
    {
        Vector3 position = new Vector3((this.squareWidth * this.width + wallWidth) / 2, -0.5f, (this.squareWidth * this.height + wallWidth) / 2);
        Vector3 scale = new Vector3(this.squareWidth * this.width + wallWidth, 1.0f, this.squareWidth * this.height + wallWidth);
        this.floor = Instantiate(this.floorPrefab, position, Quaternion.identity);
        this.floor.transform.localScale = scale;
        RenderFloorTexture(this.floor.GetComponentInChildren<MeshRenderer>(), scale);
    }

    void RenderCeiling()
    {
        Vector3 position = new Vector3((this.squareWidth * this.width + wallWidth) / 2, this.wallHeight + 0.4f, (this.squareWidth * this.height + wallWidth) / 2);
        Vector3 scale = new Vector3(this.squareWidth * this.width + wallWidth, 1.0f, this.squareWidth * this.height + wallWidth);
        this.ceiling = Instantiate(this.ceilingPrefab, position, Quaternion.identity);
        this.ceiling.transform.localScale = scale;
        RenderCeilingTexture(this.ceiling.GetComponentInChildren<MeshRenderer>(), scale);
    }

    void RenderPosts()
    {
        this.posts = new GameObject[(this.width + 1) * (this.height + 1)];
        for (int x = 0; x <= this.width; x++)
        {
            for (int y = 0; y <= this.height; y++)
            {
                Vector3 position = new Vector3(x * this.squareWidth, this.wallHeight / 2, y * this.squareWidth);
                Vector3 scale = new Vector3(this.wallWidth, this.wallHeight, this.wallWidth);
                posts[(this.width + 1) * y + x] = Instantiate(this.postPrefab, position, Quaternion.identity);
                posts[(this.width + 1) * y + x].transform.localScale = scale;
                RenderWallTexture(posts[(this.width + 1) * y + x].GetComponentInChildren<MeshRenderer>(), scale);
            }
        }
    }

    void RenderWalls()
    {
        this.horizontals = new GameObject[(this.height + 1) * this.width];
        this.verticals = new GameObject[(this.width + 1) * this.height];
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                if (y == 0 && this.data.GetBottomWall(x, y) != 0)
                {
                    Vector3 position = new Vector3((x + 0.5f) * this.squareWidth, this.wallHeight / 2, y * this.squareWidth);
                    Vector3 scale = new Vector3(this.cellSize, this.wallHeight, this.wallWidth);
                    this.horizontals[y * this.width + x] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.horizontals[y * this.width + x].transform.localScale = scale;
                    RenderWallTexture(this.horizontals[y * this.width + x].GetComponentInChildren<MeshRenderer>(), scale);
                }
                if (x == 0 && this.data.GetLeftWall(x, y) != 0)
                {
                    Vector3 position =  new Vector3(x * this.squareWidth, this.wallHeight / 2, (y + 0.5f) * this.squareWidth);
                    Vector3 scale = new Vector3(this.wallWidth, this.wallHeight, this.cellSize);
                    this.verticals[(this.width + 1) * y + x] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.verticals[(this.width + 1) * y + x].transform.localScale = scale;
                    RenderWallTexture(this.verticals[(this.width + 1) * y + x].GetComponentInChildren<MeshRenderer>(), scale);
                }
                if (this.data.GetTopWall(x, y) != 0)
                {
                    Vector3 position = new Vector3((x + 0.5f) * this.squareWidth, this.wallHeight / 2, (y + 1) * this.squareWidth);
                    Vector3 scale = new Vector3(this.cellSize, this.wallHeight, this.wallWidth);
                    this.horizontals[(y + 1) * this.width + x] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.horizontals[(y + 1) * this.width + x].transform.localScale = scale;
                    RenderWallTexture(this.horizontals[(y + 1) * this.width + x].GetComponentInChildren<MeshRenderer>(), scale);
                }
                if (this.data.GetRightWall(x, y) != 0)
                {
                    Vector3 position = new Vector3((x + 1) * this.squareWidth, this.wallHeight / 2, (y + 0.5f) * this.squareWidth);
                    Vector3 scale = new Vector3(this.wallWidth, this.wallHeight, this.cellSize);
                    this.verticals[(this.width + 1) * y + x + 1] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.verticals[(this.width + 1) * y + x + 1].transform.localScale = scale;
                    RenderWallTexture(this.verticals[(this.width + 1) * y + x + 1].GetComponentInChildren<MeshRenderer>(), scale);
                }
            }
        }
    }

    /// <summary>
    /// Texture rendering - allows an image to be used to texture the walls
    /// </summary>

    void RenderFloorTexture(Renderer renderer, Vector3 localScale)
    {
        if (renderer != null)
        {
            // Calculate the tiling scale based on the world space dimensions of the wall
            Vector2 tilingScale = new Vector2(localScale.x, localScale.z);

            // Update the tiling of the material
            renderer.material.mainTextureScale = tilingScale;
            Debug.Log(renderer.material.mainTextureScale);
        } else {
            Debug.Log("Renderer not found.");
        }
    }

    void RenderCeilingTexture(Renderer renderer, Vector3 localScale)
    {
        if (renderer != null)
        {
            // Calculate the tiling scale based on the world space dimensions of the wall
            Vector2 tilingScale = new Vector2(localScale.x, localScale.z);

            // Update the tiling of the material
            renderer.material.mainTextureScale = tilingScale;
            Debug.Log(renderer.material.mainTextureScale);
        } else {
            Debug.Log("Renderer not found.");
        }
    }

    void RenderWallTexture(Renderer renderer, Vector3 localScale)
    {
        if (renderer != null)
        {
            // Calculate the tiling scale based on the world space dimensions of the wall
            Vector2 tilingScale = new Vector2(Mathf.Max(localScale.x, localScale.z), localScale.y);

            // Update the tiling of the material
            renderer.material.mainTextureScale = tilingScale;
            Debug.Log(renderer.material.mainTextureScale);
        } else {
            Debug.Log("Renderer not found.");
        }
    }
}
