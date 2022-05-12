using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.Rendering;

public class SlicerPlane : MonoBehaviour
{
    [SerializeField] Material cutMaterial;
    [SerializeField] private ParticleSystem sliceParticle;
    [SerializeField] private Transform playerTransform;
    
    public ComboCalculator comboCalculator;
    public Material mat;
    public LayerMask mask;
    bool canlice = true;

   // [SerializeField] private finnishTrigger _finnishTrigger;
    

    private void OnTriggerEnter(Collider other)
    {
        if (canlice)
        {
            Slice();
            comboCalculator.UpdateCounter();
            comboCalculator.RenderComboText();
            canlice = false;
            Invoke("CanliceLock",0.1f);
        }
    }
//Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(new Vector3(transform.position.x,transform.position.y,transform.position.z), new Vector3(5f, 1f, 5f));
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(new Vector3(transform.position.x,transform.position.y+2,transform.position.z), new Vector3(5f, 1f, 5f), transform.rotation,mask);

        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            
            SlicedHull hull = SliceObject(hits[i].gameObject, mat);
            if (hull != null)
            {
                var o = sliceParticle.gameObject;
                o.transform.position =  hits[i].gameObject.transform.position;
                o.transform.rotation = hits[i].gameObject.transform.rotation;
                sliceParticle.Play();
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, mat);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, mat);
                AddHullComponents(bottom);
                AddHullComponents(top);
                if (hits[i].gameObject.transform.parent.parent.parent.GetComponent<lookPlayer>() != null && hits[i].gameObject.transform.parent.parent.parent.GetComponent<Enemy>() != null )
                {
                    hits[i].gameObject.transform.parent.parent.parent.GetComponent<lookPlayer>().enabled = false;
                    hits[i].gameObject.transform.parent.parent.parent.GetComponent<Enemy>().enabled = false;
                }
               
                Destroy(hits[i].gameObject);
            }
        }
    }
    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        // slice the provided object using the transforms of this object
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
    }
    public void AddHullComponents(GameObject go)
    {
        go.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        go.layer = 4;
        Rigidbody rb = go.AddComponent<Rigidbody>();
        go.GetComponent<Renderer>().material = cutMaterial;
        rb.mass = 1f;
        //rb.interpolation = RigidbodyInterpolation.Interpolate;
        //rb.AddExplosionForce(1000f,go.transform.position,2f,3f);
        
        rb.AddForce((go.transform.position- playerTransform.position).normalized* 100f,ForceMode.Impulse);
        BoxCollider collider = go.AddComponent<BoxCollider>();
        
        Destroy(go,2f);
      //  _finnishTrigger.collectBodyParts(go,4f);
    }

    public void CanliceLock()
    {
        canlice = true;
    }
}

