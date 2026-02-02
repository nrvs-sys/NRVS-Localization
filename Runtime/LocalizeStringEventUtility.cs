using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class LocalizeStringEventUtility : MonoBehaviour
{
    public enum ValueType
    {
        Int,
        Float,
        Bool,
        String
    }

    [Header("Argument Settings")]

    [Tooltip("Inject argument in OnEnable when the game is running.")]
    [SerializeField] private bool applyOnEnable = true;

    [Tooltip("Also inject while in Edit‑Mode (helps preview in Scene view / Inspector).")]
    [SerializeField] private bool applyInEditMode = false;

    [Header("Argument Definitions")]

    [SerializeField]
    string valueKey;

    [Space(10)]

    public ValueType valueType;

    [SerializeField, ShowIf(nameof(valueType), ValueType.Int)]
    int intValue;
    [SerializeField, ShowIf(nameof(valueType), ValueType.Float)]
    float floatValue;
    [SerializeField, ShowIf(nameof(valueType), ValueType.Bool)]
    bool boolValue;
    [SerializeField, ShowIf(nameof(valueType), ValueType.String)]
    string stringValue;

    [SerializeField, ShowIf(nameof(valueType), ValueType.Float)]
    [Tooltip("The decimal precision of the float value")]
    int floatPrecision = 2;

    LocalizeStringEvent localizeStringEvent;

    void Awake()
    {
        localizeStringEvent = GetComponent<LocalizeStringEvent>();
    }

    void OnEnable()
    {
        if (!Application.isPlaying && applyInEditMode)
            ApplyArgument();
        else if (Application.isPlaying && applyOnEnable)
            ApplyArgument();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && applyInEditMode)
        {
            if (localizeStringEvent == null) localizeStringEvent = GetComponent<LocalizeStringEvent>();
            ApplyArgument();
        }
    }
#endif

    public void Set(int value) => intValue = value;
    public void Set(float value) => floatValue = value;
    public void Set(bool value) => boolValue = value;
    public void Set(string value) => stringValue = value;

    /// <summary>Injects or updates the argument if it changed.</summary>
    public void ApplyArgument()
    {
        if (localizeStringEvent == null || string.IsNullOrEmpty(valueKey))
            return;


        // Try to fetch existing value
        if (!localizeStringEvent.StringReference.TryGetValue(valueKey, out var currentVal))
        {
            switch (valueType)
            {
                case ValueType.Int:
                    currentVal = new IntVariable();
                    break;
                case ValueType.Float:
                    currentVal = new FloatVariable();
                    break;
                case ValueType.Bool:
                    currentVal = new BoolVariable();
                    break;
                case ValueType.String:
                    currentVal = new StringVariable();
                    break;

            }

            localizeStringEvent.StringReference.Add(valueKey, currentVal);
        }

        // Update the value
        var valueChanged = false;
        switch (valueType)
        {
            case ValueType.Int:
                var intVar = currentVal as IntVariable;
                if (intVar != null && intVar.Value != intValue)
                {
                    intVar.Value = intValue;
                    valueChanged = true;
                }
                break;
            case ValueType.Float:
                var floatVar = currentVal as FloatVariable;
                var roundedFloatValue = Mathf.Round(floatValue * Mathf.Pow(10, floatPrecision)) / Mathf.Pow(10, floatPrecision);
                if (floatVar != null && floatVar.Value != roundedFloatValue)
                {
                    floatVar.Value = roundedFloatValue;
                    valueChanged = true;
                }
                break;
            case ValueType.Bool:
                var boolVar = currentVal as BoolVariable;
                if (boolVar != null && boolVar.Value != boolValue)
                {
                    boolVar.Value = boolValue;
                    valueChanged = true;
                }
                break;
            case ValueType.String:
                var stringVar = currentVal as StringVariable;
                if (stringVar != null && stringVar.Value != stringValue)
                {
                    stringVar.Value = stringValue;
                    valueChanged = true;
                }
                break;
        }

        // force immediate refresh
        if (valueChanged)
            localizeStringEvent.RefreshString(); 
    }
}
