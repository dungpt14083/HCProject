using UnityEngine;
using System.Collections;
using Salday.EventBus;
using Assets.Demo;
using System;

public class ChatProxy : UnityEventProxyBase
{
    #region Editor Vars
    [SerializeField]
    Material inactive;
    [SerializeField]
    Material active;

    [SerializeField]
    GameObject tooltip;

    TextMesh textMesh;
    Renderer cubeRenderer;

    #endregion

    protected override void Awake()
    {
        //Get necessary references and set initial state
        textMesh = tooltip.GetComponent<TextMesh>();
        cubeRenderer = GetComponent<Renderer>();
        cubeRenderer.material = inactive;
    }


    [Handler(HandlerPriority.Lowest)]
    public void SendMessageToChat(ChatEvArgs arg)
    {
        //Don't want to do anything, if event is already canceled
        if (arg.Canceled) return;

        //Visual representation
        ActivateSphere();
        EnableAndSetTooltip(string.Format("Received message:{0}{1}", Environment.NewLine, arg.Message));
    }

    [Handler(HandlerPriority.Low)]
    public void ResetSphere(ResetSpheres arg)
    {
        //Reset sphere to it's initial state
        tooltip.SetActive(false); //Deactivate tooltip with text
        cubeRenderer.material = inactive; //Set back the color of an object
    }

    void EnableAndSetTooltip(string text)
    {
        //Enable tooltip with text
        tooltip.SetActive(true);
        //Set text in user interface
        textMesh.text = text;
    }

    void ActivateSphere()
    {
        //Change object color
        cubeRenderer.material = active;
    }
}
