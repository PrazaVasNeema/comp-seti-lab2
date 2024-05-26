using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

public class ResultAuxParamsValues : MonoBehaviour
{
    [SerializeField] private int thisNum;
    
    [SerializeField] private GameObject paramsThemselvesGO;
    
    [SerializeField] private TMP_Text PProstoi;
    [SerializeField] private TMP_Text CIntencity;


    private void OnEnable()
    {
        GameEvents.OnChangeResultAuxParamsView += OnChangeAuxParamsView;
        GameEvents.OnClearResultAuxParamsView += OnClearAuxParamsView;
    }



    private void OnDisable()
    {
        GameEvents.OnChangeResultAuxParamsView -= OnChangeAuxParamsView;
        GameEvents.OnClearResultAuxParamsView -= OnClearAuxParamsView;
    }
    
    private void OnChangeAuxParamsView(int arg1, Vector2 arg2)
    {
       
            
        if (arg1 == thisNum)
        {
            var output1 = GetFirstNDigitsIncludingDecimal(arg2.x, numberOfDigits: 4);
            var output2 = GetFirstNDigitsIncludingDecimal(arg2.y, numberOfDigits: 4);
            if (arg2 == new Vector2(-1, -1))
            {
                output1 = "_";
                output2 = "_";
            }
            PProstoi.text = output1;
            CIntencity.text = output2;
            paramsThemselvesGO.SetActive(true);
        }
    }
    
    private void OnClearAuxParamsView()
    {
        paramsThemselvesGO.SetActive(false);
    }
    
    public static string GetFirstNDigitsIncludingDecimal(float value, int numberOfDigits)
    {
        // Convert the float to a string without using scientific notation
        string valueStr = value.ToString("F99", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
        
        // Check if the valueStr contains a decimal point
        int decimalIndex = valueStr.IndexOf('.');
        if (decimalIndex == -1)
        {
            // If there's no decimal point and the length is less than required digits, return the number itself
            return valueStr.Length <= numberOfDigits ? valueStr : valueStr.Substring(0, numberOfDigits);
        }
        
        // If the total length of the number including the decimal point is less than required digits, return the number itself
        if (valueStr.Length <= numberOfDigits)
        {
            return valueStr;
        }

        // Handle the case when the number has fewer digits before the decimal point than required
        if (decimalIndex < numberOfDigits)
        {
            // Extract the numberOfDigits including the decimal point
            return valueStr.Substring(0, numberOfDigits);
        }
        else
        {
            // If the decimal point is beyond the required number of digits, just extract the first numberOfDigits characters
            return valueStr.Substring(0, numberOfDigits);
        }
    }


}
