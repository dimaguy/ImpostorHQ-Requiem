namespace ImpostorHqR.Extensions.Api.Interface.Export
{
    public interface IPrimitiveBox
    {
        long ReadLong();

        ulong ReadULong();

        double ReadDouble();
    }

    public interface IPrimitiveBoxProvider
    {
        IPrimitiveBox GetLong(ref long val);

        IPrimitiveBox GetULong(ref ulong val);

        IPrimitiveBox GetDouble(ref double val);

    }

    public enum PrimitiveBoxType
    {
        UInt, Int, Double
    }
}
