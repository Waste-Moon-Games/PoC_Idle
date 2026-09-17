namespace Core.Common.Command.Shop
{
    public class ChangeShopViewStateCommand : Command
    {
        private readonly ICommandReceiver _receiverToClose;
        private readonly ICommandReceiver _receiverToOpen;

        public ChangeShopViewStateCommand(ICommandReceiver receiverToClose, ICommandReceiver receiverToOpen)
        {
            _receiverToClose = receiverToClose;
            _receiverToOpen = receiverToOpen;

            AddReceiverInQueue(_receiverToClose);
            AddReceiverInQueue(_receiverToOpen);
        }
    }
}