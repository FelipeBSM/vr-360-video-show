using UnityEngine;
using System.Collections.Generic;

public enum Perspective {Plateia, Lead, Bass, Drums, Guitar}

[System.Serializable]
public class UIElementConfig
{
    public Perspective perspectiveType; 
    public List<UIElementState> elementStates;
}

[System.Serializable]
public class UIElementState
{
    public GameObject uiObject;   // O botão (ex: Botão Lead)
    public Vector3 position;      // Posição para esta vista
    public Vector3 scale = Vector3.one; // Escala para esta vista
    public bool isActive;         // Se deve aparecer nesta vista
}

public class UIPerspectiveManager : MonoBehaviour
{
    [SerializeField] private List<UIElementConfig> perspectives;

    public void ApplyPerspective(Perspective type)
    {
        var config = perspectives.Find(p => p.perspectiveType == type);

        if (config != null)
        {
            foreach (var state in config.elementStates)
            {
                state.uiObject.SetActive(state.isActive);
                state.uiObject.transform.localPosition = state.position;
                state.uiObject.transform.localScale = state.scale;
            }
        }
    }
}
