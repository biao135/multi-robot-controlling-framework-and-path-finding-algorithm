using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Area
{
    private int label;
    private int[] topLeftPoint;
    private int[] bottomRightPoint; 
    private List<Vertex> entrancePoints;
    private List<Vertex> exitPoints;
    private List<Tuple<Vertex, Vertex, List<Vertex>, int>> precalculatedRoutes;
    public Area(int label, int[] topLeftPoint, int[] bottomRightPoint, List<int[]> entrancePoints, List<int[]> exitPoints, Server server){
        this.label = label;
        this.topLeftPoint = topLeftPoint;
        this.bottomRightPoint = bottomRightPoint;
        this.entrancePoints = new List<Vertex>();
        for(int i = 0; i < entrancePoints.Count; i++){
            this.entrancePoints.Add(server.findVertex(entrancePoints[i]));
        }
        this.exitPoints = new List<Vertex>();
        for(int i = 0; i < exitPoints.Count; i++){
            this.exitPoints.Add(server.findVertex(exitPoints[i]));
        }
        precalculatedRoutes = new List<Tuple<Vertex, Vertex, List<Vertex>, int>>();
    }
    public int getLabel(){
        return label;
    }
    public int[] getTopLeftPoint(){
        return topLeftPoint;
    }
    public int[] getBottomRightPoint(){
        return bottomRightPoint;
    }
    public List<Vertex> getEntrancePoints(){
        return entrancePoints;
    }
    public List<Vertex> getExitPoints(){
        return exitPoints;
    }
    public void addPrecalculatedRoutes(Vertex exit, Vertex entrance, List<Vertex> route, int cost){
        precalculatedRoutes.Add(Tuple.Create(exit, entrance, route, cost));
    }
    public Tuple<List<Vertex>, int> getPrecalculatedRoutes(Vertex exit, List<Vertex> entrance){
        for (int i = 0; i < precalculatedRoutes.Count; i++){
            if(exit == precalculatedRoutes[i].Item1){
                for(int j = 0; j < entrance.Count; j++){
                    if(entrance[j] == precalculatedRoutes[i].Item2){
                        return Tuple.Create(precalculatedRoutes[i].Item3, precalculatedRoutes[i].Item4);
                    }
                }
            }
        }
        return null;
    }
}

public class RingEntrances
{
    private int label;
    private int dynamicCost;
    public RingEntrances(int label){
        this.label = label;
        dynamicCost = 0;
    }
    public void addDynamicCost(int cost){
        dynamicCost += cost;
    }
    public int getDynamicCost(){
        return dynamicCost;
    }
}

public class Vertex
{
    private int[] label;
    private int type;
    private Area area;
    private RingEntrances ringEntrances;
    public Vertex(int[] label, int type){
        this.label = label;
        this.type = type;
        ringEntrances = new RingEntrances(0);
    }
    public int[] getLabel(){
        return label;
    }
    public int getType(){
        return type;
    }
    public void setArea(Area area){
        this.area = area;
    }
    public Area getArea(){
        return area;
    }
    public void setRingEntrances(RingEntrances ringEntrances){
        this.ringEntrances = ringEntrances;
    }
    public void addDynamicCost(int cost){
        ringEntrances.addDynamicCost(cost);
    }
    public int getDynamicCost(){
        return ringEntrances.getDynamicCost();
    }
}

public class Edge
{
    private Vertex nextVertex;
    private int cost;
    private int direction;
    public Edge(Vertex nextVertex, int cost, int direction){
        this.nextVertex = nextVertex;
        this.cost = cost;
        this.direction = direction;
    }
    public Vertex getNextVertex(){
        return nextVertex;
    }
    public int getCost(){
        if(nextVertex.getDynamicCost() > 0){
        }
        return cost + nextVertex.getDynamicCost();
    }
    public int getDirection(){
        return direction;
    }
}

