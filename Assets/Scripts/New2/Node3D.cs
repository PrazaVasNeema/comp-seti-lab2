using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Node3D : MonoBehaviour
{
    public Material highlightMaterial; // Assign the highlight material in the inspector

    [SerializeField] private TMP_Text idText;
    [SerializeField] private Vector3 offset = Vector3.up; // Offset for the text position
    [SerializeField] private Color lineColor = Color.white; // Color of the lines between the nodes

    private Material originalMaterial;
    private Renderer objectRenderer;
    private int m_nodeIndex;
    [FormerlySerializedAs("m_neighbours")] public List<Transform> neighbours = new List<Transform>();
    
    
    private List<LineRenderer> lineRendererList;
    [SerializeField] private float lineWidth = 0.1f;


    public void LoadData(int nodeIndex)
    {

        
        m_nodeIndex = nodeIndex;
        this.neighbours = new List<Transform>();
        idText.text = nodeIndex.ToString();

    }

    public void SetupLineRenderers()
    {
        lineRendererList = new List<LineRenderer>();
        for (int i = 0; i < neighbours.Count; i++)
        {
            var newLineRenderer = new GameObject("LineRenderer").AddComponent<LineRenderer>();
            newLineRenderer.transform.parent = transform;
            lineRendererList.Add(newLineRenderer);
            // lineRendererList[i] = GetComponent<LineRenderer>();
            lineRendererList[i].startWidth = lineWidth;
            lineRendererList[i].endWidth = lineWidth;
            lineRendererList[i].material = new Material(Shader.Find("Sprites/Default"));
            lineRendererList[i].startColor = lineColor;
            lineRendererList[i].endColor = lineColor;
        }
    }
    
    private void Update()
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            lineRendererList[i].SetPosition(0, transform.position);
            lineRendererList[i].SetPosition(1, neighbours[i].position);
        }
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