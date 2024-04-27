namespace CommandService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string messsage);
    }
}
