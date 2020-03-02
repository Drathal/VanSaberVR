using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sliceFollow : MonoBehaviour {

    public GameObject note;
    public GameObject plane;
    void Update()
    {
        transform.LookAt(note.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    public void OnDrawGizmos()
    {
        EzySlice.Plane cuttingPlane = new EzySlice.Plane(plane.transform.position, plane.transform.forward);
        cuttingPlane.Compute(plane.transform);
        cuttingPlane.OnDebugDraw();
    }

}
