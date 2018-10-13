using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextController : MonoBehaviour, IPointerClickHandler
{

    public Text textComponent;
    public GameObject textPanel;

    public delegate void ViewNext();

    public event ViewNext evento;

    private int lineCount = 0;

    public float lineHeight;

    void Update()
    {
        lineCount = textComponent.cachedTextGenerator.lines.Count;
        float myHeight = (lineCount + 1) * lineHeight;
        textPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, myHeight);
    }

    public void addEvent(ViewNext fx)
    {
        evento += fx;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (evento != null)
        {
            evento();
        }
    }

}
