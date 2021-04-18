using Impostor.Api.Net;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ImpostorHqR.Core.Command.Processor;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Player;
using ImpostorHqR.Extension.Api.Utils.Chat;

namespace ImpostorHqR.Core.Command.Included
{ 
    internal static class HelpCommandRegister
    {
        private static CommandData Data { get; set; }

        public static void Register()
        {
            Data = new CommandData("/help", "This will display information about the currently registered commands.");
            IPlayerCommand.Create(Data, PlayerCommandInvoked, null);
        }

        private static async void PlayerCommandInvoked(IClientPlayer clientObj, string[] tokens)
        {
            await ChatWriter.WriteChatTo(clientObj, "(requiem)",
                new[] {CreateHelp(CommandProcessor.GetGameCommands().ToArray())});
        }

        private static string CreateHelp(IPlayerCommand[] commands)
        {
            using var sb = IReusableStringBuilder.Get();
            foreach (var gameCommand in commands)
            {
                sb.AppendLine($"{gameCommand.Data.Prefix} -> {gameCommand.Data.Help}");
            }

            var result = sb.ToString();
            return result;
        }
    }
}
