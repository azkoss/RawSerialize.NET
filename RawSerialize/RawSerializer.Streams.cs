#define USE_UNSAFE //Use unsafe pointers for memory access (faster)
#define USE_INLINING //Use aggressive inlining (usually faster; requires at least .NET 4.5)
#define USE_TASKS //Provide asynchronous methods for the async/await/Task system (requires at least .NET 4.5 or a reference to the Task Parallel Library)

using System;
using System.IO;
using System.Runtime.InteropServices;

#if USE_TASKS
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RawSerialize
{
	public static partial class RawSerializer
	{
		/// <summary>
		/// Writes the raw data of the supplied struct to this stream.
		/// </summary>
		/// <typeparam name="T">The type of the struct.</typeparam>
		/// <param name="stream">The stream that should be written to.</param>
		/// <param name="struct">The struct that should be written to the stream.</param>
		public static void WriteRawData<T>(this Stream stream, T @struct)
			where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] data = RawSerializer.GetRawData<T>(@struct, size);
			stream.Write(data, 0, size);
		}

#if USE_TASKS
		/// <summary>
		/// Writes the raw data of the supplied struct to this stream asynchronously.
		/// </summary>
		/// <typeparam name="T">The type of the struct.</typeparam>
		/// <param name="stream">The stream that should be written to.</param>
		/// <param name="struct">The struct that should be written to the stream.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		public static async Task WriteRawDataAsync<T>(this Stream stream, T @struct)
			where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] data = RawSerializer.GetRawData<T>(@struct, size);
			await stream.WriteAsync(data, 0, size);
		}

		/// <summary>
		/// Writes the raw data of the supplied struct to this stream asynchronously and monitors cancellation requests.
		/// </summary>
		/// <typeparam name="T">The type of the struct.</typeparam>
		/// <param name="stream">The stream that should be written to.</param>
		/// <param name="struct">The struct that should be written to the stream.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		public static async Task WriteRawDataAsync<T>(this Stream stream, T @struct, CancellationToken cancellationToken)
			where T : struct
		{
			cancellationToken.ThrowIfCancellationRequested();
			int size = Marshal.SizeOf(typeof(T));
			byte[] data = RawSerializer.GetRawData<T>(@struct, size);
			await stream.WriteAsync(data, 0, size, cancellationToken);
		}
#endif



#if USE_TASKS
		/// <summary>
		/// Reads the raw data of a struct of the supplied type from this stream asynchronously and reconstructs the instance.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="stream">The stream that should be read from.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the reconstructed struct.</returns>
		public static async Task<T> ReadStructFromRawDataAsync<T>(this Stream stream)
			where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] rawData = new byte[size];
			await stream.ReadAsync(rawData, 0, size);
			return RawSerializer.GetStructFromRawData<T>(rawData);
		}

		/// <summary>
		/// Reads the raw data of a struct of the supplied type from this stream asynchronously, reconstructs the instance and monitors cancellation requests.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="stream">The stream that should be read from.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the reconstructed struct.</returns>
		public static async Task<T> ReadStructFromRawDataAsync<T>(this Stream stream, CancellationToken cancellationToken)
			where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] rawData = new byte[size];
			await stream.ReadAsync(rawData, 0, size, cancellationToken);
			return RawSerializer.GetStructFromRawData<T>(rawData);
		}
#endif

		/// <summary>
		/// Reads the raw data of a struct of the supplied type from this stream and reconstructs the instance.
		/// </summary>
		/// <typeparam name="T">The type of the struct that should be reconstructed.</typeparam>
		/// <param name="stream">The stream that should be read from.</param>
		/// <returns>An instance of the struct specified by the generic parameter which was reconstructed from the supplied raw data.</returns>
		public static T ReadStructFromRawData<T>(this Stream stream)
			where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] rawData = new byte[size];
			stream.Read(rawData, 0, size);
			return RawSerializer.GetStructFromRawData<T>(rawData);
		}
	}
}