public class MoveGrid : Vertex
{
    private List<Edge> edge = new List<Edge>();
    public MoveGrid(int[] label, int type) : base(label, type){
    }
    public void addEdge(Vertex nextVertex, int cost, int direction){
        Edge edge = new Edge(nextVertex, cost, direction);
        this.edge.Add(edge);
    }
    public List<Edge> getEdge(){
        List<Edge> newEdge = new List<Edge>();
        for(int i = 0; i < edge.Count; i++){
            newEdge.Add(edge[i]);
        }
        return newEdge;
    }
}

public class PickUpPoint : Vertex
{
    private List<Vertex> pickUpGrids = new List<Vertex>();
    public PickUpPoint(int[] label, int type) : base(label, type){
    }
    public void addPickUpGrids(Vertex vertex){
        this.pickUpGrids.Add(vertex);
    }
    public List<Vertex> getPickUpGrids(){
        return pickUpGrids;
    }
}

public class DropOffPoint : Vertex
{
    private List<Vertex> dropOffPoint = new List<Vertex>();
    public DropOffPoint(int[] label, int type) : base(label, type){
    }
    public void addDropOffGrids(Vertex vertex){
        this.dropOffPoint.Add(vertex);
    }
    public List<Vertex> getDropOffGrids(){
        return dropOffPoint;
    }
}

public class ChargingPoint : Vertex
{
    private List<Edge> edge = new List<Edge>();
    public ChargingPoint(int[] label, int type) : base(label, type){
    }
    public void addEdge(Vertex nextVertex, int cost, int direction){
        Edge edge = new Edge(nextVertex, cost, direction);
        this.edge.Add(edge);
    }
    public List<Edge> getEdge(){
        return edge;
    }
}

public class Graph
{
    private int[,] graph;
    private int[] shape;
    private List<PickUpPoint> pickUpPoints = new List<PickUpPoint>();
    private List<DropOffPoint> dropOffPoints = new List<DropOffPoint>();
    private List<ChargingPoint> chargingPoints = new List<ChargingPoint>();
    private List<List<Vertex>> vertices = new List<List<Vertex>>();
    private int [] moveGrids = {0,1,2,3,4,8};
    private List<Area> areas = new List<Area>();

