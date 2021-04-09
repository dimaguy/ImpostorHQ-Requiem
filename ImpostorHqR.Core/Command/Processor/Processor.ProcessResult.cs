using System;

namespace ImpostorHqR.Core.Command.Processor
{
    public partial class CommandProcessor
    {
        private ref struct ProcessResult
        {
            public ProcessError Error { get; set; }

            public bool HasData { get; set; }

            public ICommand Command { get; set; }

            public ReadOnlySpan<char> Data { get; set; }

        }
    }
}