using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RawSerialize
{
    public static partial class RawSerializer
    {
        /// <summary>
        /// Writes the raw data of the supplied struct to this BinaryWriter.
        /// </summary>
        /// <typeparam name="T">The type of the struct.</typeparam>
        /// <param name="writer">The BinaryWriter that should be written to</param>
        /// <param name="struct">The struct that should be written to the BinaryWriter.</param>
        public static void WriteRawData<T>(
#if USE_EXTENSIONS
    this 
#endif
    BinaryWriter writer, T @struct)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = RawSerializer.GetRawDataInternal<T>(@struct, size);
            writer.Write(data, 0, size);
        }



        /// <summary>
        /// Reads the raw data of a struct of the supplied type from this BinaryReader and reconstructs the instance.
        /// </summary>
        /// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
        /// <param name="reader">The BinaryReader that should be read from.</param>
        /// <returns>An instance of the struct specified by the generic parameter which was reconstructed from the supplied raw data.</returns>
        public static T ReadStructFromRawData<T>(
#if USE_EXTENSIONS
    this 
#endif
    BinaryReader reader)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] rawData = new byte[size];
            reader.Read(rawData, 0, size);
            return RawSerializer.GetStructFromRawDataInternal<T>(rawData, 0);
        }
    }
}