    public Graph(int[,] graph){
        this.graph = graph;
        shape = new int []{graph.GetUpperBound(0), graph.GetUpperBound(1)};

        // vertices labels
        // 0 = intersection
        // 1 = left
        // 2 = right
        // 3 = up
        // 4 = down
        // 6 = pickup points
        // 7 = dropoff points
        // 8 = charging points
        // 9 = not used

        // Create Vertices
        for (int i = 0; i <= shape[0]; i++){
            List<Vertex> tempVertices = new List<Vertex>();
            for (int j = 0; j <= shape[1]; j++){
                int currentType = graph[i,j];
                Vertex v;
                if (Array.Exists(moveGrids, element => element == currentType)){
                v = new MoveGrid(new int [] {i,j}, graph[i,j]);
                }
                else if (currentType == 6){
                    v = new PickUpPoint(new int [] {i,j}, graph[i,j]);
                    pickUpPoints.Add(v as PickUpPoint);
                }
                else if (currentType == 7){
                    v = new DropOffPoint(new int [] {i,j}, graph[i,j]);
                    dropOffPoints.Add(v as DropOffPoint);
                }
                else if (currentType == 8){
                    v = new ChargingPoint(new int [] {i,j}, graph[i,j]);
                    chargingPoints.Add(v as ChargingPoint);
                }
                else{
                    v = new Vertex(new int [] {i,j}, graph[i,j]);
                }
                tempVertices.Add(v);
            }
            vertices.Add(tempVertices);
        }

        // Add Edges
        for (int i = 0; i <= shape[0]; i++){
            for (int j = 0; j <= shape[1]; j++){
                Vertex currentVertex = vertices[i][j];
                int currentType = currentVertex.getType();

                // Intersection
                if (currentType == 0){
                    if (i != 0 && 
                    Array.Exists(new int [] {0,1,2,3,8}, element => element == graph[i-1,j]) &&
                    (i == shape[0] || graph[i+1,j] != 4) 
                    // && (i <= 1 || graph[i-2,j] != 4)
                    
                    ){
                        (currentVertex as MoveGrid).addEdge(vertices[i-1][j], 3, 3);
                    }

                    if (i != shape[0] && 
                    Array.Exists(new int [] {0,1,2,4,8}, element => element == graph[i+1,j]) &&
                    (i == 0 || graph[i-1,j] != 3) 
                    // && (i >= shape[0]-2 || graph[i+2,j] != 3)
                    
                    ){
                        (currentVertex as MoveGrid).addEdge(vertices[i+1][j], 3, 4);
                    }

                    if (j != 0 && 
                    Array.Exists(new int [] {0,1,3,4,8}, element => element == graph[i,j-1]) &&
                    (j == shape[1] || graph[i,j+1] != 2) 
                    // && (j <= 1 || graph[i,j-2] != 2)
                    
                    ){
                        (currentVertex as MoveGrid).addEdge(vertices[i][j-1], 3, 1);
                    }

                    if (j != shape[1] && 
                    Array.Exists(new int [] {0,2,3,4,8}, element => element == graph[i,j+1]) &&
                    (j == 0 || graph[i,j-1] != 1) 
                    // && (j >= shape[1]-2 || graph[i,j+2] != 1)
                    
                    ){
                        (currentVertex as MoveGrid).addEdge(vertices[i][j+1], 3, 2);
                    }
                }

                // Other grids to move
                else if (Array.Exists(moveGrids, element => element == currentType)){
                    if (i != 0 && currentType != 4 && 
                    (graph[i-1,j] == 0 || graph[i-1,j] == currentType)){
                        (currentVertex as MoveGrid).addEdge(vertices[i-1][j], 3, 3);
                    }

                    if (i != shape[0] && currentType != 3 && 
                    (graph[i+1,j] == 0 || graph[i+1,j] == currentType)){
                        (currentVertex as MoveGrid).addEdge(vertices[i+1][j], 3, 4);
                    }

                    if (j != 0 && currentType != 2 && 
                    (graph[i,j-1] == 0 || graph[i,j-1] == currentType)){
                        (currentVertex as MoveGrid).addEdge(vertices[i][j-1], 3, 1);
                    }

                    if (j != shape[1] && currentType != 1 && 
                    (graph[i,j+1] == 0 || graph[i,j+1] == currentType)){
                        (currentVertex as MoveGrid).addEdge(vertices[i][j+1], 3, 2);
                    }
                }

                // Pick up points
                else if (currentType == 6){
                    if (i != 0 && Array.Exists(moveGrids, element => element == graph[i-1,j])){
                        (currentVertex as PickUpPoint).addPickUpGrids(vertices[i-1][j]);
                    }
                    if (i != shape[0] && Array.Exists(moveGrids, element => element == graph[i+1,j])){
                        (currentVertex as PickUpPoint).addPickUpGrids(vertices[i+1][j]);
                    }
                    if (j != 0 && Array.Exists(moveGrids, element => element == graph[i,j-1])){
                        (currentVertex as PickUpPoint).addPickUpGrids(vertices[i][j-1]);
                    }
                    if (j != shape[1] && Array.Exists(moveGrids, element => element == graph[i,j+1])){
                        (currentVertex as PickUpPoint).addPickUpGrids(vertices[i][j+1]);
                    }
                }

                // Drop off points
                else if(currentType == 7){
                    if (i != 0 && Array.Exists(moveGrids, element => element == graph[i-1,j])){
                        (currentVertex as DropOffPoint).addDropOffGrids(vertices[i-1][j]);
                    }
                    if (i != shape[0] && Array.Exists(moveGrids, element => element == graph[i+1,j])){
                        (currentVertex as DropOffPoint).addDropOffGrids(vertices[i+1][j]);
                    }
                    if (j != 0 && Array.Exists(moveGrids, element => element == graph[i,j-1])){
                        (currentVertex as DropOffPoint).addDropOffGrids(vertices[i][j-1]);
                    }
                    if (j != shape[1] && Array.Exists(moveGrids, element => element == graph[i,j+1])){
                        (currentVertex as DropOffPoint).addDropOffGrids(vertices[i][j+1]);
                    }
                }

                // Charging points
                if (currentType == 8){
                    if (i != 0 && 
                    Array.Exists(new int [] {0,1,2,3}, element => element == graph[i-1,j])){
                        (currentVertex as ChargingPoint).addEdge(vertices[i-1][j], 3, 3);
                    }

                    if (i != shape[0] && 
                    Array.Exists(new int [] {0,1,2,4}, element => element == graph[i+1,j])){
                        (currentVertex as ChargingPoint).addEdge(vertices[i+1][j], 3, 4);
                    }

                    if (j != 0 && 
                    Array.Exists(new int [] {0,1,3,4}, element => element == graph[i,j-1])){
                        (currentVertex as ChargingPoint).addEdge(vertices[i][j-1], 3, 1);
                    }

                    if (j != shape[1] && 
                    Array.Exists(new int [] {0,2,3,4}, element => element == graph[i,j-1])){
                        (currentVertex as ChargingPoint).addEdge(vertices[i][j+1], 3, 2);
                    }
                }
            }
        }
    }

