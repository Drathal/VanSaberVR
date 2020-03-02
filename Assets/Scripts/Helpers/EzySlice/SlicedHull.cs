using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class _Sliced
{
    private Mesh _leftSide;
    private Mesh _rightSide;

    public _Sliced(Mesh _leftSide, Mesh _rightSide)
    {
        this._leftSide = _leftSide;
        this._rightSide = _rightSide;
    }

    public GameObject CreateUpperHull(GameObject original, Material crossSectionMat, float angle)
    {
        GameObject newObject = CreateUpperHull();

        if (newObject != null)
        {
            newObject.transform.position = original.transform.position;
            newObject.transform.localRotation = original.transform.localRotation * Quaternion.Euler(0.0f, angle, 0.0f);
            newObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            Rigidbody new_rig = newObject.GetComponent<Rigidbody>();
            BoxCollider new_col = newObject.GetComponent<BoxCollider>();

            if (new_rig == null)
            {
                new_rig = newObject.AddComponent<Rigidbody>();
            }

            if (new_col == null)
            {
                new_col = newObject.AddComponent<BoxCollider>();
            }


            new_rig.AddForce(original.transform.forward * 600 - original.transform.right * 300 - original.transform.up * 300);
            new_rig.AddTorque(original.transform.forward * 600 + original.transform.right * 300 + original.transform.up * 600);
            new_rig.mass = 8;

            Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
            Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

            // nothing changed in the hierarchy, the cross section must have been batched
            // with the submeshes, return as is, no need for any changes
            if (mesh.subMeshCount == _leftSide.subMeshCount)
            {
                // the the material information
                newObject.GetComponent<Renderer>().sharedMaterials = shared;

                return newObject;
            }

            // otherwise the cross section was added to the back of the submesh array because
            // it uses a different material. We need to take this into account
            Material[] newShared = new Material[shared.Length + 1];

            // copy our material arrays across using native copy (should be faster than loop)
            System.Array.Copy(shared, newShared, shared.Length);
            newShared[shared.Length] = crossSectionMat;

            // the the material information
            newObject.GetComponent<Renderer>().sharedMaterials = newShared;
            //newObject.AddComponent<Trash>();
        }

        return newObject;
    }

    public GameObject CreateLowerHull(GameObject original, Material crossSectionMat, float angle)
    {
        GameObject newObject = CreateLowerHull();

        if (newObject != null)
        {
            newObject.transform.position = original.transform.position;
            newObject.transform.localRotation = original.transform.localRotation * Quaternion.Euler(0.0f, angle, 0.0f);
            newObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            Rigidbody new_rig = newObject.GetComponent<Rigidbody>();
            BoxCollider new_col = newObject.GetComponent<BoxCollider>();

            if (new_rig == null)
            {
                new_rig = newObject.AddComponent<Rigidbody>();
            }

            if (new_col == null)
            {
                new_col = newObject.AddComponent<BoxCollider>();
            }

            new_rig.AddForce(original.transform.forward * 600 + original.transform.right * 300 - original.transform.up * 300);
            new_rig.AddTorque(-original.transform.forward * 600 - original.transform.right * 300 - original.transform.up * 600);
            new_rig.mass = 8;

            Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
            Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

            // nothing changed in the hierarchy, the cross section must have been batched
            // with the submeshes, return as is, no need for any changes
            if (mesh.subMeshCount == _rightSide.subMeshCount)
            {
                // the the material information
                newObject.GetComponent<Renderer>().sharedMaterials = shared;

                return newObject;
            }

            // otherwise the cross section was added to the back of the submesh array because
            // it uses a different material. We need to take this into account
            Material[] newShared = new Material[shared.Length + 1];

            // copy our material arrays across using native copy (should be faster than loop)
            System.Array.Copy(shared, newShared, shared.Length);
            newShared[shared.Length] = crossSectionMat;

            // the the material information
            newObject.GetComponent<Renderer>().sharedMaterials = newShared;
            //newObject.AddComponent<Trash>();
        }

        return newObject;
    }

    /**
     * Generate a new GameObject from the upper hull of the mesh
     * This function will return null if upper hull does not exist
     */
    public GameObject CreateUpperHull()
    {
        return CreateEmptyObject("Upper_Hull", _leftSide);
    }

    /**
     * Generate a new GameObject from the Lower hull of the mesh
     * This function will return null if lower hull does not exist
     */
    public GameObject CreateLowerHull()
    {
        return CreateEmptyObject("Lower_Hull", _rightSide);
    }

    public Mesh upperHull
    {
        get { return this._leftSide; }
    }

    public Mesh lowerHull
    {
        get { return this._rightSide; }
    }

    /**
     * Helper function which will create a new GameObject to be able to add
     * a new mesh for rendering and return.
     */
    private static GameObject CreateEmptyObject(string name, Mesh hull)
    {
        if (hull == null)
        {
            return null;
        }

        GameObject newObject = new GameObject(name);
        newObject.transform.SetParent(PoolHandler.Instance.gameObject.transform);

        newObject.AddComponent<MeshRenderer>();
        MeshFilter filter = newObject.AddComponent<MeshFilter>();

        filter.mesh = hull;

        ChunkHandler _chunk = newObject.GetComponent<ChunkHandler>();

        if (_chunk == null)
            _chunk = newObject.AddComponent<ChunkHandler>();

        return newObject;
        /*
        if (hull == null)
        {
            return null;
        }

        GameObject newObject = ObjectPooler._current.GetPooledChunk();

        newObject.GetComponent<MeshRenderer>();
        MeshFilter filter = newObject.GetComponent<MeshFilter>();

        filter.mesh = hull;
        Chunks _chunk;

        _chunk = newObject.GetComponent<Chunks>();

        if(_chunk == null)
            _chunk = newObject.AddComponent<Chunks>();

        newObject.SetActive(true);

        return newObject;*/
    }
}