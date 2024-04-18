using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Found goal.");
            GameObject maze = GameObject.Find("Maze");
            if (maze != null)
            {
                maze.GetComponent<MazeContainer>().IncreaseLevel();
            }
        }
    }
}