    public int[,] getGraph(){
        return graph;
    }

    public int[] getShape(){
        return shape;
    }

    public Vertex findVertex(int[] position){
        return vertices[position[0]][position[1]];
    }

    public List<PickUpPoint> getPickUpPoints(){
        return pickUpPoints;
    }

    public List<DropOffPoint> getDropOffPoints(){
        return dropOffPoints;
    }

    public void addArea(Area area){
        int[] topLeftPoint = area.getTopLeftPoint();
        int[] bottomRightPoint = area.getBottomRightPoint();
        
        // detect which area that the vertices are located at
        for(int i = 0; i < vertices.Count; i++){
            for (int j = 0; j < vertices[i].Count; j++){
                int[] vertexPosition = vertices[i][j].getLabel();
                if(vertexPosition[0] >= bottomRightPoint[0] && vertexPosition[0] <= topLeftPoint[0] &&
                vertexPosition[1] >= topLeftPoint[1] && vertexPosition[1] <= bottomRightPoint[1]){
                    vertices[i][j].setArea(area);
                }
            }
        }

        // add precalculated route from the exit of one area to the entrance of another area
        List<Vertex> areaExitPoints = area.getExitPoints();
        List<Vertex> areaEntrancePoints = area.getEntrancePoints();
        for (int i = 0; i < areas.Count; i++){
            List<Vertex> exitPoints = areas[i].getExitPoints();
            List<Vertex> entrancePoints = areas[i].getEntrancePoints();
            for (int j = 0; j < exitPoints.Count; j++){
                Vertex currentExitPoint = exitPoints[j];
                Tuple<List<Vertex>, int, List<Vertex>> routeFound = findRouteWithMultipleDestinations(currentExitPoint, new List<List<Vertex>> {areaEntrancePoints});
                areas[i].addPrecalculatedRoutes(currentExitPoint, routeFound.Item3[0], routeFound.Item1, routeFound.Item2);
            }
            for (int j = 0; j < areaExitPoints.Count; j++){
                Vertex currentExitPoint = areaExitPoints[j];
                Tuple<List<Vertex>, int, List<Vertex>> routeFound = findRouteWithMultipleDestinations(currentExitPoint, new List<List<Vertex>> {entrancePoints});
                area.addPrecalculatedRoutes(currentExitPoint, routeFound.Item3[0], routeFound.Item1, routeFound.Item2);
            }
        }

        areas.Add(area);
    }

    public Area getArea(int[] position){
        for(int i = 0; i < areas.Count; i++){
            int[] topLeftPoint = areas[i].getTopLeftPoint();
            int[] bottomRightPoint = areas[i].getBottomRightPoint();
            if(position[0] >= bottomRightPoint[0] && position[0] <= topLeftPoint[0] &&
            position[1] >= topLeftPoint[1] && position[1] <= bottomRightPoint[1]){
                return areas[i];
            }
        }
        return null;
    }

