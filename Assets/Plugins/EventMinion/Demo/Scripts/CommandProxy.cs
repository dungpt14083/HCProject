using UnityEngine;
using Assets.Demo;
using Salday.EventBus;
using System;

public class CommandProxy : UnityEventProxyBase
{
    #region Editor Vars
    [SerializeField]
    Material inactive;
    [SerializeField]
    Material active;

    [SerializeField]
    GameObject tooltip;

    TextMesh textMesh;
    Renderer sphereRenderer;

    #endregion

    protected override void Awake()
    {
        //Get necessary references and set initial state
        textMesh = tooltip.GetComponent<TextMesh>();
        sphereRenderer = GetComponent<Renderer>();
        sphereRenderer.material = inactive;
    }

    [Handler(HandlerPriority.Highest)]
    public void HandleIncomingMessage(ChatEvArgs arg)
    {
        //Don't want to do anything, if event is already canceled
        if (arg.Canceled) return;
        if (arg.Message.StartsWith("/"))
        {
            //Inform other handlers, that there is no need to process this event
            //it is already handled here
            arg.Canceled = true;

            //Visual representation
            ActivateShphere();
            EnableAndSetTooltip(string.Format("Message starts with '/' {0} it'll be handled here", Environment.NewLine));

        }
        else
            EnableAndSetTooltip(string.Format("No '/' found {0} continuing", Environment.NewLine));

    }

    [Handler(HandlerPriority.Low)]
    public void ResetSphere(ResetSpheres arg)
    {
        //Reset sphere to it's initial state
        tooltip.SetActive(false); //Deactivate tooltip with text
        sphereRenderer.material = inactive; //Set back the color of an object
    }

    void EnableAndSetTooltip(string text)
    {
        //Enable tooltip with text
        tooltip.SetActive(true);
        //Set text in user interface
        textMesh.text = text;
    }

    void ActivateShphere()
    {
        //Change object color
        sphereRenderer.material = active;
    }
}