namespace Cultivation.Shared.Exception;

public class MethodNotAllowedException : System.Exception
{
    public MethodNotAllowedException(string message) : base(message) { }
}