    public Tuple<List<Vertex>, int> findRouteWithOneDestination(Vertex vertexStart, Vertex vertexEnd, int changeDirectionPenalize = 0){
        // Store visited vertices
        List<Vertex> visited = new List<Vertex>();

        if (vertexStart == vertexEnd){
            return Tuple.Create(visited, 0);
        }

        visited.Add(vertexStart);

        List<Edge> toVisit = (vertexStart as MoveGrid).getEdge();
        List<int> visitCosts = new List<int>();
        List<List<Vertex>> visitPaths = new List<List<Vertex>>();
        int movingDirection = 0;

        for (int i = 0; i < toVisit.Count; i++){
            visitCosts.Add(toVisit[i].getCost());
            List<Vertex> newPath = new List<Vertex>();
            newPath.Add(toVisit[i].getNextVertex());
            visitPaths.Add(newPath);
        }

        while(toVisit.Count != 0){
            int shortest = 0;
            visited.Add(toVisit[shortest].getNextVertex());
            movingDirection = toVisit[shortest].getDirection();
            List<Vertex> curRoute = visitPaths[shortest];
            int curCost = visitCosts[shortest];

            // destination reached
            if (visited.Last() == vertexEnd){
                return Tuple.Create(curRoute, curCost);
            }

            // remove visited nodes from the nodes to be visited
            int i = 0;
            while (i < toVisit.Count){
                if(toVisit[i].getNextVertex() == visited.Last()){
                    toVisit.RemoveAt(i);
                    visitCosts.RemoveAt(i);
                    visitPaths.RemoveAt(i);
                    continue;
                }
                i++;
            }

            // Add edges from the last visited node
            List<Edge> newEdges = (visited.Last() as MoveGrid).getEdge();
            for(int j = 0; j < newEdges.Count; j++){
                toVisit.Add(newEdges[j]);

                // Penalize for changing direction
                int nextDirection = newEdges[j].getDirection();
                if(movingDirection != nextDirection){
                    visitCosts.Add(curCost + newEdges[j].getCost() + changeDirectionPenalize);
                }
                else{
                    visitCosts.Add(curCost + newEdges[j].getCost());
                }

                visitCosts.Add(curCost + newEdges[j].getCost());
                List<Vertex> newRoute = new List<Vertex>();
                for (int k = 0; k < curRoute.Count; k++){
                    newRoute.Add(curRoute[k]);
                }
                newRoute.Add(newEdges[j].getNextVertex());
                visitPaths.Add(newRoute);
            }
        }
        return Tuple.Create(new List<Vertex>(), 0);
    }

