namespace Core.Common.Command
{
    public interface ICommandInvoker
    {
        void Run(string receiverId);
    }
}