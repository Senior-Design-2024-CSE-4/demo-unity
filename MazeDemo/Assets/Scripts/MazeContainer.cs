using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeContainer : MonoBehaviour
{
    
    // Objects
    Plane floor;
    public GameObject postPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    private GameObject[] posts;
    private GameObject[] verticals;
    private GameObject[] horizontals;
    private GameObject player;

    // Dimensions
    public int width;
    public int height;

    float cellSize = 5.0f;
    float wallWidth = 1.0f;
    float wallHeight = 5.0f;

    // Data
    MazeData data;
    MazeGenerationFunction gen;

    
    // Start is called before the first frame update
    void Start()
    {
        this.floor = new Plane(Vector3.up, 0f);
        this.data = new MazeData(this.width, this.height);
        this.gen = new DFSGeneration();
        gen.Generate(this.data, 0, 0);
        Debug.Log("Maze generated.");
        RenderPosts();
        Debug.Log("Posts rendered.");
        RenderWalls();
        Debug.Log("Walls rendered.");
        SpawnPlayer(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RenderPosts()
    {
        this.posts = new GameObject[(this.width + 1) * (this.height + 1)];
        for (int x = 0; x <= this.width; x++)
        {
            for (int y = 0; y <= this.height; y++)
            {
                posts[(this.width + 1) * y + x] = Instantiate(this.postPrefab, new Vector3(x * (this.cellSize + this.wallWidth), this.wallHeight / 2, y * (this.cellSize + this.wallWidth)), Quaternion.identity);
                posts[(this.width + 1) * y + x].transform.localScale = new Vector3(this.wallWidth, this.wallHeight, this.wallWidth);
                Debug.Log("Initialized post " + x + " " + y + ".");
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
                if (x == 0 && this.data.GetBottomWall(x, y) != 0)
                {
                    this.verticals[y * this.width + x] = Instantiate(this.wallPrefab, new Vector3(x * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 0.5f) * (this.cellSize + this.wallWidth)), Quaternion.identity);
                    this.verticals[y * this.width + x].transform.localScale = new Vector3(this.wallWidth, this.wallHeight, this.wallHeight);
                }
                if (y == 0 && this.data.GetLeftWall(x, y) != 0)
                {
                    this.horizontals[(this.width + 1) * y + x] = Instantiate(this.wallPrefab, new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), this.wallHeight / 2, y * (this.cellSize + this.wallWidth)), Quaternion.identity);
                    this.horizontals[(this.width + 1) * y + x].transform.localScale = new Vector3(this.cellSize, this.wallHeight, this.wallWidth);
                }
                if (this.data.GetTopWall(x, y) != 0)
                {
                    this.verticals[(y + 1) * this.width + x] = Instantiate(this.wallPrefab, new Vector3((x + 1) * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 0.5f) * (this.cellSize + this.wallWidth)), Quaternion.identity);
                    this.verticals[(y + 1) * this.width + x].transform.localScale = new Vector3(this.wallWidth, this.wallHeight, this.wallHeight);
                }
                if (this.data.GetRightWall(x, y) != 0)
                {
                    this.horizontals[(this.width + 1) * y + x + 1] = Instantiate(this.wallPrefab, new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 1) * (this.cellSize + this.wallWidth)), Quaternion.identity);
                    this.horizontals[(this.width + 1) * y + x + 1].transform.localScale = new Vector3(this.cellSize, this.wallHeight, this.wallWidth);
                }
            }
        }
    }
    void SpawnPlayer(int x, int y)
    {
        this.player = Instantiate(this.playerPrefab, new Vector3((x + 0.5f) * (this.cellSize + this.wallWidth), this.wallHeight / 2, (y + 0.5f) * (this.cellSize + this.wallWidth)), Quaternion.identity);
    }
}
