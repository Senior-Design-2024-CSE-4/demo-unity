using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeContainer : MonoBehaviour
{
    // Objects
    Plane floor;

    // Dimensions
    double width;
    double height;

    // Data
    MazeData data;
    MazeGenerationFunction gen;

    
    // Start is called before the first frame update
    void Start()
    {
        this.floor = new Plane(Vector3.up, 0f);
        this.data = new MazeData(10, 10);
        this.gen = new DFSGeneration();
        gen.Generate(this.data, 0, 0);
        Debug.Log("Maze generated.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RenderMaze()
    {

    }
}
