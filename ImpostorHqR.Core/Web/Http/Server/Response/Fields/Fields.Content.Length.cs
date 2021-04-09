using System;

namespace ImpostorHqR.Core.Web.Http.Server.Response.Fields
{
    public readonly struct FieldContentLength : IResponseField
    {
        public string Code => "Content-Length: ";

        public string Value => Convert.ToString(Size);

        public int Size { get; }

        public FieldContentLength(int length) => this.Size = length;

        public string Compile() => string.Concat(Code, Value);
    }
}
