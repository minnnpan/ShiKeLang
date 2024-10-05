using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExecuteInEditMode]
public class PositionOutline : MonoBehaviour
{
    public Shader Toon;
    public Shader OutlineShader;
    public Material outlineMaterial;
    public Renderer renderer;

    [HideInInspector] public Material ToonMaterial;
    [HideInInspector] public Material New_OutlineMaterial;
    [HideInInspector] public List<Material> ListRenderMaterials;

    [HideInInspector] public float fireRate = 0.1f;
    [HideInInspector] private float nextFire = 0.0F;
    [HideInInspector] public bool isRemoved;


    void Update()
    {

        // Check if its Used
        if (New_OutlineMaterial && ToonMaterial && renderer)
        {
            if (ToonMaterial.GetFloat("_OutLineMode") == 1 || ToonMaterial.GetFloat("_AddOutline") == 0 || ToonMaterial.GetFloat("_UseOutlines") == 0)
            {
                New_OutlineMaterial.SetFloat("_Scale", 0);

                return;
            }
        }

        // if its Used

        if (Time.time > nextFire)
        {
            this.hideFlags = HideFlags.HideInInspector;
            New_OutlineMaterial.hideFlags = HideFlags.HideInInspector;

            nextFire = Time.time + fireRate;

            // Check if Toon material was removed

            if (renderer)
            {

                List<Material> ListRenderMaterialss = new List<Material>(renderer.sharedMaterials);
                List<Material> ToonMaterials = new List<Material>();


                foreach (Material material in ListRenderMaterialss)
                {
                    if (material.shader == Toon)
                    {
                        ToonMaterials.Add(material);
                    }
                }

                isRemoved = ToonMaterials.Count > 0 ? false : true;
            }

            if (isRemoved)
            {
                RemoveEverything();
            }

        }


        CheckDuo();
        UpdateValues();
    }

    public void CheckDuo()
    {

        List<PositionOutline> PositionOutline_LIST = new List<PositionOutline>();

        if (PositionOutline_LIST.Count > 1)
        {
            if (PositionOutline_LIST.IndexOf(this) != 0)
            {
                DestroyImmediate(this);
            }
        }
    }

    public void SetUp(Material mat)
    {

        renderer = gameObject.GetComponent<Renderer>();

        if (!Application.isPlaying)
        {

            New_OutlineMaterial = mat;
            ListRenderMaterials = new List<Material>(renderer.sharedMaterials);

            // add toon material reference
            foreach (Material material in ListRenderMaterials)
            {
                if (material.shader == Toon)
                {
                    ToonMaterial = material;
                    break;
                }

            }

            // check if "outline material exists"
            foreach (Material material in ListRenderMaterials)
            {
                if (material.shader == OutlineShader)
                {
                    return;
                }
            }


            // add outline material
            ListRenderMaterials.Add(New_OutlineMaterial);
            renderer.materials = ListRenderMaterials.ToArray();
        }
    }
    public void FixOutline()
    {
        renderer = gameObject.GetComponent<Renderer>();

        // check if "outline material exists"
        foreach (Material material in ListRenderMaterials)
        {
            if (material.shader == OutlineShader)
            {
                // add outline material
                ListRenderMaterials.Remove(material);

                renderer.materials = ListRenderMaterials.ToArray();

                SetUp(outlineMaterial);
            }
        }
    }
    public void UpdateValues()
    {
        New_OutlineMaterial.SetColor("_OutlineColor", ToonMaterial.GetColor("_OutlineColor"));
        New_OutlineMaterial.SetFloat("_Scale", ToonMaterial.GetFloat("_OutlineWidth"));
    }
    public void RemoveEverything()
    {
        renderer = gameObject.GetComponent<Renderer>();

        // check the materials list
        List<Material> materialList = new List<Material>(renderer.sharedMaterials);

        // remove material from list
        materialList.Remove(New_OutlineMaterial);
        renderer.materials = materialList.ToArray();

        //delete this component
        DestroyImmediate(this);
    }
}
