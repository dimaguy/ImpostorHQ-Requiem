namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public readonly struct FieldContentRange : IResponseField
    {
        public string Code => "Content-Range: ";

        public string Value { get; }

        public FieldContentRange(bool useRange, long? start, long? end, long fileSize)
        {
            /*  ♡ Mozilla
             *  Content-Range: <unit> <range-start>-<range-end>/<size>
             *  Content-Range: <unit> <range-start>-<range-end>/*
             *  Content-Range: <unit> * /<size>
             */

            this.Value = useRange ? $"bytes {start}-{end}/{fileSize}" : $"bytes */{fileSize}";
        }

        public string Compile() => string.Concat(Code, Value);
    }
}
