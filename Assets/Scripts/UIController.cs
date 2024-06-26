using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public enum UIState
    {
        Charts,
        ServerActivity
    }
    
    public enum UIStateAux
    {
        Green,
        Grey,
        Red
    }

    [SerializeField] private GameObject m_greenAux;
    [SerializeField] private GameObject m_greyAux;
    [SerializeField] private GameObject m_redAux;
    
    [SerializeField] private GameObject m_charts;
    [SerializeField] private GameObject m_serverActivity;


    public void OnEnable()
    {
        GameEvents.OnChangeUIStateAux += ChangeUIStateAux;
        GameEvents.OnClickOnNode += SetCanvasView;
    }
    
    public void OnDisable()
    {
        GameEvents.OnChangeUIStateAux -= ChangeUIStateAux;
        GameEvents.OnClickOnNode -= SetCanvasView;
    }

    private void ChangeUIStateAux(UIStateAux uiStateAux)
    {
        m_greenAux.SetActive(false);
        m_greyAux.SetActive(false);
        m_redAux.SetActive(false);

        switch (uiStateAux)
        {
            case UIStateAux.Green:
                m_greenAux.SetActive(true);
                break;
            case UIStateAux.Grey:
                m_greyAux.SetActive(true);
                break;
            case UIStateAux.Red:
                m_redAux.SetActive(true);
                break;
        }
    }

    public void ChangeUIStateCharts()
    {
        ChangeUIState(UIState.Charts);
    }
    
    public void ChangeUIStateServerActivity()
    {
        ChangeUIState(UIState.ServerActivity);
    }
    
    private void ChangeUIState(UIState uiState)
    {
        m_greenAux.SetActive(false);
        m_greyAux.SetActive(false);
        m_redAux.SetActive(false);
        
        m_charts.SetActive(false);
        m_serverActivity.SetActive(false);

        switch (uiState)
        {
            case UIState.Charts:
                m_charts.SetActive(true);
                break;
            case UIState.ServerActivity:
                m_serverActivity.SetActive(true);
                break;
        }
    }


    public void JustShowGrey()
    {
        m_greenAux.SetActive(false);
        m_greyAux.SetActive(true);
        m_redAux.SetActive(false);
    }

    [SerializeField] private GameObject m_ui3D;
    [SerializeField] private GameObject m_canvas;
    [SerializeField] private ViewOverseer m_ViewOverseer;
    
    public void Set3DView()
    {
        m_ui3D.SetActive(true);
        m_canvas.SetActive(false);
    }

    public void SetCanvasView()
    {
        m_ui3D.SetActive(false);
        m_canvas.SetActive(true);

    }
    

    public void SetCanvasView(int index)
    {
        m_canvas.SetActive(true);
        
        m_ViewOverseer.OnUpdateNodeDataCoreLogic(index);
    }
    
}
