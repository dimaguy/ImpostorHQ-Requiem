#nullable enable
using Impostor.Api.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;
using ImpostorHqR.Core.Impostor.Chat;
using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Command.Processor
{
    public partial class CommandProcessor : ICommandProcessor
    {
        public static readonly CommandProcessor Global = new CommandProcessor();

        private static readonly List<ICommand> GameCommands = new List<ICommand>();

        private static readonly List<ICommand> DashboardCommands = new List<ICommand>();

        public void RegisterGameCommand(Extensions.Api.Interface.Game.ICommand command)
        {
            lock (GameCommands) AddCommand((ICommand)command, GameCommands);
        }

        public void RegisterDashboardCommand(ICommand command)
        {
            lock (DashboardCommands) AddCommand(command, DashboardCommands);
        }

        private static void AddCommand(ICommand command, List<ICommand> set)
        {
            if (set.Any(cmd => cmd.Data.Prefix.Equals(command.Data.Prefix)))
            {
                throw new ArgumentException("A command with the same prefix exists.");
            }

            set.Add(command);
        }

        public static void ProcessGameChat(IClientPlayer source, string data)
        {
            ProcessResult result;
            lock (GameCommands) result = ProcessText(data, GameCommands);
            switch (result.Error)
            {
                case ProcessError.None: break;
                case ProcessError.NotFound:
                    Task.Run(async () =>
                    {
                        await ChatWriter.Instance.WriteChatTo(source, new ChatMessage()
                        {
                            Color = Color.Red,
                            Messages = new[] { "Unknown command. To get a list of commands, please use /help." },
                            SourceName = "(server)"
                        });
                    });
                    return;
                case ProcessError.NoData:
                    (result.Command as Command)?.CommandInvalidSyntaxEvent?.Invoke(source);
                    return;
            }

            result.Command.Process(source, result.Data != null?new string(result.Data) : null);
        }

        private static ProcessResult ProcessText(string str, IEnumerable<ICommand> set)
        {
            var chars = str.AsSpan();

            var hasData = false;

            var start = chars.IndexOf(' ');
            ReadOnlySpan<char> prefix;
            if (start != -1)
            {
                hasData = true;
                prefix = chars.Slice(0, start);
            }
            else
            {
                var whiteSpace = chars.IndexOf(' ');
                prefix = chars.Slice(0, whiteSpace == -1 ? chars.Length : whiteSpace);
            }

            ICommand? cmd = null;

            foreach (var command in set)
            {
                if (!command.Data.Prefix.AsSpan().SequenceEqual(prefix)) continue;
                cmd = command;
                break;
            }

            if (cmd == null)
            {
                return new ProcessResult()
                {
                    Error = ProcessError.NotFound
                };
            }

            if (cmd.Data.HasData && !hasData) return new ProcessResult() { Error = ProcessError.NoData };
            return new ProcessResult()
            {
                Command = cmd,
                Data = hasData? chars.Slice(prefix.Length) : null,
                HasData = hasData,
                Error = ProcessError.None
            };
        }

        public static ICommand[] GetGameCommands()
        {
            lock (GameCommands) return GameCommands.ToArray();
        }

        public static ICommand[] GetDashboardCommands()
        {
            lock (DashboardCommands) return DashboardCommands.ToArray();
        }
    }
}
