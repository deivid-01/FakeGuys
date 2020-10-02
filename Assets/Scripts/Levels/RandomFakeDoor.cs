using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFakeDoor : MonoBehaviour
{
    [System.Serializable]
    public class Wall
    {
        public GameObject leftSide;
        public GameObject rigthSide;

        public bool isFake;

        public Wall(GameObject left, GameObject right)
        {
            this.leftSide = left;
            this.rigthSide = right;
            isFake = false;
        }
    }


    public List<Wall> walls;

    void Start()
    {
        InitializeWalls();
        SetFakeDoors();

        MakeFakeDoors();
    }

    public void InitializeWalls()
    {
        walls = new List<Wall>(transform.childCount / 2);
        for (int i = 0; (i + 1) <= transform.childCount; i += 2)
        {
            Wall wall = new Wall(transform.GetChild(i).gameObject, transform.GetChild(i + 1).gameObject);
            walls.Add(wall);
        }

    }

    public void MakeFakeDoors()
    {
        foreach (var wall in walls)
        {
            if (wall.isFake)
            {
                wall.leftSide.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                wall.rigthSide.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    public void SetFakeDoors()
    {

        int numFakeDoors = ((int)walls.Count / 2);

        int cont = 0;

        while (cont < numFakeDoors)
        {
            int index = Random.Range(0, walls.Count - 1);

            if (!walls[index].isFake)
            {
                walls[index].isFake = true;
                cont++;
            }      
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (var wall in walls)
        {
            if (!wall.isFake)
            {
                Gizmos.DrawCube(wall.leftSide.transform.position, wall.leftSide.transform.localScale + Vector3.forward);
                Gizmos.DrawCube(wall.rigthSide.transform.position, wall.rigthSide.transform.localScale + Vector3.forward);
            }
        }
        
    }

}