    // Use wisely. Very computationaly expensive.
    public Tuple<List<Vertex>, int, List<Vertex>> findRouteWithMultipleDestinations(Vertex vertexStart, List<List<Vertex>> destinations){
        // Store visited vertices
        List<List<Vertex>> visited = new List<List<Vertex>>();
        List<Tuple<Edge, int>> toVisit = new List<Tuple<Edge, int>>(); 
        List<int> visitCosts = new List<int>();
        List<List<Vertex>> visitPaths = new List<List<Vertex>>();
        List<int> destinationsVisited = new List<int>();
        List<List<Vertex>> destinationVertices = new List<List<Vertex>>();
        int firstVertexDestination = 0;

        // Check if start vertex if the first destination
        if(destinations[0].Contains(vertexStart)){
            firstVertexDestination += 1;
        }

        // If there is only one destination and the destination is the start vertex
        if (destinations.Count == firstVertexDestination){
            return Tuple.Create(new List<Vertex>(), 0, new List<Vertex>(){vertexStart});
        }

        // seperated visited vertices lists for different destinations
        for (int i = 0; i < destinations.Count; i++){
            visited.Add(new List<Vertex>(){});
        }
        visited[firstVertexDestination].Add(vertexStart);

        // Add the first vertex to visit
        List<Edge> firstToVisit = (vertexStart as MoveGrid).getEdge();
        for(int i = 0; i < firstToVisit.Count; i++){
            toVisit.Add(Tuple.Create(firstToVisit[i], firstVertexDestination));
        }

        for (int i = 0; i < toVisit.Count; i++){ 
            Tuple<Edge, int> currentToVisit = toVisit[i];
            visitCosts.Add(currentToVisit.Item1.getCost()); 
            List<Vertex> newPath = new List<Vertex>();
            newPath.Add(currentToVisit.Item1.getNextVertex()); 
            visitPaths.Add(newPath);
            destinationsVisited.Add(firstVertexDestination);
            if (firstVertexDestination == 1){
                destinationVertices.Add(new List<Vertex>(){vertexStart});
            }
            else{
                destinationVertices.Add(new List<Vertex>());
            }
        }

        //Add exit points as visited if the destination is not an exit point
        for(int i = 0; i < destinations.Count; i++){
            List<Vertex> currentAreaExitPoints = destinations[i].First().getArea().getExitPoints();
            if(!currentAreaExitPoints.Contains(destinations[i].First())){
                visited[i].AddRange(currentAreaExitPoints);
            }
        }

        while(toVisit.Count != 0){ 
            int shortest = visitCosts.IndexOf(visitCosts.Min());
            int curDestinationsVisited = destinationsVisited[shortest];
            List<Vertex> curVerticesVisited = destinationVertices[shortest];
            Tuple<Edge, int> nextToVisit = toVisit[shortest];
            List<Vertex> curRoute = visitPaths[shortest];
            int curCost = visitCosts[shortest];

            toVisit.RemoveAt(shortest);
            visitCosts.RemoveAt(shortest);
            visitPaths.RemoveAt(shortest);
            destinationsVisited.RemoveAt(shortest);
            destinationVertices.RemoveAt(shortest);

            // continue if the node has already been visited
            if(visited[curDestinationsVisited].Contains(nextToVisit.Item1.getNextVertex())){
                continue;
            }
            visited[curDestinationsVisited].Add(nextToVisit.Item1.getNextVertex());

            // destination reached
            if (destinations[curDestinationsVisited].Contains(visited[curDestinationsVisited].Last())){
                Vertex lastVisitedVertex = visited[curDestinationsVisited].Last();
                curVerticesVisited.Add(lastVisitedVertex);
                curDestinationsVisited += 1;

                Area currentArea = lastVisitedVertex.getArea();

                // found all destinations
                if (curDestinationsVisited == destinations.Count){
                    return Tuple.Create(curRoute, curCost, curVerticesVisited);
                }

                // Add precalculated routes if the current destination is an exit point
                if(currentArea.getExitPoints().Contains(lastVisitedVertex)){
                    Tuple<List<Vertex>, int> precalculatedRoute = currentArea.getPrecalculatedRoutes(lastVisitedVertex, destinations[curDestinationsVisited]);
                    if(precalculatedRoute != null){
                        curRoute.AddRange(precalculatedRoute.Item1);
                        curCost += precalculatedRoute.Item2;
                        curDestinationsVisited += 1;
                        lastVisitedVertex = precalculatedRoute.Item1.Last();
                    }
                }

                visited[curDestinationsVisited].Add(lastVisitedVertex);
            }

            // Get new edges to visit
            List<Edge> newEdges = (visited[curDestinationsVisited].Last() as MoveGrid).getEdge();

            // Add edges from the last visited node
            for(int j = 0; j < newEdges.Count; j++){
                //check if the node has been visited before
                Edge currentEdge = newEdges[j];
                if(!visited[curDestinationsVisited].Contains(currentEdge.getNextVertex())){
                    toVisit.Add(Tuple.Create(currentEdge,curDestinationsVisited));
                    visitCosts.Add(curCost + currentEdge.getCost());

                    List<Vertex> newRoute = new List<Vertex>();
                    for (int k = 0; k < curRoute.Count; k++){
                        newRoute.Add(curRoute[k]);
                    }
                    List<Vertex> newVerticesVisited = new List<Vertex>();
                    for(int k = 0; k < curVerticesVisited.Count; k++){
                        newVerticesVisited.Add(curVerticesVisited[k]);
                    }
                    newRoute.Add(currentEdge.getNextVertex());
                    visitPaths.Add(newRoute);
                    destinationsVisited.Add(curDestinationsVisited);
                    destinationVertices.Add(newVerticesVisited);
                }
            }
        }
        return Tuple.Create(new List<Vertex>(), 0, new List<Vertex>());
    }
}