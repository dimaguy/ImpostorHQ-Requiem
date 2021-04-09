#nullable enable
using System;
using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Command.Processor
{
    public interface ICommand
    {
        CommandData Data { get; }

        void Process(object client, string? data);

        string Help { get; }
    }
}