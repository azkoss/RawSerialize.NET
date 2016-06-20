using System;
using System.Runtime.InteropServices;

namespace RawSerialize.Test
{
    /// <summary>
    /// A struct used internally for unit testing.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct TestStruct
        : IEquatable<TestStruct>
    {
        public TestStruct(bool a, byte b, int c, double d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public bool A;
        public byte B;
        public int C;
        public double D;

        public static bool operator ==(TestStruct left, TestStruct right)
        {
            return left.A == right.A && left.B == right.B && left.C == right.C && left.D == right.D;
        }

        public static bool operator !=(TestStruct left, TestStruct right)
        {
            return !(left == right);
        }

        public bool Equals(TestStruct other)
        {
            return this == other;
        }
    }
}
