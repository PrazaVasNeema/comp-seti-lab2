using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Node3D : MonoBehaviour
{
    public Material highlightMaterial; // Assign the highlight material in the inspector
    
    [SerializeField] private Vector3 offset = Vector3.up; // Offset for the text position
    [SerializeField] private Color lineColor = Color.white; // Color of the lines between the nodes

    private Material originalMaterial;
    private Renderer objectRenderer;
    private int m_nodeIndex;
    [FormerlySerializedAs("m_neighbours")] public List<Transform> neighbours = new List<Transform>();


    public void LoadData(int nodeIndex)
    {
        m_nodeIndex = nodeIndex;
        this.neighbours = new List<Transform>();
    }

    void OnEnable()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
    }

    void OnMouseEnter()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = highlightMaterial;
        }
    }

    void OnMouseExit()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }

    void OnMouseDown()
    {
        GameEvents.OnClickOnNode?.Invoke(m_nodeIndex);
        Debug.Log("CLICK");
    }
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 position = transform.position + offset;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 20;
        
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, m_nodeIndex.ToString(), style);
#endif
        
        Gizmos.color = lineColor;
        foreach (var obj in neighbours)
        {
            if (obj != null)
            {
                Gizmos.DrawLine(transform.position, obj.position);
            }
        }
    }
}