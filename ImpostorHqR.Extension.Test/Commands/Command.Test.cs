using Impostor.Api.Net;
using ImpostorHq.Extensions.Api.Interface.Game;

namespace ImpostorHq.Extension.Test.Commands
{
    public partial class Commands
    {
        public static ICommand GetTestCommand()
        {
            var commandData = new CommandData()
            {
                FinalizeRest = false,
                HasData = false,
                Help = "test command from test extension.",
                Prefix = "/test-ext",
                SplitChar = null,
                TokenCount = 0
            };

            return Proxy.Instance.PreInitialization.CommandApi.Producer.Create(
                commandData,
                (sender, data) => CommandHandler.HandleTestCommand(sender as IClientPlayer, data),
                null);
        }
    }
}
