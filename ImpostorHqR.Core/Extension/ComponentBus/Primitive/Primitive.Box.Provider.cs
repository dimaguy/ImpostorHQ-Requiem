using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Core.Extension.ComponentBus.Primitive
{
    public class PrimitiveBoxProducer : IPrimitiveBoxProvider
    {
        public static readonly IPrimitiveBoxProvider Instance = new PrimitiveBoxProducer();

        public IPrimitiveBox GetDouble(ref double val) => new PrimitiveBox(ref val);

        public IPrimitiveBox GetLong(ref long val) => new PrimitiveBox(ref val);

        public IPrimitiveBox GetULong(ref ulong val) => new PrimitiveBox(ref val);
    }
}
