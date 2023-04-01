using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public GameObject corridor;
    public GameObject room1; // f, b
    public GameObject room2; // f, b, l
    public GameObject room3; // f, b, r
    public GameObject room4; // b, l, r
    public GameObject room5; // f, l, r
    public GameObject room6; // l, r

    public GameObject wall;

    public GameObject player;

    private GameObject[] rooms;
    private List<Vector3> endPoints; // Y is direction, 0 is f, 1 is r, 2 is b, 3 is l
    private List<Vector3> placedPos;
    private List<int> placedPosDirection;
    

    // Start is called before the first frame update
    void Start()
    {
        rooms = new GameObject[] { corridor, room1, room2, room3, room4, room5, room6 };
        endPoints = new List<Vector3>();
        placedPos = new List<Vector3>();
        placedPosDirection = new List<int>();

        Instantiate(corridor, new Vector3(0, 0, 0), Quaternion.identity);
        endPoints.Add(new Vector3(0, 0, 15));
        placedPos.Add(new Vector3(0, 0, 0));
        placedPosDirection.Add(0);
        Instantiate(wall, new Vector3(0, 0, -15), Quaternion.Euler(0, 0, 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < endPoints.Count; i++)
        {
            int direction = (int)endPoints[i].y;

            if (isPlayerClose(endPoints[i]) && isPosNotUsed(endPoints[i], direction, i))
            {
                int room = 0;
                if (Random.Range(0, 101) <= 59) { room = 1; }

                Vector3 newPos = endPoints[i];

                bool end = false;
                bool loopCountCheck = false;
                int loopCounter = 0;
                while (end == false)
                {
                    if (room == 1 || loopCountCheck) 
                    {
                        if (Random.Range(0, 101) <= 70) { room = Random.Range(0, rooms.Length); }
                    }
                    

                    switch (direction) // Check if room is valid with direction
                    {
                        case 0: 
                            if (room == 0 || room == 1 || room == 2 || room == 3 || room == 4) { end = true; }
                            break;
                        case 1:
                            if (room == 0 || room == 2 || room == 4 || room == 5 || room == 6) { end = true; }
                            break;
                        case 2:
                            if (room == 0 || room == 1 || room == 2 || room == 3 || room == 5) { end = true; }
                            break;
                        case 3:
                            if (room == 0 || room == 3 || room == 4 || room == 5 || room == 6) { end = true; }
                            break;
                    }

                    if (loopCounter == 15) { room = 0; break; }

                    loopCountCheck = true;
                }

                
                if (room == 0 && (direction == 1 || direction == 3)) 
                {
                    Instantiate(rooms[room], new Vector3(newPos.x, 0, newPos.z), Quaternion.Euler(0, 90, 0)); 
                }
                else { Instantiate(rooms[room], new Vector3(newPos.x, 0, newPos.z), rooms[room].transform.rotation); Debug.Log("Room Num: " + room); } // Create new room

                placedPos.Add(new Vector3(newPos.x, room, newPos.z));
                placedPosDirection.Add(direction);

                endPoints[i] = new Vector3(endPoints[i].x, 30, endPoints[i].z);


                switch (room) // Create new endPoints
                {
                    case 0: // Corridor
                        
                        switch (direction)
                        {
                            case 0:
                                newPos.z += 15;
                                break;
                            case 1:
                                newPos.x += 15;
                                break;
                            case 2:
                                newPos.z -= 15;
                                break;
                            case 3:
                                newPos.x -= 15;
                                break;
                        }

                        endPoints.Add(newPos);
                        break;

                    case 1:
                        createEndPoints(true, true, false, false, direction, newPos);
                        break;

                    case 2:
                        createEndPoints(true, true, true, false, direction, newPos);
                        break;

                    case 3:
                        createEndPoints(true, true, false, true, direction, newPos);
                        break;

                    case 4:
                        createEndPoints(false, true, true, true, direction, newPos);
                        break;

                    case 5:
                        createEndPoints(true, false, true, true, direction, newPos);
                        break;

                    case 6:
                        createEndPoints(false, false, true, true, direction, newPos);
                        break;

                }
            }
        }

        for (int i = 0; i < endPoints.Count; i++)
        {
            if (endPoints[i].y == 30) { endPoints.RemoveAt(i); }
        }

    }

    private bool isPlayerClose(Vector3 endPoint)
    {
        float distance = Vector3.Distance(endPoint, player.transform.position);
        if (distance < 20) { return true; }
        else { return false; }
    }

    private bool isPosNotUsed(Vector3 newPos, int direction, int arrayPos)
    {
        for (int i = 0; i < placedPos.Count; i++)
        {
            if (placedPos[i].x == newPos.x && placedPos[i].z == newPos.z) 
            {
                if (placedPos[i].y == 0)
                {
                    bool createWall = true;

                    switch (placedPosDirection[i])
                    {
                        case 0:
                            if (direction == 2) { createWall = false; }
                            break;
                        case 1:
                            if (direction == 3) { createWall = false; }
                            break;
                        case 2:
                            if (direction == 0) { createWall = false; }
                            break;
                        case 3:
                            if (direction == 1) { createWall = false; }
                            break;
                    }

                    if (createWall)
                    {
                        float rotation = 0; // Create Wall
                        switch (direction)
                        {
                            case 0:
                                rotation = 180;
                                break;
                            case 1:
                                rotation = 270;
                                break;
                            case 2:
                                rotation = 0;
                                break;
                            case 3:
                                rotation = 90;
                                break;
                        }

                        Instantiate(wall, new Vector3(newPos.x, 0, newPos.z), Quaternion.Euler(0, rotation, 0));
                    }
                }
                
                endPoints[arrayPos] = new Vector3(endPoints[arrayPos].x, 30, endPoints[arrayPos].z);

                return false;
            }
        }

        return true;
    }

    private void createEndPoints(bool front, bool back, bool left, bool right, int direction, Vector3 pos)
    {
        if (front) { if (direction != 2) { endPoints.Add(new Vector3(pos.x, 0, pos.z + 15)); } }
        if (back) { if (direction != 0) { endPoints.Add(new Vector3(pos.x, 2, pos.z - 15)); } }
        if (left) { if (direction != 1) { endPoints.Add(new Vector3(pos.x - 15, 3, pos.z)); } }
        if (right) { if (direction != 3) { endPoints.Add(new Vector3(pos.x + 15, 1, pos.z)); } }

    }

}
