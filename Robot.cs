using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    private int label;
    private bool serverApproved;
    private int[] position;
    public Robot(int label, int[] position){
        this.label = label;
        this.position = position;
        serverApproved = false;
    }
    public void setApprove(bool serverApproved){
        this.serverApproved = serverApproved;
    }
    public bool getApprove(){
        return serverApproved;
    }
    public void updatePosition(int[] position){
        this.position = position;
    }
    public void setPosition(int[] position){
        this.position = position;
    }
    public int[] getPosition(){
        return position;
    }
}
