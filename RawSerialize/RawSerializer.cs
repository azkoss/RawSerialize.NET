#define USE_UNSAFE //Use unsafe pointers for memory access (faster)
#define USE_INLINING //Use aggressive inlining (usually faster; requires at least .NET 4.5)
#define USE_TASKS //Provide asynchronous methods for the async/await/Task system (requires at least .NET 4.5 or a reference to the Task Parallel Library)

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if USE_TASKS
using System.Threading.Tasks;
#endif

namespace RawSerialize
{
	/// <summary>
	/// Provides methods for (de-)serializing structs in/from their raw data.
	/// </summary>
	public static partial class RawSerializer
	{
		/* Read and write from:
		 * [x] byte[]
		 * [ ] IEnumerable<byte>
		 * [x] Stream
		 * [x] BinaryWriter
		 * [ ] (Stream/Text)-(Reader/-Writer)
		 * [ ] Struct
		 */
		
		
		
		/// <summary>
		/// Serializes the supplied struct in a way that does not produce any data overhead.
		/// </summary>
		/// <typeparam name="T">The type of the struct.</typeparam>
		/// <param name="struct">The struct that should be serialized.</param>
		/// <returns>A byte-array which contains all the raw data of the supplied struct.</returns>
		public static unsafe byte[] GetRawData<T>(T @struct)
			where T : struct
		{
			return RawSerializer.GetRawData<T>(@struct, Marshal.SizeOf(typeof(T)));
		}

		/// <summary>
		/// Serializes the supplied struct in a way that does not produce any data overhead.
		/// </summary>
		/// <typeparam name="T">The type of the struct.</typeparam>
		/// <param name="struct">The struct that should be serialized.</param>
		/// <param name="size">The size of the struct in bytes.</param>
		/// <returns>A byte-array which contains all the raw data of the supplied struct.</returns>
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static unsafe byte[] GetRawData<T>(T @struct, int size)
			where T : struct
		{
			byte[] data = new byte[size];
#if USE_UNSAFE
			TypedReference @ref = __makeref(@struct);
			Marshal.Copy(*((IntPtr*)&@ref), data, 0, size);
#else
			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				Marshal.StructureToPtr(@struct, handle.AddrOfPinnedObject(), false);
			}
			finally
			{
				handle.Free();
			}
#endif
			return data;
		}



		/// <summary>
		/// Reconstructs a struct of the specified type from the supplied raw data.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="rawData">The raw data of the struct that should be reconstructed.</param>
		/// <returns>An instance of the struct specified by the generic parameter which was reconstructed from the supplied raw data.</returns>
		public static unsafe T GetStructFromRawData<T>(byte[] rawData)
			where T : struct
		{
			return RawSerializer.GetStructFromRawData<T>(rawData, Marshal.SizeOf(typeof(T)));
		}

		/// <summary>
		/// Reconstructs a struct of the specified type from the supplied raw data.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="rawData">The raw data of the struct that should be reconstructed.</param>
		/// <param name="size">The size of the supplied struct type.</param>
		/// <returns>An instance of the struct specified by the generic parameter which was reconstructed from the supplied raw data.</returns>
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static unsafe T GetStructFromRawData<T>(byte[] rawData, int size)
			where T : struct
		{
#if USE_UNSAFE
			T @struct = default(T);
			TypedReference @ref = __makeref(@struct);
			Marshal.Copy(rawData, 0, *((IntPtr*)&@ref), Marshal.SizeOf(typeof(T)));
			return @struct;
#else
			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			try
			{
				return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
			}
			finally
			{
				handle.Free();
			}
#endif
		}
	}
}