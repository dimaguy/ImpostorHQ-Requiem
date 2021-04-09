namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public interface IResponseField
    {
        string Code { get; }

        string Value { get; }

        string Compile();
    }
}
