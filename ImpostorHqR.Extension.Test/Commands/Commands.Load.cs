using Impostor.Api.Net;
using ImpostorHq.Extensions.Api.Interface.Game;

namespace ImpostorHq.Extension.Test.Commands
{
    public partial class Commands
    {
        public static ICommand GetLoadCommand()
        {
            var commandData = new CommandData()
            {
                FinalizeRest = false,
                HasData = false,
                Help = "shows server load.",
                Prefix = "/server-load",
                SplitChar = null,
                TokenCount = 0
            };

            return Proxy.Instance.PreInitialization.CommandApi.Producer.Create(
                commandData,
                (sender, data) => CommandHandler.HandleLoadCommand(sender as IClientPlayer, data),
                null);
        }
    }
}
