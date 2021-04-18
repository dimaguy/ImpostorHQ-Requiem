using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImpostorHqR.Extension.Api.Api.Player
{
    public class CommandData
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

        /// <summary>
        /// Shows if the other players should see the command.
        /// </summary>
        public bool SuppressChat { get; set; }

        public CommandData(string prefix, string help)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentException("prefix can't be null.");
            if (string.IsNullOrEmpty(help)) throw new ArgumentException("help can't be null.");

            this.Prefix = prefix;
            this.Help = help;
        }

        public CommandData(string prefix, byte tokens, string help)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentException("prefix can't be null.");
            if (string.IsNullOrEmpty(help)) throw new ArgumentException("help can't be null.");

            this.Prefix = prefix;
            this.HasData = true;
            this.TokenCount = tokens;
            this.Help = help;
        }
    }
}
