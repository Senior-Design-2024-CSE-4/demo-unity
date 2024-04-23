using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class MazeGenerationFunction
{   
    private int h = 0;
    private int w = 0;
    private int[] cells = new int[1];

    MazeData maze;
    
    public void Generate(MazeData maze, int startX, int startY)
    {
        this.h = maze.GetHeight();
        this.w = maze.GetWidth();
        this.maze = maze;

        this.cells = new int[this.h * this.w];

        Stack<int> stack = new Stack<int>();
        stack.Push(Point2Index(startX, startY));
        this.cells[Point2Index(startX, startY)] = 1;

        while (stack.Count > 0)
        {
            int cell = stack.Pop();
            Debug.Log("Examining cell " + cell);
            List<int> neighbors = GetUnusedNeighbors(cell);
            Debug.Log(neighbors.Count);
            if (neighbors.Count > 0)
            {
                stack.Push(cell);
                int index = Random.Range(0, neighbors.Count);
                stack.Push(neighbors[index]);
                this.cells[neighbors[index]] = 1;
                Debug.Log("Adding cell " + neighbors[index]);
                RemoveWall(cell, neighbors[index]);
            }
        }
    }

    private void RemoveWall(int c1, int c2)
    {
        (int, int) p1 = Index2Point(c1);
        Debug.Log(p1);
        (int, int) p2 = Index2Point(c2);
        Debug.Log(p2);

        if (p2.Item2 > p1.Item2)
        {
            Debug.Log("TOP " + p1.Item1 + " " + p1.Item2);
            this.maze.SetTopWall(p1.Item1, p1.Item2, 0);
            return;
        }

        if (p2.Item2 < p1.Item2)
        {
            Debug.Log("BOTTOM " + p1.Item1 + " " + p1.Item2);
            this.maze.SetBottomWall(p1.Item1, p1.Item2, 0);
            return;
        }

        if (p2.Item1 > p1.Item1)
        {
            Debug.Log("RIGHT " + p1.Item1 + " " + p1.Item2);
            this.maze.SetRightWall(p1.Item1, p1.Item2, 0);
            return;
        }

        if (p2.Item1 < p1.Item1)
        {

            Debug.Log("LEFT " + p1.Item1 + " " + p1.Item2);
            this.maze.SetLeftWall(p1.Item1, p1.Item2, 0);
            return;
        }

        Debug.Log("Error! Points are the same, somehow.");
    }

    public int Point2Index(int x, int y)
    {
        return y * this.w + x;
    }

    public (int, int) Index2Point(int index)
    {
        return (index % this.w, index / this.h);
    }

    public List<int> GetOpenNeighbors(int index)
    {
        (int, int) coords = Index2Point(index);
        int x = coords.Item1;
        int y = coords.Item2;
        List<int> neighbors = new List<int>();
        
        if (this.maze.ValidCoordinate(x, y + 1) && this.maze.GetTopWall(x, y) == 0)
        {
            neighbors.Add(Point2Index(x, y + 1));
        }
        if (this.maze.ValidCoordinate(x + 1, y) && this.maze.GetRightWall(x, y) == 0)
        {
            neighbors.Add(Point2Index(x + 1, y));
        }
        if (this.maze.ValidCoordinate(x, y - 1) && this.maze.GetBottomWall(x, y) == 0)
        {
            neighbors.Add(Point2Index(x, y - 1));
        }
        if (this.maze.ValidCoordinate(x - 1, y) && this.maze.GetLeftWall(x, y) == 0)
        {
            neighbors.Add(Point2Index(x - 1, y));
        }

        return neighbors;
    }

    private List<int> GetUnusedNeighbors(int index)
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
        return this.maze.ValidCoordinate(x, y) && (this.cells[Point2Index(x, y)] == 0);
    }

    public int[] GetDistanceArray(int x, int y)
    {
        int[] cells = new int[this.h * this.w];
        
        if (this.maze.ValidCoordinate(x, y))
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(Point2Index(x, y));
            cells[Point2Index(x, y)] = 1;

            while (queue.Count > 0)
            {
                int cell = queue.Dequeue();
                List<int> neighbors = GetOpenNeighbors(cell);
                foreach (int neighbor in neighbors)
                {
                    if (cells[neighbor] == 0 || cells[neighbor] > cells[cell])
                    {
                        cells[neighbor] = cells[cell] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        } else {
            Debug.LogError("Attempting to generate distance array from invalid point (" + x + "," + y + ").");
        }

        foreach (int cell in cells)
        {
            Debug.Log("CELL: " + cell);
        }
        
        return cells;

    }
}