namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public readonly struct FieldServer : IResponseField
    {
        public string Code => "Server: ";

        public string Value { get; }

        public FieldServer(string name) => this.Value = name;

        public string Compile() => string.Concat(Code, Value);
    }
}
