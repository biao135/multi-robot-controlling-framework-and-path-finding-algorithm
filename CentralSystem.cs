using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CentralSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject intersection;
    [SerializeField]
    private GameObject left;
    [SerializeField]
    private GameObject right;
    [SerializeField]
    private GameObject up;
    [SerializeField]
    private GameObject down;
    [SerializeField]
    private GameObject pickup;
    [SerializeField]
    private GameObject dropoff;
    [SerializeField]
    private GameObject charging;
    [SerializeField]
    private GameObject notused;
    [SerializeField]
    private GameObject robot;
    [SerializeField]
    private GameObject throughputCounterObject;
    private TextMeshPro throughputCounter;
    // Start is called before the first frame update
    void Start()
    {
        throughputCounter = throughputCounterObject.GetComponent<TMPro.TextMeshPro>();
        // Create Vertices
        // 0 = intersection
        // 1 = left
        // 2 = right
        // 3 = up
        // 4 = down
        // 6 = pickup points
        // 7 = dropoff points
        // 8 = charging points
        // 9 = not used

        // create map
        int[,] graph = new int[,] {
            {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
            {9,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,9},
            {9,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,9},
            {9,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,9},
            {9,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,9},
            {9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9},
            {9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9},
            {9,3,4,9,3,4,9,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9},
            {9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,3,4,9,3,4,9},
            {9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9},
            {9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9},
            {9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9},
            {9,3,4,9,3,4,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,7,3,4,9,3,4,9,3,4,9},
            {9,3,4,9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9,3,4,9},
            {9,3,4,9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9,3,4,9},
            {9,3,4,9,3,4,9,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,6,3,4,9,3,4,9,3,4,9},
            {9,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,2,0,0,9},
            {9,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,9},
            {9,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,9},
            {9,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,9},
            {9,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,9},
            {9,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,9},
            {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9}
        };

        Server server = new Server(graph, throughputCounter);

        // create areas
        server.addArea(new Area(1, new int[]{49,1}, new int[]{31,44},
                                new List<int[]>{new int[] {40,5}, new int[] {43,2}, new int[] {40,41}, new int[] {43,44}}, //entrance
                                new List<int[]>{new int[] {40,4}, new int[] {43,1}, new int[] {40,40}, new int[] {43,43}}, //exit
                                server));

        server.addArea(new Area(2, new int[]{30,1}, new int[]{20,44}, 
                                new List<int[]>{new int[] {29,6}, new int[] {30,39}, new int[] {20,6}, new int[] {21,39}}, //entrance
                                new List<int[]>{new int[] {30,6}, new int[] {29,39}, new int[] {21,6}, new int[] {20,39}}, //exit
                                server));

        server.addArea(new Area(3, new int[]{19,1}, new int[]{1,44}, 
                                new List<int[]>{new int[] {7,1}, new int[] {10,4}, new int[] {10,40}, new int[] {7,43}}, //entrance
                                new List<int[]>{new int[] {7,2}, new int[] {10,5}, new int[] {10,41}, new int[] {7,44}}, //exit
                                server));

        server.addRingEntrances(new RingEntrances(1), new List<int[]>(){new int[]{43,1},new int[]{7,2}});
        server.addRingEntrances(new RingEntrances(1), new List<int[]>(){new int[]{40,4},new int[]{30,6},new int[]{21,6},new int[]{10,5}});
        server.addRingEntrances(new RingEntrances(1), new List<int[]>(){new int[]{40,40},new int[]{29,39},new int[]{20,39},new int[]{10,41}});
        server.addRingEntrances(new RingEntrances(1), new List<int[]>(){new int[]{43,43},new int[]{7,44}});

        int[] shape = server.getShape();

        //display map tile images
        for (int i = 0; i <= shape[0]; i++){
            for (int j = 0; j <= shape[1]; j++){
                switch(graph[i,j]) 
                {
                    case 0:
                        Instantiate(intersection, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(left, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(right, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(up, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 4:
                        Instantiate(down, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 6:
                        Instantiate(pickup, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 7:
                        Instantiate(dropoff, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 8:
                        Instantiate(charging, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    case 9:
                        Instantiate(notused, new Vector3(j, i, -10), Quaternion.identity);
                        break;
                    default:
                        break;
                }     
            }
        }

        int[,] spawnSpots = new int[,]{
            {1,1},{1,2},{1,3},{1,4},{1,5},{1,6},{1,7},{1,8},{1,9},{1,10},{1,11},{1,12},{1,13},{1,14},{1,15},{1,16},{1,17},{1,18},{1,19},{1,20},
            {1,21},{1,22},
            {1,23},{1,24},{1,25},{1,26},{1,27},{1,28},{1,29},{1,30},{1,31},{1,32},{1,33},{1,34},{1,35},{1,36},{1,37},{1,38},{1,39},{1,40},{1,41},{1,42},
            {1,43},{1,44},

            {2,1},{2,2},{2,3},{2,4},{2,5},{2,6},{2,7},{2,8},{2,9},{2,10},{2,11},{2,12},{2,13},{2,14},{2,15},{2,16},{2,17},{2,18},{2,19},{2,20},
            {2,21},{2,22},
            {2,23},{2,24},{2,25},{2,26},{2,27},{2,28},{2,29},{2,30},{2,31},{2,32},{2,33},{2,34},{2,35},{2,36},{2,37},{2,38},{2,39},{2,40},{2,41},{2,42},
            {2,43},{2,44},

            // {3,1},{3,2},{3,3},{3,4},{3,5},{3,6},{3,7},{3,8},{3,9},{3,10},{3,11},{3,12},{3,13},{3,14},{3,15},{3,16},{3,17},{3,18},{3,19},{3,20},
            // {3,21},{3,22},
            // {3,23},{3,24},{3,25},{3,26},{3,27},{3,28},{3,29},{3,30},{3,31},{3,32},{3,33},{3,34},{3,35},{3,36},{3,37},{3,38},{3,39},{3,40},{3,41},{3,42},
            // {3,43},{3,44},

            // {47,1},{47,2},{47,3},{47,4},{47,5},{47,6},{47,7},{47,8},{47,9},{47,10},{47,11},{47,12},{47,13},{47,14},{47,15},{47,16},{47,17},{47,18},{47,19},{47,20},
            // {47,21},{47,22},
            // {47,23},{47,24},{47,25},{47,26},{47,27},{47,28},{47,29},{47,30},{47,31},{47,32},{47,33},{47,34},{47,35},{47,36},{47,37},{47,38},{47,39},{47,40},{47,41},{47,42},
            // {47,43},{47,44},

            {48,1},{48,2},{48,3},{48,4},{48,5},{48,6},{48,7},{48,8},{48,9},{48,10},{48,11},{48,12},{48,13},{48,14},{48,15},{48,16},{48,17},{48,18},{48,19},{48,20},
            {48,21},{48,22},
            {48,23},{48,24},{48,25},{48,26},{48,27},{48,28},{48,29},{48,30},{48,31},{48,32},{48,33},{48,34},{48,35},{48,36},{48,37},{48,38},{48,39},{48,40},{48,41},{48,42},
            {48,43},{48,44},

            {49,1},{49,2},{49,3},{49,4},{49,5},{49,6},{49,7},{49,8},{49,9},{49,10},{49,11},{49,12},{49,13},{49,14},{49,15},{49,16},{49,17},{49,18},{49,19},{49,20},
            {49,21},{49,22},
            {49,23},{49,24},{49,25},{49,26},{49,27},{49,28},{49,29},{49,30},{49,31},{49,32},{49,33},{49,34},{49,35},{49,36},{49,37},{49,38},{49,39},{49,40},{49,41},{49,42},
            {49,43},{49,44},

        };

        for (int i = 0; i <= spawnSpots.GetUpperBound(0); i++){
            GameObject AMR = (GameObject) Instantiate(robot, new Vector3(spawnSpots[i,1], spawnSpots[i,0], -20), Quaternion.identity);
            AMR.GetComponent<RobotMovement>().Initialize(i, server, new int[] {spawnSpots[i,0], spawnSpots[i,1]});
        }
    }
    // void FixedUpdate(){}

}