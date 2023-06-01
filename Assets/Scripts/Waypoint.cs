using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private GameObject[] child;
    [SerializeField] private Waypoint[] neighbours;
    private Vector3 offset = new Vector3(0,1,0);
    public Waypoint[] Neighbours { get { return neighbours; } }
    public GameObject[] Child { get { return child; } }

    private void OnDrawGizmos()
    {
        //Draws red lines between a parent and its children.
        if (neighbours.Length != 0)
        {
            foreach (Waypoint neigh in neighbours)
            {
                Debug.DrawLine(transform.position, neigh.transform.position + offset, Color.green);
            }
        }
        //Draws green lines between a child and its children.
        if (child.Length != 0)
        {
            foreach (GameObject cld in child)
            {
               Debug.DrawLine(transform.position, cld.transform.position + offset, Color.red); 
            }
        }
    }
}
