﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkRooms : MonoBehaviour
{

    public int nonDigZone = 5;

    public int width;
    public int height;

    int[,] map;

    public int changeDirectionChance = 5;
    public int addRoomChance = 5;
    [Range(0.1f, 0.9f)]
    public float targetPercentage = 0.5f;

    [Range(0.01f, 1)]
    public float waitingTime = 0.5f;

    enum Directions
    {
        N,S,E,W
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(GenerateMap());
        //GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GenerateMap()
    {
        map = new int[width, height];
        //fill map with all walls (1s)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                map[x, y] = 1;
            }
        }
        //creates a path by randomly moving through the level
        //the agent starts at a random position
        int agentX = Random.Range(0,width);
        int agentY = Random.Range(0,height);
        
        //let's "make a hole" in the specified position
        map[agentX, agentY] = 0;
        //while we haven't found the exit (always in the top right corner)
        while (PercentageDug() < targetPercentage)
        {

            Directions direction = (Directions)Random.Range(0, 4);
            //move agent in the current direction
            switch (direction)
            {
                case Directions.N:
                    if (agentY < height - 1)
                        agentY++;
                    break;
                case Directions.S:
                    if (agentY > 0)
                        agentY--;
                    break;
                case Directions.E:
                    if (agentX < width - 1)
                        agentX++;
                    break;
                case Directions.W:
                    if (agentX > 0)
                        agentX--;
                    break;
            }
            //dig
            map[agentX, agentY] = 0;

            bool[,] zoneWhite = new bool[nonDigZone,nonDigZone];
            bool dig; 

            for (int i = 0; i < nonDigZone; i++)
            {
                for (int j = 0; j < nonDigZone; j++)
                {
                    if (map[(agentX - nonDigZone / 2) + i, (agentY - nonDigZone / 2) + j] == 0) { 

                        zoneWhite[i,j] = true;

                    }
                   
                }
            }

            for (int i = 0; i < nonDigZone; i++)
            {
                for (int j = 0; j < nonDigZone; j++)
                {
                    if (map[(agentX - nonDigZone / 2) + i, (agentY - nonDigZone / 2) + j] == 0)
                    {
                        if (zoneWhite[i, j] == false) {
                            dig = true; 
                        }
                    }

                }
            }




            //Room maker



            int chanceToMakeRoom = Random.Range(0, 100);

            if (addRoomChance > chanceToMakeRoom)
            {
                int roomHeight = Random.Range(3, 8);
                int roomWidth = Random.Range(3, 8);

                    for (int i = 0; i < roomWidth; i++)
                    {
                        for (int j = 0; j < roomHeight; j++)
                        {

                        try
                        {
                            map[(agentX - roomWidth / 2) + i, (agentY - roomHeight / 2) + j] = 0;
                        }
                        catch { }
                        }
                    }

                    addRoomChance = 0;
            

            }
            else
            {
                addRoomChance += 5;
            }



            yield return new WaitForSeconds(waitingTime);
        }



    }

    //checks how much of the dungeon has been dug and returns a percentage in range [0,1] (e.g. 0.5 = 50%)
    float PercentageDug()
    {
        float count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                count += map[x, y];
            }
        }
        return (width * height - count) / (width * height);
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}

