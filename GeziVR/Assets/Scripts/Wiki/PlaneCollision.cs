using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCollision : MonoBehaviour
{
    void OnCollisionExit(Collision other)
    {
        WikipediaAPI.isStatusChanged = true;
        if(this.name == "Plane2")
        {
            WikipediaAPI.isExitedPlane2 = true;
        }
    }
}
