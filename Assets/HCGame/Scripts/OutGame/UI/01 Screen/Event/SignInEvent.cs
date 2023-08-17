
using Salday.EventBus;

public class SignInEvent : CancelableEventBase{
	    public SignInType signInType;
    public SignInEvent(SignInType signInType)
    {
	this.signInType = signInType;
    }
}

public enum SignInType
{
    Google,
    Apple,
    FNCY,
    Guets,
}