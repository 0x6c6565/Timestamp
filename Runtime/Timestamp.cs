using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Timestamp
{
    using DateTimeFormatInfo = System.Globalization.DateTimeFormatInfo;

    /// <summary>https://referencesource.microsoft.com/#mscorlib/system/datetime.cs</summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = sizeof(UInt64))]
    public struct Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>, IComparable<DateTime>, IFormattable
    {
        public const string iso8601Format = "yyyyMMddTHH:mm:ssZ";
        public const string iso8601HyphenatedFormat = "yyyy-MM-ddTHH:mm:ssZ";

        [FieldOffset(0)] public DateTime dateTime; // Same size as UInt64 (8B).
        [FieldOffset(0)] public UInt64 uint64;

        public Timestamp(DateTime dateTime) : this() { this.dateTime = dateTime; }

        public static Timestamp Now
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Timestamp { dateTime = DateTime.Now };
        }

        public static Timestamp UtcNow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Timestamp { dateTime = DateTime.UtcNow };
        }

        public Timestamp Add(TimeSpan value) { return new Timestamp { dateTime = dateTime.Add(value) }; }
        public static Timestamp operator +(Timestamp timestamp, TimeSpan timeSpan) { return timestamp.Add(timeSpan); }

        public override bool Equals(object obj) { return obj is Timestamp timestamp && Equals(timestamp); }
        public bool Equals(Timestamp other) { return dateTime == other.dateTime; }

        public override int GetHashCode() { return -71619280 + dateTime.GetHashCode(); }

        public static implicit operator DateTime(Timestamp timestamp) { return timestamp.dateTime; }
        public static implicit operator Timestamp(DateTime dateTime) { return new Timestamp { dateTime = dateTime }; }

        public static bool operator ==(Timestamp left, Timestamp right) { return left.Equals(right); }
        public static bool operator !=(Timestamp left, Timestamp right) { return !(left == right); }

        public static bool operator <(Timestamp left, Timestamp right) { return left.dateTime < right.dateTime; }
        public static bool operator >(Timestamp left, Timestamp right) { return left.dateTime > right.dateTime; }
        public static bool operator <=(Timestamp left, Timestamp right) { return left.dateTime <= right.dateTime; }
        public static bool operator >=(Timestamp left, Timestamp right) { return left.dateTime >= right.dateTime; }

        public static TimeSpan operator -(Timestamp left, Timestamp right) { return left.dateTime - right.dateTime; }

        /// <summary>https://docs.microsoft.com/en-us/dotnet/api/system.datetime?view=net-5.0#formatting-05</summary>
        /// <returns>An IS08601 web service format (<see cref="iso8601Format"/>)</returns>
        public override string ToString() { return dateTime.ToString(iso8601Format); }
        public string ToString(string format) { return ToString(format, DateTimeFormatInfo.CurrentInfo); }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return dateTime.ToString(format, formatProvider);
        }

        public int CompareTo(Timestamp other) { return dateTime.CompareTo(other.dateTime); }
        public int CompareTo(DateTime other) { return dateTime.CompareTo(other); }

        public static float Parameterize(Timestamp a, Timestamp b, Timestamp at)
        {
            if (a.dateTime == b.dateTime)
                return 0;

            TimeSpan numerator = at.dateTime - a.dateTime;
            TimeSpan denominator = b.dateTime - a.dateTime;

            return (float)(numerator.TotalSeconds / denominator.TotalSeconds);
        }
    }
}
