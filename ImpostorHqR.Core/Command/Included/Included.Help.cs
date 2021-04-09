using Impostor.Api.Net;
using System.Drawing;
using System.Threading.Tasks;
using ImpostorHqR.Core.Command.Processor;
using ImpostorHqR.Core.Impostor.Chat;
using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;
using ImpostorHqR.Core.Services;
using ImpostorHqR.Extensions.Api.Interface.Game;
using ICommand = ImpostorHqR.Extensions.Api.Interface.Game.ICommand;

namespace ImpostorHqR.Core.Command.Included
{
    internal class HelpCommandRegister : IService
    {
        private static CommandData Data { get; set; }

        public void PostInit()
        {
            Data = new CommandData()
            {
                FinalizeRest = false,
                HasData = false,
                Prefix = "/help",
                TokenCount = 0,
                Help = "This will display information about the currently registered commands."
            };
            var playerCommand = new Processor.Command(Data, PlayerCommandInvoked, null);
            CommandProcessor.Global.RegisterGameCommand((Extensions.Api.Interface.Game.ICommand)playerCommand);

            //TODO: add dashboard register
        }

        private void PlayerCommandInvoked(object clientObj, string[] tokens)
        {
            if (!(clientObj is IClientPlayer player)) return;

            Task.Run(async () =>
            {
                await ChatWriter.Instance.WriteChatTo(player, new ChatMessage()
                {
                    Color = Color.Aqua,
                    Messages = new[] { CreateHelp(CommandProcessor.GetGameCommands()) },
                    SourceName = "(server)"
                });
            });
        }

        private static string CreateHelp(Processor.ICommand[] commands)
        {
            using var sb = StringBuilderPool.Instance.Get();
            foreach (var gameCommand in commands)
            {
                sb.AppendLine($"{gameCommand.Data.Prefix} -> {gameCommand.Data.Help}");
            }

            var result = sb.ToString();
            return result;
        }

        public void Activate() { }

        public void Shutdown() { }
    }
}
