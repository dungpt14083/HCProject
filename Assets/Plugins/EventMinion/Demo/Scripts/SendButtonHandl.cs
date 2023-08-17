using UnityEngine;
using UnityEngine.UI;
using Assets.Demo;

public class SendButtonHandl : MonoBehaviour
{
    #region Editor Vars

    [SerializeField]
    UnityAdoptedEventBus evBus;

    [SerializeField]
    InputField field;

    #endregion

    //This method is called from unity, when button next to the input field is clicked
    public void PublishMessage()
    {
        var resetArg = new ResetSpheres();

        //Create Chat Event Arguments, that will hold data (message)
        var arg = new ChatEvArgs()
        {
            Message = field.text
        };

        //Clear input field
        field.text = string.Empty;

        //Reset all spheres and tooltips
        evBus.Publish(resetArg);

        //Publish message to event bus. It will distribute event data among hanlers
        evBus.Publish(arg);

        field.Select();
        
    }

    private void FixedUpdate()
    {
        // Call publish message by pressing return
        if (Input.GetKeyDown(KeyCode.Return))
            PublishMessage();
    }
}
