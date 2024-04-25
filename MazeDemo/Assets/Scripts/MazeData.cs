/// MazeData.cs
/// Separates data from the container class.
/// This helps separate ideas to allow for easier modification
/// NOTE: Assumes (0, 0) is the BOTTOM LEFT square

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class MazeData
{
    // Dimension components
    private readonly int height;
    private readonly int width;

    // Data components - stored as horizontal and vertical walls
    int[] horizontal;
    int[] vertical;

    // Initialization
    public MazeData(int height, int width)
    {
        this.height = height;
        this.width = width;
        this.horizontal = new int[(height + 1) * width];
        this.vertical = new int[(width + 1) * height];
        Initialize();
    }

    // Set everything to 1
    private void Initialize()
    {
        for (int i = 0; i < this.horizontal.Length; i++)
        {
            this.horizontal[i] = 1;
        }
        for (int i = 0; i < this.vertical.Length; i++)
        {
            this.vertical[i] = 1;
        }
    }

    // Makes sure a given coordinate is a point within this maze.
    public bool ValidCoordinate((int, int) coords)
    {
        bool valid_x = coords.Item1 >= 0 && coords.Item1 < this.width;
        bool valid_y = coords.Item2 >= 0 && coords.Item2 < this.height;
        return valid_x && valid_y;
    }

    // Size getters/setters
    public int GetHeight()
    {
        return this.height;
    }

    public int GetWidth()
    {
        return this.width;
    }

    // Wall getters/setters
    public int? GetLeftWall((int, int) coords)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            return this.vertical[(this.width + 1) * y + x];
        }
        // return null if coordinate is not valid
        Debug.LogError("Attempting to get invalid coordinate " + coords);
        return null;
    }

    public void SetLeftWall((int, int) coords, int val)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            this.vertical[(this.width + 1) * y + x] = val;
        } else {
            Debug.LogError("Attempting to set invalid coordinate " + coords);
        }
    }

    public int? GetRightWall((int, int) coords)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            return this.vertical[(this.width + 1) * y + x + 1];
        }
        // return null if coordinate is not valid
        Debug.LogError("Attempting to get invalid coordinate " + coords);
        return null;
    }

    public void SetRightWall((int, int) coords, int val)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            this.vertical[(this.width + 1) * y + x + 1] = val;
        } else {
            Debug.LogError("Attempting to set invalid coordinate " + coords);
        }
    }

    public int? GetTopWall((int, int) coords)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            return this.horizontal[(y + 1) * this.width + x];
        }
        // return null if coordinate is not valid
        Debug.LogError("Attempting to get invalid coordinate " + coords);
        return null;
    }

    public void SetTopWall((int, int) coords, int val)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            this.horizontal[(y + 1) * this.width + x] = val;
        } else {
            Debug.LogError("Attempting to set invalid coordinate " + coords);
        }
    }

    public int? GetBottomWall((int, int) coords)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            return this.horizontal[y * this.width + x];
        }
        // return null if coordinate is not valid
        Debug.LogError("Attempting to get invalid coordinate " + coords);
        return null;
    }

    public void SetBottomWall((int, int) coords, int val)
    {
        if (ValidCoordinate(coords))
        {
            int x = coords.Item1;
            int y = coords.Item2;
            this.horizontal[y * this.width + x] = val;
        } else {
            Debug.LogError("Attempting to set invalid coordinate " + coords);
        }
    }

    public (int, int) RandomPoint()
    {
        int x = Random.Range(0, this.width);
        int y = Random.Range(0, this.height);
        return (x, y);
    }
}
