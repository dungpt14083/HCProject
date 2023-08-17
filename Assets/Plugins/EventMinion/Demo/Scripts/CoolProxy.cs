using Salday.EventBus;
using Assets.Demo;
using UnityEngine;

public class CoolProxy : UnityEventProxyBase
{
    #region Inspector Vars
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

    [Handler(HandlerPriority.Medium)]
    public void Kk(ChatEvArgs arg)
    {
        //Don't want to do anything, if event is already canceled
        if (arg.Canceled) return;
        if (arg.Message.Contains(":)")) //Check if message contains desired substring
        {
            //Change text of the message, further handlers will receive updated event argument
            arg.Message = arg.Message.Replace(":)", "XD");

            //Visual representation
            ActivateSphere();
            EnableAndSetTooltip("Replaced all ':)' in the \nmessage with 'XD'");
        }
        else
            EnableAndSetTooltip("No :) found, continuing");
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

    void ActivateSphere()
    {
        sphereRenderer.material = active; //Change object color
    }
}
