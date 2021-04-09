using System;
using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Command.Processor
{
    public class CommandProducer : ICommandProducer
    {
        public static readonly ICommandProducer Instance = new CommandProducer();

        public Extensions.Api.Interface.Game.ICommand Create(CommandData data, Action<object, string[]> invoker, Action<object> syntaxError)
        {
            return new Command(data, invoker, syntaxError);
        }
    }
}
