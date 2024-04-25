using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class MazeGenerationFunction
{   
    private int h = 0;
    private int w = 0;
    private int[,] cells;

    MazeData maze;
    
    public void Generate(MazeData maze, (int, int) start)
    {
        this.h = maze.GetHeight();
        this.w = maze.GetWidth();
        this.maze = maze;

        this.cells = new int[this.w, this.h];

        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push(start);
        this.cells[start.Item1, start.Item2] = 1;

        while (stack.Count > 0)
        {
            (int, int) cell = stack.Pop();
            Debug.Log("Examining cell " + cell);
            List<(int, int)> neighbors = GetUnusedNeighbors(cell);
            Debug.Log(neighbors.Count);
            if (neighbors.Count > 0)
            {
                stack.Push(cell);
                int index = Random.Range(0, neighbors.Count);
                (int, int) neighbor = neighbors[index];
                stack.Push(neighbor);
                this.cells[neighbor.Item1, neighbor.Item2] = 1;
                Debug.Log("Adding cell " + neighbor);
                RemoveWall(cell, neighbors[index]);
            }
        }
    }

    private void RemoveWall((int, int) p1, (int, int) p2)
    {

        if (p2.Item2 > p1.Item2)
        {
            Debug.Log("REMOVE WALL: TOP " + p1);
            this.maze.SetTopWall(p1, 0);
            return;
        }

        if (p2.Item2 < p1.Item2)
        {
            Debug.Log("REMOVE WALL: BOTTOM " + p1);
            this.maze.SetBottomWall(p1, 0);
            return;
        }

        if (p2.Item1 > p1.Item1)
        {
            Debug.Log("REMOVE WALL: RIGHT " + p1);
            this.maze.SetRightWall(p1, 0);
            return;
        }

        if (p2.Item1 < p1.Item1)
        {

            Debug.Log("REMOVE WALL: LEFT " + p1);
            this.maze.SetLeftWall(p1, 0);
            return;
        }

        Debug.LogError("REMOVE WALL: Points are the same somehow.");
    }

    public int Point2Index((int, int) point)
    {
        return point.Item2 * this.w + point.Item1;
    }

    public (int, int) Index2Point(int index)
    {
        return (index % this.w, index / this.h);
    }

    public List<(int, int)> GetOpenNeighbors((int, int) coords)
    {
        int x = coords.Item1;
        int y = coords.Item2;
        List<(int, int)> neighbors = new List<(int, int)>();
        (int, int) left = (x - 1, y);
        (int, int) right = (x + 1, y);
        (int, int) down = (x, y - 1);
        (int, int) up = (x, y + 1);

        if (this.maze.ValidCoordinate(left) && this.maze.GetLeftWall(coords) == 0) { neighbors.Add(left); }
        if (this.maze.ValidCoordinate(right) && this.maze.GetRightWall(coords) == 0) { neighbors.Add(right); }
        if (this.maze.ValidCoordinate(down) && this.maze.GetBottomWall(coords) == 0) { neighbors.Add(down); }
        if (this.maze.ValidCoordinate(up) && this.maze.GetTopWall(coords) == 0) { neighbors.Add(up); }

        Debug.Log("OPEN NEIGHBORS: neighbors for " + coords);
        foreach ((int, int) neighbor in neighbors)
        {
            Debug.Log("OPEN NEIGHBORS: " + neighbor);
        } 

        return neighbors;
    }

    private List<(int, int)> GetUnusedNeighbors((int, int) coords)
    {
        int x = coords.Item1;
        int y = coords.Item2;
        List<(int, int)> neighbors = new List<(int, int)>();
        (int, int) left = (x - 1, y);
        (int, int) right = (x + 1, y);
        (int, int) down = (x, y - 1);
        (int, int) up = (x, y + 1);
        if (ValidCell(left)) { neighbors.Add(left); }
        if (ValidCell(right)) { neighbors.Add(right); }
        if (ValidCell(down)) { neighbors.Add(down); }
        if (ValidCell(up)) { neighbors.Add(up); }

        return neighbors;
    }

    private bool ValidCell((int, int) coords)
    {
        return this.maze.ValidCoordinate(coords) && (this.cells[coords.Item1, coords.Item2] == 0);
    }

    public int[,] GetDistanceArray((int, int) goal)
    {
        int[,] distanceCells = new int[this.w, this.h];
        
        if (this.maze.ValidCoordinate(goal))
        {
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue(goal);
            distanceCells[goal.Item1, goal.Item2] = 1;

            while (queue.Count > 0)
            {
                (int, int) cell = queue.Dequeue();
                Debug.Log("DISTANCE ARRAY: checking " +  cell);
                int cellDistance = distanceCells[cell.Item1, cell.Item2];
                Debug.Log("DISTANCE ARRAY: cell distance " +  cellDistance);
                List<(int, int)> neighbors = GetOpenNeighbors(cell);
                foreach ((int, int) neighbor in neighbors)
                {
                    Debug.Log("DISTANCE ARRAY: checking neighbor " + neighbor);
                    int neighborDistance = distanceCells[neighbor.Item1, neighbor.Item2];
                    if (neighborDistance == 0 || neighborDistance > cellDistance)
                    {
                        Debug.Log("DISTANCE ARRAY: adding distance for " + neighbor);
                        Debug.Log("DISTANCE ARRAY: new distance " + (cellDistance + 1));
                        distanceCells[neighbor.Item1, neighbor.Item2] = cellDistance + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        } else {
            Debug.LogError("DISTANCE ARRAY: Bad goal " + goal);
        }
        
        return distanceCells;

    }
}