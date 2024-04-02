/// MazeData.cs
/// Separates data from the container class.
/// This helps separate ideas to allow for easier modification
/// NOTE: Assumes (0, 0) is the BOTTOM LEFT square

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool ValidCoordinate(int x, int y)
    {
        return (x >= 0 && x < this.width) && (y >= 0 && y < this.height);
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
    public int? GetLeftWall(int x, int y)
    {
        if (ValidCoordinate(x, y))
        {
            return this.vertical[(this.width + 1) * y + x];
        }
        // return null if coordinate is not valid
        return null;
    }

    public void SetLeftWall(int x, int y, int val)
    {
        if (ValidCoordinate(x, y))
        {
            this.vertical[(this.width + 1) * y + x] = val;
        }
        Debug.Log("Cannot set coordinate (" + x + ", " + y + "); point is not valid.");
    }

    public int? GetRightWall(int x, int y)
    {
        if (ValidCoordinate(x, y))
        {
            return this.vertical[(this.width + 1) * y + x + 1];
        }
        // return null if coordinate is not valid
        return null;
    }

    public void SetRightWall(int x, int y, int val)
    {
        if (ValidCoordinate(x, y))
        {
            this.vertical[(this.width + 1) * y + x + 1] = val;
        }
        Debug.Log("Cannot set coordinate (" + x + ", " + y + "); point is not valid.");
    }

    public int? GetTopWall(int x, int y)
    {
        if (ValidCoordinate(x, y))
        {
            return this.horizontal[(y + 1) * this.width + x];
        }
        // return null if coordinate is not valid
        return null;
    }

    public void SetTopWall(int x, int y, int val)
    {
        if (ValidCoordinate(x, y))
        {
            this.horizontal[(y + 1) * this.width + x] = val;
        }
        Debug.Log("Cannot set coordinate (" + x + ", " + y + "); point is not valid.");
    }

    public int? GetBottomWall(int x, int y)
    {
        if (ValidCoordinate(x, y))
        {
            return this.horizontal[y * this.width + x];
        }
        // return null if coordinate is not valid
        return null;
    }

    public void SetBottomWall(int x, int y, int val)
    {
        if (ValidCoordinate(x, y))
        {
            this.horizontal[y * this.width + x] = val;
        }
        Debug.Log("Cannot set coordinate (" + x + ", " + y + "); point is not valid.");
    }
}
