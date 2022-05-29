using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class GhostTrail : MonoBehaviour
{
    public GameObject GhostParent;
    public GameObject GhostPrefab;
    private bool timer = true;
    public static bool work = false;
    private int counter = 0;
    public List<Material> materials;
    public int materialIndex;

    private void Start()
    {
       // GhostPrefab.GetComponentInChildren<Renderer>().material = materials[materialIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (work)
        {
            if (timer)
            {
                counter++;
                GhostEffect();
                timer = false;
                Invoke("Timer",.03f);
            }
        }
    }

    private void GhostEffect()
    {
        var go = Instantiate(GhostPrefab,transform.position,transform.rotation);
        go.transform.parent = GhostParent.transform;
        Mesh mesh = new Mesh();
        this.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
        go.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = mesh ;
        Destroy(go,.23f);
    }

    private void Timer()
    {
        timer = true;
    }
}
