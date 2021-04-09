namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public readonly struct FieldAcceptRanges : IResponseField
    {
        public string Code => "Accept-Ranges: ";

        public string Value { get; }

        public FieldAcceptRanges(string type) => this.Value = type;

        public string Compile() => string.Concat(Code, Value);
    }
}
