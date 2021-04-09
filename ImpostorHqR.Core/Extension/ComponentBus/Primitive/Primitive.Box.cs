using System;
using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Core.Extension.ComponentBus.Primitive
{
    public unsafe class PrimitiveBox : IPrimitiveBox
    {
        private long* Int64Ref { get; }

        private ulong* UInt64Ref { get; }

        private double* DoubleRef { get; }

        public PrimitiveBoxType Type { get; }

        public PrimitiveBox(ref long val)
        {
            fixed (long* ptr = &val) this.Int64Ref = ptr;
            this.Type = PrimitiveBoxType.Int;
        }

        public PrimitiveBox(ref ulong val)
        {
            fixed (ulong* ptr = &val) this.UInt64Ref = ptr;
            this.Type = PrimitiveBoxType.UInt;
        }

        public PrimitiveBox(ref double val)
        {
            fixed (double* ptr = &val) this.DoubleRef = ptr;
            this.Type = PrimitiveBoxType.Double;
        }

        public long ReadLong()
        {
            TypeCheck(PrimitiveBoxType.Int);
            return *Int64Ref;
        }

        public ulong ReadULong()
        {
            TypeCheck(PrimitiveBoxType.UInt);
            return *UInt64Ref;
        }

        public double ReadDouble()
        {
            TypeCheck(PrimitiveBoxType.Double);
            return *DoubleRef;
        }

        private void TypeCheck(PrimitiveBoxType type)
        {
            if (Type != type) throw new Exception($"Invalid type. The box was initialized with {Type}.");
        }
    }
}
