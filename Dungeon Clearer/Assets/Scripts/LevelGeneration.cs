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

    public GameObject player;

    private GameObject[] rooms;
    private List<Vector3> endPoints; // Y is direction, 0 is f, 1 is r, 2 is b, 3 is l
    private List<Vector3> placedPos;

    // Start is called before the first frame update
    void Start()
    {
        rooms = new GameObject[] { corridor, room1, room2, room3, room4, room5, room6 };
        endPoints = new List<Vector3>();
        placedPos = new List<Vector3>();

        Instantiate(corridor, new Vector3(0,0,0), Quaternion.identity);
        endPoints.Add(new Vector3(0,0,15));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<int> endPointsToRemove = new List<int>();

        for (int i = 0; i < endPoints.Count; i++)
        {
            if (isPlayerClose(endPoints[i]) && isPosNotUsed(endPoints[i]))
            {
                int room = 0;
                int direction = (int)endPoints[i].y;
                Vector3 newPos = endPoints[i];

                bool end = false;
                while (end == false)
                {
                    if (Random.Range(0, 2) == 1) { room = Random.Range(0, rooms.Length); }

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
                }

                
                if (room == 0 && (direction == 1 || direction == 3)) 
                {
                    Instantiate(rooms[room], new Vector3(newPos.x, 0, newPos.z), Quaternion.Euler(0, 90, 0)); 
                }
                else { Instantiate(rooms[room], new Vector3(newPos.x, 0, newPos.z), rooms[room].transform.rotation); Debug.Log("Room Num: " + room); } // Create new room

                placedPos.Add(new Vector3(newPos.x, 0, newPos.z));

                endPointsToRemove.Add(i);



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

        for (int i = 0; i < endPointsToRemove.Count; i++)
        {
            endPoints.RemoveAt(endPointsToRemove[i]);
        }

    }

    private bool isPlayerClose(Vector3 endPoint)
    {
        float distance = Vector3.Distance(endPoint, player.transform.position);
        if (distance < 20) { return true; }
        else { return false; }
    }

    private bool isPosNotUsed(Vector3 newPos)
    {
        for (int i = 0; i < placedPos.Count; i++)
        {
            if (placedPos[i].x == newPos.x && placedPos[i].z == newPos.z) { return false; }
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
