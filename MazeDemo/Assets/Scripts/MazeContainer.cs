using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeContainer : MonoBehaviour
{
    
    // Objects
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject postPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab;
    GameObject floor;
    GameObject ceiling;
    private GameObject[] posts;
    private GameObject[] verticals;
    private GameObject[] horizontals;
    private GameObject player;
    private GameObject goal;

    // Dimensions
    public int width;
    public int height;

    float cellSize = 10.0f;
    float wallWidth = 1.0f;
    float wallHeight = 7.0f;

    // Data
    private MazeData data;
    private MazeGenerationFunction gen;
    
    // Start is called before the first frame update
    void Start()
    {
        RenderFloor();
        Debug.Log("Floor generated.");
        RenderCeiling();
        Debug.Log("Ceiling generated.");
        GenerateMaze();
        SpawnPlayer(0, 0);
        SpawnGoal(this.width - 1, this.height - 1);
        this.player.GetComponent<Player>().SetGoal(this.goal.transform);
        Vector3 distance = new Vector3((cellSize + wallWidth) * this.width + wallWidth, 0, (cellSize + wallWidth) * this.height + wallWidth);
        this.player.GetComponent<Player>().SetMaxDistance(distance.magnitude);
    }

    void ClearMaze()
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
    }

    public void RestartMaze()
    {
        ClearMaze();
        GenerateMaze();
        MovePlayer(0, 0);
    }

    void GenerateMaze() 
    {
        this.data = new MazeData(this.width, this.height);
        this.gen = new DFSGeneration();
        gen.Generate(this.data, 0, 0);
        Debug.Log("Maze generated.");
        RenderPosts();
        Debug.Log("Posts rendered.");
        RenderWalls();
        Debug.Log("Walls rendered.");
    }

    public void IncreaseLevel()
    {
        Debug.Log("Increasing level.");
        this.width += 1;
        this.height += 1;
        Destroy(this.floor);
        RenderFloor();
        Destroy(this.ceiling);
        RenderCeiling();
        MoveGoal(this.width - 1, this.height - 1);
        RestartMaze();
    }

    void RenderFloor()
    {
        Vector3 position = new Vector3(((cellSize + wallWidth) * this.width + wallWidth) / 2, -0.5f, ((cellSize + wallWidth) * this.height + wallWidth) / 2);
        Vector3 scale = new Vector3((cellSize + wallWidth) * this.width + wallWidth, 1.0f, (cellSize + wallWidth) * this.height + wallWidth);
        this.floor = Instantiate(this.floorPrefab, position, Quaternion.identity);
        this.floor.transform.localScale = scale;
        RenderFloorTexture(this.floor.GetComponentInChildren<MeshRenderer>(), scale);
    }

    void RenderCeiling()
    {
        Vector3 position = new Vector3(((cellSize + wallWidth) * this.width + wallWidth) / 2, this.wallHeight + 0.4f, ((cellSize + wallWidth) * this.height + wallWidth) / 2);
        Vector3 scale = new Vector3((cellSize + wallWidth) * this.width + wallWidth, 1.0f, (cellSize + wallWidth) * this.height + wallWidth);
        this.ceiling = Instantiate(this.ceilingPrefab, position, Quaternion.identity);
        this.ceiling.transform.localScale = scale;
        RenderCeilingTexture(this.ceiling.GetComponentInChildren<MeshRenderer>(), scale);
    }

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

    void RenderPosts()
    {
        this.posts = new GameObject[(this.width + 1) * (this.height + 1)];
        for (int x = 0; x <= this.width; x++)
        {
            for (int y = 0; y <= this.height; y++)
            {
                Vector3 position = new Vector3(x * (this.cellSize + this.wallWidth), this.wallHeight / 2, y * (this.cellSize + this.wallWidth));
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
                    Vector3 position = new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), this.wallHeight / 2, y * (this.cellSize + this.wallWidth));
                    Vector3 scale = new Vector3(this.cellSize, this.wallHeight, this.wallWidth);
                    this.horizontals[y * this.width + x] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.horizontals[y * this.width + x].transform.localScale = scale;
                    RenderWallTexture(this.horizontals[y * this.width + x].GetComponentInChildren<MeshRenderer>(), scale);
                }
                if (x == 0 && this.data.GetLeftWall(x, y) != 0)
                {
                    Vector3 position =  new Vector3(x * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 0.5f) * (this.cellSize + this.wallWidth));
                    Vector3 scale = new Vector3(this.wallWidth, this.wallHeight, this.cellSize);
                    this.verticals[(this.width + 1) * y + x] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.verticals[(this.width + 1) * y + x].transform.localScale = scale;
                    RenderWallTexture(this.verticals[(this.width + 1) * y + x].GetComponentInChildren<MeshRenderer>(), scale);
                }
                if (this.data.GetTopWall(x, y) != 0)
                {
                    Vector3 position = new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 1) * (this.cellSize + this.wallWidth));
                    Vector3 scale = new Vector3(this.cellSize, this.wallHeight, this.wallWidth);
                    this.horizontals[(y + 1) * this.width + x] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.horizontals[(y + 1) * this.width + x].transform.localScale = scale;
                    RenderWallTexture(this.horizontals[(y + 1) * this.width + x].GetComponentInChildren<MeshRenderer>(), scale);
                }
                if (this.data.GetRightWall(x, y) != 0)
                {
                    Vector3 position = new Vector3((x + 1) * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 0.5f) * (this.cellSize + this.wallWidth));
                    Vector3 scale = new Vector3(this.wallWidth, this.wallHeight, this.cellSize);
                    this.verticals[(this.width + 1) * y + x + 1] = Instantiate(this.wallPrefab, position, Quaternion.identity);
                    this.verticals[(this.width + 1) * y + x + 1].transform.localScale = scale;
                    RenderWallTexture(this.verticals[(this.width + 1) * y + x + 1].GetComponentInChildren<MeshRenderer>(), scale);
                }
            }
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

    void SpawnPlayer(int x, int y)
    {
        this.player = Instantiate(this.playerPrefab, new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), 1f, (y + 0.5f) * (this.cellSize + this.wallWidth)), Quaternion.identity);
    }

    void MovePlayer(int x, int y)
    {
        CharacterController cc = this.player.GetComponent<CharacterController>();
        Vector3 newPosition = new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), 1f, (y + 0.5f) * (this.cellSize + this.wallWidth));
        cc.enabled = false;
        this.player.transform.position = newPosition;
        this.player.transform.rotation = Quaternion.identity;
        cc.enabled = true;
        Debug.Log("Current position is " + transform.position);
        Debug.Log("Player moved to " + this.player.transform.position);
    }

    public void SetPlayerNavigationMode(int mode)
    {
        this.player.GetComponent<Player>().SetNavigationMode(mode);
    }
    void SpawnGoal(int x, int y)
    {
        this.goal = Instantiate(this.goalPrefab, new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), 2f, (y + 0.5f) * (this.cellSize + this.wallWidth)), Quaternion.identity);
    }
    void MoveGoal(int x, int y)
    {
        this.goal.transform.position = new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), 2f, (y + 0.5f) * (this.cellSize + this.wallWidth));
        this.goal.transform.rotation = Quaternion.identity;
    }
}
