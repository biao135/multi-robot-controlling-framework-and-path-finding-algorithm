using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

public class RobotMovement : MonoBehaviour
{
    private Robot robot;

    [SerializeField]
    private GameObject parcel;
    private bool displayParcel = false;
    private Rigidbody2D rb;

    [SerializeField]
    private GameObject arrow;

    private int label;
    private Server server;

    private int[] position;
    private int[] gridToRelease;
    private List<int[]> diagonalGridsToRelease = new List<int[]>();
    private List<Vertex> pickUpPoint;
    private List<Vertex> dropOffPoint;
    private List<Vertex> route;
    private Vector2 endPosition;
    private bool moving = false;

    private List<Vertex> exits;
    private List<Vertex> entrances;

    public void Initialize(int _label, Server _server, int[] _position){
        server = _server;
        position = _position;
        endPosition = new Vector2(position[1], position[0]);
        gridToRelease = position;
        route = new List<Vertex>();
        robot = new Robot(_label, _position);
        server.Register(position, robot);
    }

    private int getDirection(int[] position1, int[] position2){
        if (position1[0] < position2[0]){
            return 4;
        }
        else if (position1[0] > position2[0]){
            return 3;
        }
        else if (position1[1] < position2[1]){
            return 2;
        }
        else if (position1[1] > position2[1]){
            return 1;
        }
        return 0;
    }

    private List<int[]> getGridsToMoveDiagonally(int[] position1, int[] position2, int[] position3){
        int vertical = position3[0] - position2[0];
        int horizontal = position3[1] - position2[1];
        int[] position4 = new int[]{position1[0] + vertical, position1[1] + horizontal};
        return new List<int[]>(){position2, position3, position4};
    }

    private Tuple<List<Vertex>, int, List<Vertex>> findRoute(Vertex vertexStart, List<Vertex> destinations){
        Area currentArea = server.getArea(robot.getPosition());
        List<List<Vertex>> newDestinations = new List<List<Vertex>>();
        List<List<Vertex>> tempExits = new List<List<Vertex>>();
        List<List<Vertex>> tempEntrances = new List<List<Vertex>>();
        exits = new List<Vertex>();
        entrances = new List<Vertex>();

        // add entrance and exit points
        for(int i = 0; i < destinations.Count; i++){
            Area nextArea = destinations[i].getArea();

            if(currentArea != nextArea){
                newDestinations.Add(currentArea.getExitPoints());
                tempExits.Add(currentArea.getExitPoints());
                newDestinations.Add(nextArea.getEntrancePoints());
                tempEntrances.Add(nextArea.getEntrancePoints());
            }
            if(destinations[i] is PickUpPoint){
                newDestinations.Add((destinations[i] as PickUpPoint).getPickUpGrids());
            }
            else if(destinations[i] is DropOffPoint){
                newDestinations.Add((destinations[i] as DropOffPoint).getDropOffGrids());
            }
            else{
                newDestinations.Add(new List<Vertex>(){destinations[i]});
            }
            currentArea = nextArea;
        }

        Tuple<List<Vertex>, int, List<Vertex>> routeFound = server.findRouteWithMultipleDestinations(vertexStart, newDestinations);

        List<Vertex> routes = routeFound.Item1;
        for(int i = 0; i < tempExits.Count; i++){
            for(int j = 0; j < tempExits[i].Count; j++){
                int index = routes.FindIndex(a => a == tempExits[i][j]);
                if(index >= 0){
                    exits.Add(routes[index]);
                    server.addCostToEntrance(exits.Last(), 10);
                }
                index = routes.FindIndex(a => a == tempEntrances[i][j]);
                if(index >= 0){
                    entrances.Add(routes[index]);
                }
            }
        }

        return routeFound;
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        // finished the whole route, get new route
        if (route.Count == 0){
            Vertex startPoint = server.findVertex(position);

            // Get pick up point and drop off point
            Tuple<PickUpPoint, DropOffPoint> points = server.getTask();
            pickUpPoint = points.Item1.getPickUpGrids();
            dropOffPoint = points.Item2.getDropOffGrids();

            // Find route
            Tuple<List<Vertex>, int, List<Vertex>> routeFound = findRoute(startPoint, new List<Vertex>(){points.Item1 as Vertex, points.Item2 as Vertex});
            route = routeFound.Item1;
        }

        else{
            // get the next position to move
            if(!moving){
                Vertex curVertex = route[0];
                int[] currentVertexLabel = curVertex.getLabel();
                
                // to move diagonally
                if(route.Count >= 2){
                    Vertex nextVertex = route[1];
                    int[] nextVertexLabel = nextVertex.getLabel();
                    if(getDirection(position, currentVertexLabel) != getDirection(currentVertexLabel, nextVertexLabel)){
                        List<int[]> gridsToMoveDiagonally = getGridsToMoveDiagonally(position, currentVertexLabel, nextVertexLabel);
                        if(server.ReserveGridsForMovingDiagonally(gridsToMoveDiagonally)){
                            if(exits.Count > 0 && entrances.Count > 0 && (curVertex == entrances.First() || nextVertex == entrances.First())){
                                server.addCostToEntrance(exits.First(), -10);
                                exits.RemoveAt(0);
                                entrances.RemoveAt(0);
                            }
                            route.RemoveAt(0);
                            route.RemoveAt(0);
                            gridsToMoveDiagonally.Add(position);
                            gridsToMoveDiagonally.RemoveAt(gridsToMoveDiagonally.IndexOf(nextVertexLabel));
                            diagonalGridsToRelease = gridsToMoveDiagonally;
                            position = nextVertexLabel;
                            endPosition = new Vector2(nextVertexLabel[1], nextVertexLabel[0]);
                            moving = true;

                            // display parcel image
                            if(pickUpPoint.Contains(nextVertex)){
                                displayParcel = true;
                            }
                            else if(dropOffPoint.Contains(nextVertex)){
                                server.addThroughput();
                                displayParcel = false;
                            }
                            return;
                        }
                    }
                }

                // normal navigation
                int direction = getDirection(position, currentVertexLabel);
                if(robot.getApprove() || server.ReserveGrid(position, currentVertexLabel, robot, direction)){
                    if(exits.Count > 0 && entrances.Count > 0 && curVertex == entrances.First()){
                        server.addCostToEntrance(exits.First(), -10);
                        exits.RemoveAt(0);
                        entrances.RemoveAt(0);
                    }
                    route.RemoveAt(0);
                    position = currentVertexLabel;
                    endPosition = new Vector2(position[1], position[0]);
                    moving = true;

                    // display parcel image
                    if(pickUpPoint.Contains(curVertex)){
                        displayParcel = true;
                    }
                    else if(dropOffPoint.Contains(curVertex)){
                        server.addThroughput();
                        displayParcel = false;
                    }
                }
            }
            
            // reached the destination
            else if(rb.position == endPosition) {
                parcel.SetActive(displayParcel);
                moving = false;
                
                // release extra grids if the robot moved diagonally
                if(diagonalGridsToRelease.Count != 0){
                    int numberOfGrids = diagonalGridsToRelease.Count;
                    for (int i = 0; i < numberOfGrids; i++){
                        server.ReleaseGrid(diagonalGridsToRelease[0]);
                        diagonalGridsToRelease.RemoveAt(0);
                    }
                }

                // no need to release grid if want to break circular wait
                else if(!robot.getApprove()){
                    server.ReleaseGrid(gridToRelease);
                }

                gridToRelease = position;
                robot.setPosition(position);
                robot.setApprove(false);
            }

            // move animation
            Vector2 newPosition = Vector2.MoveTowards(rb.position, endPosition, 5 * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }
}