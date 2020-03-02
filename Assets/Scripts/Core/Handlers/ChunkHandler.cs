using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHandler : MonoBehaviour
{
    Renderer red;
    // Start is called before the first frame update
    void OnEnable()
    {
        red = GetComponent<Renderer>();
        StartCoroutine(Dissolve());
    }

    public void delete()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());
        gameObject.SetActive(false);
    }

    IEnumerator Dissolve()
    {
        for (float i = 0; i < 2f; i += Time.deltaTime)
        {
            red.material.SetFloat("_SliceAmount", Mathf.Lerp(0, 1, i));
            yield return null;
        }

        Invoke("delete", 0);
    }
}
