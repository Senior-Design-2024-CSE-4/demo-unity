using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public abstract class MazeGenerationFunction
{   
    // Start is called before the first frame update
    public abstract void Generate(MazeData maze, int startX, int startY);
}

public class DFSGeneration: MazeGenerationFunction
{
    private int h = 0;
    private int w = 0;
    private int[] cells = new int[1];

    MazeData maze;
    
    public override void Generate(MazeData maze, int startX, int startY)
    {
        this.h = maze.GetHeight();
        this.w = maze.GetWidth();
        this.maze = maze;

        this.cells = new int[this.h * this.w];

        Stack<int> stack = new Stack<int>();
        stack.Push(Point2Index(startX, startY));

        while (stack.Count > 0)
        {
            int cell = stack.Pop();
            Debug.Log("Examining cell " + cell);
            List<int> neighbors = GetNeighbors(cell);
            if (neighbors.Count > 0)
            {
                stack.Push(cell);
                int index = Random.Range(0, neighbors.Count);
                stack.Push(neighbors[index]);
                RemoveWall(cell, neighbors[index]);
            }
        }
    }

    private void RemoveWall(int c1, int c2)
    {
        (int, int) p1 = Index2Point(c1);
        (int, int) p2 = Index2Point(c2);

        if (p2.Item2 > p1.Item2)
        {
            this.maze.SetTopWall(p1.Item1, p1.Item2, 0);
            return;
        }

        if (p2.Item2 < p1.Item2)
        {
            this.maze.SetBottomWall(p1.Item1, p1.Item2, 0);
            return;
        }

        if (p2.Item1 > p1.Item1)
        {
            this.maze.SetRightWall(p1.Item1, p1.Item2, 0);
            return;
        }

        if (p2.Item1 > p1.Item1)
        {
            this.maze.SetLeftWall(p1.Item1, p1.Item2, 0);
            return;
        }

        Debug.Log("Error! Points are the same, somehow.");
    }

    private int Point2Index(int x, int y)
    {
        return y * this.w + x;
    }

    private (int, int) Index2Point(int index)
    {
        return (index % this.w, index / this.h);
    }

    private List<int> GetNeighbors(int index)
    {
        (int, int) coords = Index2Point(index);
        int x = coords.Item1;
        int y = coords.Item2;
        List<int> neighbors = new List<int>();
        
        if (ValidCell(x, y + 1))
        {
            neighbors.Add(Point2Index(x, y + 1));
        }
        if (ValidCell(x + 1, y))
        {
            neighbors.Add(Point2Index(x + 1, y));
        }
        if (ValidCell(x, y - 1))
        {
            neighbors.Add(Point2Index(x, y - 1));
        }
        if (ValidCell(x - 1, y))
        {
            neighbors.Add(Point2Index(x - 1, y));
        }

        return neighbors;
    }

    private bool ValidCell(int x, int y)
    {
        return this.maze.ValidCoordinate(x, y) && this.cells[Point2Index(x, y)] == 0;
    }
}