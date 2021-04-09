using System;

namespace ImpostorHqR.Extensions.Api.Interface.Game
{
    /// <summary>
    /// This system will allow you to easily register game and dashboard commands.
    /// </summary>
    public interface ICommandApi
    {
        ICommandProcessor Processor { get; }

        ICommandProducer Producer { get; }
    }

    /// <summary>
    /// This system processes commands that you register.
    /// </summary>
    public interface ICommandProcessor
    {
        void RegisterGameCommand(ICommand command);
    }

    /// <summary>
    /// This object is an abstracted holder for command data. It is obtained from the ICommandProducer's Create() function.
    /// </summary>
    public interface ICommand
    {

    }

    /// <summary>
    /// Produces ICommand instances. 
    /// </summary>
    public interface ICommandProducer
    {
        /// <summary>
        /// This will generate commands.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="invoker">The function to invoke when the command is successfully parsed. It passes a client object.</param>
        /// <param name="syntaxError">The function to invoke when the command is invoked, but the parameters do not match the definition. It passes a client object.</param>
        /// <returns></returns>
        ICommand Create(CommandData data, Action<object, string[]> invoker, Action<object> syntaxError);
    }

    public struct CommandData
    {
        /// <summary>
        /// If not true, the command only requires the prefix to be sent in order to trigger the event. A null array will be passed.
        /// </summary>
        public bool HasData { get; set; }
        /// <summary>
        /// The command's prefix. Must start with ["/"]. Example: /cmd
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// How many split operations to do.
        /// </summary>
        public byte TokenCount { get; set; }
        /// <summary>
        /// The character to split with (usually a space).
        /// </summary>
        public char? SplitChar { get; set; }
        /// <summary>
        /// If true, the last item of the token array will be the leftover text(if any). Please refer to the examples for that/
        /// </summary>
        public bool FinalizeRest { get; set; }
        /// <summary>
        /// Mandatory data. Describe the usage of your command here.
        /// </summary>
        public string Help { get; set; }
    }
}
