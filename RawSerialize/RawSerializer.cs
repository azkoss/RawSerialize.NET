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
		/// <summary>
		/// Serializes the supplied struct in a way that does not produce any data overhead.
		/// </summary>
		/// <typeparam name="T">The type of the struct.</typeparam>
		/// <param name="struct">The struct that should be serialized.</param>
		/// <returns>A byte-array which contains all the raw data of the supplied struct.</returns>
		
		public static
#if USE_UNSAFE
		unsafe
#endif
		byte[] GetRawData<T>(
#if USE_EXTENSIONS
		this 
#endif
		T @struct)
			where T : struct
		{
			return RawSerializer.GetRawDataInternal<T>(@struct, Marshal.SizeOf(typeof(T)));
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
		internal static
#if USE_UNSAFE
		unsafe
#endif
		byte[] GetRawDataInternal<T>(T @struct, int size)
			where T : struct
		{
			byte[] data = new byte[size];
#if USE_UNSAFE
			fixed (byte* ptr = &data[0])
			{
				Marshal.StructureToPtr(@struct, (IntPtr)ptr, false);
			}
			return data;
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
		public static
#if USE_UNSAFE
		unsafe
#endif
		T GetStructFromRawData<T>(
#if USE_EXTENSIONS
	    this 
#endif
	    byte[] rawData)
			where T : struct
		{
			return RawSerializer.GetStructFromRawDataInternal<T>(rawData, 0);
		}

		/// <summary>
		/// Reconstructs a struct of the specified type from the supplied raw data.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin reading the raw struct data.</param>
		/// <param name="rawData">The raw data of the struct that should be reconstructed.</param>
		/// <returns>An instance of the struct specified by the generic parameter which was reconstructed from the supplied raw data.</returns>
		public static
#if USE_UNSAFE
		unsafe
#endif
		T GetStructFromRawData<T>(
#if USE_EXTENSIONS
	    this 
#endif
	    byte[] rawData, int offset)
			where T : struct
		{
			return RawSerializer.GetStructFromRawDataInternal<T>(rawData, offset);
		}

		/// <summary>
		/// Reconstructs a struct of the specified type from the supplied raw data.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="rawData">The raw data of the struct that should be reconstructed.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin reading the raw struct data.</param>
		/// <param name="size">The size of the supplied struct type.</param>
		/// <returns>An instance of the struct specified by the generic parameter which was reconstructed from the supplied raw data.</returns>
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static
#if USE_UNSAFE
		unsafe
#endif
		T GetStructFromRawDataInternal<T>(byte[] rawData, int offset)
			where T : struct
		{
#if USE_UNSAFE
			fixed (byte* ptr = &rawData[offset])
			{
				return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
			}
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