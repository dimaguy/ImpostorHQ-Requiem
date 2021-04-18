namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public readonly struct FieldAuthentication : IResponseField
    {
        public string Mode { get; }

        public string Realm { get; }

        public FieldAuthentication(string mode, string realm)
        {
            this.Mode = mode;
            this.Realm = realm;
        }

        public string Compile() => $"WWW-Authenticate: {Mode} realm={Realm}";
    }
}
