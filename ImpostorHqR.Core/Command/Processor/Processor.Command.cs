using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Impostor.Api.Net;
using ImpostorHqR.Extension.Api.Api.Player;

namespace ImpostorHqR.Core.Command.Processor
{
    public class PlayerCommand : IPlayerCommand
    {
        public CommandData Data { get; private set; }

        public string Help => Data.Help;

        public Action<IClientPlayer, string[]> CommandSuccessEvent { get; private set; }

        public Action<IClientPlayer> CommandInvalidSyntaxEvent { get; private set; }

        public PlayerCommand(CommandData data, Action<IClientPlayer, string[]> commandInvoked, Action<IClientPlayer> invalidSyntax)
        {
            data.Prefix = data.Prefix.Trim();

            if (string.IsNullOrEmpty(data.Prefix) || !data.Prefix.StartsWith('/'))
            {
                throw new ArgumentException("A command must start with a forward slash [\"/\"] and must not be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(data.Help) || data.Help.Length < 5)
            {
                throw new ArgumentException("Invalid command help.");
            }

            this.Data = data;
            this.CommandSuccessEvent = commandInvoked;
            this.CommandInvalidSyntaxEvent = invalidSyntax;

            CommandProcessor.RegisterGameCommand(this);
        }

        public void Process(IClientPlayer client, string commandData)
        {
            if (!Data.HasData)
            {
                CommandSuccessEvent?.Invoke(client, null);
                return;
            }

            if (Data.TokenCount == 0)
            {
                CommandSuccessEvent?.Invoke(client, new[] { commandData });
                return;
            }

            if (commandData == null) return;

            if (commandData.Count(c => c == Data.SplitChar) < Data.TokenCount)
            {
                CommandInvalidSyntaxEvent?.Invoke(client);
                return;
            }

            var tokens = new List<string>();
            byte index = 0;

            for (byte i = 0; i < commandData.Length; i++)
            {
                if (index == Data.TokenCount + 1)
                {
                    if (Data.FinalizeRest)
                    {
                        var str = new string(commandData.Skip(i).ToArray());
                        if (!string.IsNullOrWhiteSpace(str)) tokens.Add(str);
                    }
                    break;
                }
                if (commandData[i] != Data.SplitChar)
                {
                    if (tokens.Count - 1 != index) tokens.Add($"{commandData[i]}");
                    else tokens[index] += commandData[i];
                }
                else index++;
            }

            CommandSuccessEvent?.Invoke(client, tokens.ToArray());
        }
    }
}
