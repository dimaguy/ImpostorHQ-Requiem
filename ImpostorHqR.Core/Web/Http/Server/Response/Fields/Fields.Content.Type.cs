namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public readonly struct FieldContentType : IResponseField
    {
        public string Code => "Content-Type: ";

        public string Value { get; }

        public FieldContentType(string mime) => this.Value = mime;

        public string Compile() => string.Concat(Code, Value);
    }
}
