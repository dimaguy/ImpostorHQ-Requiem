#nullable enable
using Impostor.Api.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Extension.Api.Api.Player;
using ImpostorHqR.Extension.Api.Configuration;
using ImpostorHqR.Extension.Api.Utils.Chat;

namespace ImpostorHqR.Core.Command.Processor
{
    struct ProcessResult
    {
        public enum ProcessError
        {
            None,
            NoData,
            NotFound
        }

        public ProcessError Error { get; set; }

        public bool HasData { get; set; }

        public IPlayerCommand Command { get; set; }

        public string? Data { get; set; }

    }

    public static class CommandProcessor
    {
        private static readonly ConcurrentDictionary<string, IPlayerCommand> GameCommands = new ConcurrentDictionary<string, IPlayerCommand>();

        public static void Start()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            // ReSharper disable once PossibleNullReferenceException
            typeof(IPlayerCommand).GetField("_Provider", flags).SetValue(null, new Converter<(CommandData data, Action<IClientPlayer, string[]> commandInvoked, Action<IClientPlayer> invalidSyntax), IPlayerCommand>(PlayerCommandProvider));
        }

        private static IPlayerCommand PlayerCommandProvider((CommandData data, Action<IClientPlayer, string[]> commandInvoked, Action<IClientPlayer> invalidSyntax) input)
        {
            return new PlayerCommand(input.data, input.commandInvoked, input.invalidSyntax);
        }

        public static void RegisterGameCommand(IPlayerCommand command)
        {
            AddCommand(command, GameCommands);
        }

        private static void AddCommand(IPlayerCommand command, ConcurrentDictionary<string, IPlayerCommand> set)
        {
            if (!set.TryAdd(command.Data.Prefix, command))
            {
                throw new ArgumentException($"A command with the prefix of \"{command.Data.Prefix}\" already exists.");
            }
        }

        public static async void ProcessGameChat(IClientPlayer source, string data)
        {
            var result = ProcessText(data, GameCommands);
            switch (result.Error)
            {
                case ProcessResult.ProcessError.None: break;
                case ProcessResult.ProcessError.NotFound:
                    await ChatWriter.WriteChatTo(source, "(requiem)",
                        new[] {"Unknown command. To get a list of commands, please use /help."});
                    return;
                case ProcessResult.ProcessError.NoData: 
                    (result.Command as PlayerCommand)?.CommandInvalidSyntaxEvent?.Invoke(source);
                    return;
            }

            result.Command.Process(source, result.Data != null?new string(result.Data) : null);
        }

        private static unsafe ProcessResult ProcessText(string str, ConcurrentDictionary<string, IPlayerCommand> set)
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

            var found = false;
            IPlayerCommand? cmd;
            fixed (char* pointer = &prefix[0])
            { 
                found = set.TryGetValue(new string(pointer, 0, prefix.Length), out var command);
                if (found)
                {
                    Trace.Assert(command != null, "Command was null after successful lookup! L89 Processor");
                    cmd = command!;
                }
                else cmd = null;
            }


            if (!found)
            {
                return new ProcessResult()
                {
                    Error = ProcessResult.ProcessError.NotFound
                };
            }

            if (cmd!.Data.HasData && !hasData) return new ProcessResult() { Error = ProcessResult.ProcessError.NoData };
            string? data = null;

            if (hasData)
            {
                var span = chars.Slice(prefix.Length);
                fixed (char* ptr = &span[0])
                {
                    data = new string(ptr, 0, span.Length);
                }
            }

            return new ProcessResult()
            {
                Command = cmd,
                Data = data,
                HasData = hasData,
                Error = ProcessResult.ProcessError.None
            };
        }

        public static IEnumerable<IPlayerCommand> GetGameCommands()
        {
            return GameCommands.Select(kvp=>kvp.Value);
        }
    }
}
