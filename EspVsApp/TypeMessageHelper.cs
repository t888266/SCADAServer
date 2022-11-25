namespace SCADAServer.EspVsApp
{
    public enum TypeMessage
    {
        Error,
        Status,
        Command,
        Value,
    }
    public static class TypeMessageHelper
    {
        public static TypeMessage GetTypeMessage(string message)
        {
            if (message[0].Equals('E'))
            {
                return TypeMessage.Error;
            }
            else if(message[0].Equals('S'))
            {
                return TypeMessage.Status;
            }
            else if(message[0].Equals('C'))
            {
                return TypeMessage.Command;
            }
            return TypeMessage.Value;
        }
    }
}
