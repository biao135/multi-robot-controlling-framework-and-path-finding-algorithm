using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;

public class Server : Graph
{
    private int[,] map;
    private int[,] deadlockMap;
    private List<Robot> robots;
    private int throughput;
    private TextMeshPro throughputCounter;
    public Server(int[,] graph, TextMeshPro throughputCounter) : base(graph){
        int [] mapShape = getShape();
        map = new int[mapShape[0], mapShape[1]];
        deadlockMap = new int[mapShape[0], mapShape[1]];
        this.throughputCounter = throughputCounter;
        robots = new List<Robot>();
    }

    public Tuple<PickUpPoint, DropOffPoint> getTask(){
        System.Random rdm = new System.Random();
        List<PickUpPoint> pickUpPoints = getPickUpPoints();
        PickUpPoint pickUpPoint = pickUpPoints[rdm.Next(pickUpPoints.Count)];
        List<DropOffPoint> dropOffPoints = getDropOffPoints();
        DropOffPoint dropOffPoint = dropOffPoints[rdm.Next(dropOffPoints.Count)];
        return Tuple.Create(pickUpPoint, dropOffPoint);
    }

    public void Register(int[] position, Robot robot){
        map[position[0], position[1]] = 1;
        robots.Add(robot);
    }
    
    public void ReleaseGrid(int[] position){
        map[position[0], position[1]] = 0;
    }

    public bool ReserveGrid(int[] currentPosition, int[] position, Robot robot, int direction){
        if(map[position[0], position[1]] == 0){
            map[position[0], position[1]] = 1;
            deadlockMap[currentPosition[0], currentPosition[1]] = 0;
            return true;
        }

        // detect circular wait deadlock if deadlock happens
        deadlockMap[currentPosition[0], currentPosition[1]] = direction;
        checkAndBreakCircularWait(currentPosition);

        return false;
    }

    public void checkAndBreakCircularWait(int [] currentPosition){
        int currentDirection;
        int deadlockStartPosition;
        int[] nextPosition = currentPosition;
        List<int[]> deadlockedPositions = new List<int[]>();
        
        // to detect circles
        while(true){
            // to detect whether deadlock exists and where the circular wait deadlock starts and ends
            deadlockStartPosition = deadlockedPositions.FindIndex(p => p.SequenceEqual(nextPosition));
            if(deadlockStartPosition >= 0){
                break;
            }
            currentDirection = deadlockMap[nextPosition[0], nextPosition[1]];
            deadlockedPositions.Add(nextPosition);
            if (currentDirection == 1){
                nextPosition = new int [] {nextPosition[0], nextPosition[1] - 1};
            }
            else if (currentDirection == 2){
                nextPosition = new int [] {nextPosition[0], nextPosition[1] + 1};
            }
            else if (currentDirection == 3){
                nextPosition = new int [] {nextPosition[0] - 1, nextPosition[1]};
            }
            else if (currentDirection == 4){
                nextPosition = new int [] {nextPosition[0] + 1, nextPosition[1]};
            }
            else{
                return;
            }
        }

        // Allow all robots in the circular wait deadlock to move
        for(int i = deadlockStartPosition; i < deadlockedPositions.Count; i++){
            for(int j = 0; j < robots.Count; j++){
                if(robots[j].getPosition().SequenceEqual(deadlockedPositions[i])){
                    robots[j].setApprove(true);
                    deadlockMap[deadlockedPositions[i][0], deadlockedPositions[i][1]] = 0;
                    break;
                }
            }
        }
    }

    public bool ReserveGridsForMovingDiagonally(List<int[]> positions){
        // check if the extra grid is a movable grid
        int[] lastPosition = positions.Last();
        if(!Array.Exists(new int [] {0,1,2,3,4,8}, element => element == getGraph()[lastPosition[0], lastPosition[1]])){
            return false;
        }

        // check if all grids are empty
        for(int i = 0; i < positions.Count; i++){
            if(map[positions[i][0], positions[i][1]] == 1){
                return false;
            }
        }

        for(int i = 0; i < positions.Count; i++){
            map[positions[i][0], positions[i][1]] = 1;
        }

        return true;
    }

    public void addCostToEntrance(Vertex vertex, int cost){
        vertex.addDynamicCost(cost);
    }

    public void addThroughput(){
        throughput += 1;
        throughputCounter.text = throughput.ToString();
    }

    public void addRingEntrances(RingEntrances ringEntrances, List<int[]> positions){
        for(int i = 0; i < positions.Count; i++){
            findVertex(positions[i]).setRingEntrances(ringEntrances);
        }
    }
}
