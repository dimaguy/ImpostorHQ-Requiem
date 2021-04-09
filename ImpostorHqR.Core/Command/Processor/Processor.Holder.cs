using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Command.Processor
{
    public class CommandApiHolder : ICommandApi
    {
        public static readonly ICommandApi Instance = new CommandApiHolder();

        public ICommandProducer Producer => CommandProducer.Instance;

        ICommandProcessor ICommandApi.Processor => CommandProcessor.Global;
    }
}
