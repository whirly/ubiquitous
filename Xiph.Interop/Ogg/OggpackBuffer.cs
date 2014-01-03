/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2002             *
 * by the Xiph.Org Foundation http://www.xiph.org/                  *
 *                                                                  *
 ********************************************************************

 function: toplevel libogg include

 ********************************************************************/

/* C#/.NET interop-port
 * 
 * Copyright 2004 Klaus Prückl <klaus.prueckl@aon.at>
 */

using System;
using System.Runtime.InteropServices;

namespace Xiph.Interop.Ogg
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct oggpack_buffer
	{
		internal int	endbyte;
		internal int	endbit;

		internal IntPtr	buffer;	// unsigned char *
		internal IntPtr	ptr;	// unsigned char *
		internal int	storage;
	}

	/// <summary>
	/// The OggpackBuffer class is used with libogg's bitpacking functions.
	/// You should never need to directly access anything in this class. 
	/// </summary>
	public class OggpackBuffer
	{
		#region Variables
		internal oggpack_buffer oggpack_buffer = new oggpack_buffer();
		private bool memAllocated = false;
		internal bool changed = true;
		private byte[] buffer = null;
		#endregion

		#region Constructor(s) and Destructor
		/// <summary>
		/// </summary>
		~OggpackBuffer() 
		{
			if(this.memAllocated)
				Marshal.FreeHGlobal(this.oggpack_buffer.buffer);
		}
		#endregion

		#region Properties
//		public int Endbyte 
//		{
//			get { return this.oggpack_buffer.endbyte; }
//		}
//		public int Endbit 
//		{
//			get { return this.oggpack_buffer.endbit; }
//		}
		/// <summary>
		/// Data being manipulated.
		/// </summary>
		public byte[] Buffer 
		{
			get 
			{
				if(this.changed) 
				{
					int bytes = OggpackBuffer.Bytes(this);
					buffer = new byte[bytes];
					Marshal.Copy(this.oggpack_buffer.buffer, buffer, 0, bytes);
					this.changed = false;
				}
				return buffer;
			}
		}
		/// <summary>
		/// Size of buffer.
		/// </summary>
		internal int Storage 
		{
			get { return this.oggpack_buffer.storage; }
		}
		#endregion

		#region Ogg BITSTREAM PRIMITIVES: bitstream ************************

		#region oggpack_writeinit
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_writeinit(
			[In,Out]	ref oggpack_buffer	b);
		/// <summary>
		/// This function initializes an <see cref="OggpackBuffer"/> for
		/// writing using the Ogg bitpacking functions.
		/// </summary>
		/// <param name="b">
		/// Buffer to be used for writing. This is an ordinary data buffer with
		/// some extra markers to ease bit navigation and manipulation.
		/// </param>
		public static void Writeinit(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_writeinit(ref b.oggpack_buffer);
			b.changed = true;
		}
		#endregion

		#region oggpack_reset
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_reset(
			[In,Out]	ref oggpack_buffer	b);
		/// <summary>
		/// This function resets the contents of an <see cref="OggpackBuffer"/>
		/// to their original state but does not free the memory used.
		/// </summary>
		/// <param name="b">
		/// <see cref="OggpackBuffer"/> to be reset.
		/// </param>
		public static void Reset(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_reset(ref b.oggpack_buffer);
			b.changed = true;
		}
		#endregion

		#region oggpack_writetrunc
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_writetrunc(
			[In,Out]	ref oggpack_buffer	b,
			int bits);
		/// <summary>
		/// This function truncates an already written-to
		/// <see cref="OggpackBuffer"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="OggpackBuffer"/> must already be initialized for
		/// writing using <see cref="Writeinit"/>.
		/// </remarks>
		/// <param name="b">
		/// Buffer to be truncated.
		/// </param>
		/// <param name="bits">
		/// Number of bits to keep in the buffer (size after truncation).
		/// </param>
		public static void Writetrunc(OggpackBuffer b, int bits) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_writetrunc(ref b.oggpack_buffer, bits);
			b.changed = true;
		}
		#endregion

		#region oggpack_writealign
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_writealign(
			[In,Out]	ref oggpack_buffer	b);
		/// <summary>
		/// This function pads the <see cref="OggpackBuffer"/> with zeros out
		/// to the next byte boundary.
		/// </summary>
		/// <remarks>
		/// The <see cref="OggpackBuffer"/> must already be initialized for
		/// writing using <see cref="Writeinit"/>.
		/// <para>
		/// Only 32 bits can be written at a time.
		/// </para>
		/// </remarks>
		/// <param name="b">Buffer to be used for writing.</param>
		public static void Writealign(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_writealign(ref b.oggpack_buffer);
			b.changed = true;
		}
		#endregion

		#region oggpack_writecopy TODO
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_writecopy(
			[In,Out] ref oggpack_buffer b, IntPtr source, int bits); // void *
//		/// <summary>
//		/// This function copies a sequence of bits from a source buffer into
//		/// an <see cref="OggpackBuffer"/>.
//		/// </summary>
//		/// <remarks>
//		/// The <see cref="OggpackBuffer"/> must already be initialized for
//		/// writing using <see cref="Writeinit"/>. 
//		/// <para>
//		/// Only 32 bits can be written at a time.
//		/// </para>
//		/// </remarks>
//		/// <param name="b">
//		/// Buffer to be used for writing.
//		/// </param>
//		/// <param name="source">
//		/// A pointer to the data to be written into the buffer.
//		/// </param>
//		/// <param name="bits">
//		/// The number of bits to be copied into the buffer.
//		/// </param>
//		public static void Writecopy(OggpackBuffer b, IntPtr source, int bits) 
//		{
//			throw new NotImplementedException();
//
//			if(b == null)
//				throw new ArgumentException("OggpackBuffer b cannot be null!");
//			oggpack_writecopy(b, source, bits);
//			b.changed = true;
//		}
		#endregion

		#region oggpack_writeclear
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_writeclear(
			[In,Out] ref oggpack_buffer b);
		/// <summary>
		/// This function clears the buffer after writing and frees the memory
		/// used by the <see cref="OggpackBuffer"/>. 
		/// </summary>
		/// <param name="b">
		/// Our <see cref="OggpackBuffer"/>. This is an ordinary data buffer
		/// with some extra markers to ease bit navigation and manipulation.
		/// </param>
		public static void Writeclear(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_writeclear(ref b.oggpack_buffer);
			b.changed = true;
		}
		#endregion

		#region oggpack_readinit TODO
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_readinit(
			[In,Out] ref oggpack_buffer b, IntPtr buf, int bytes); // unsigned char *
		/// <summary>
		/// This function takes an ordinary buffer and prepares an
		/// <see cref="OggpackBuffer"/> for reading using the Ogg bitpacking
		/// functions. 
		/// </summary>
		/// <param name="b">
		/// <see cref="OggpackBuffer"/> class object to be initialized with
		/// some extra markers to ease bit navigation and manipulation.
		/// </param>
		/// <param name="buf">
		/// Original data buffer, to be inserted into the
		/// <see cref="OggpackBuffer"/> so that it can be read using bitpacking
		/// functions.
		/// </param>
		/// <param name="bytes"></param>
		public static void Readinit(OggpackBuffer b, byte[] buf, int bytes) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			if(b.memAllocated)
				Marshal.FreeHGlobal(b.oggpack_buffer.buffer);
			else
				b.memAllocated = true;
			IntPtr pbuf = Marshal.AllocHGlobal(bytes);
			Marshal.Copy(buf, 0, pbuf, bytes);
			oggpack_readinit(ref b.oggpack_buffer, pbuf, bytes);
			b.changed = true;
		}
		#endregion

		#region oggpack_write
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_write(
			[In,Out] ref oggpack_buffer b, uint value, int bits);
		/// <summary>
		/// This function writes bits into an <see cref="OggpackBuffer"/>.
		/// </summary>
		/// <remarks>
		/// The OggpackBuffer must already be initialized for writing using
		/// <see cref="Writeinit"/>.
		/// <para>
		/// Only 32 bits can be written at a time.
		/// </para> 
		/// </remarks>
		/// <param name="b">
		/// Buffer to be used for writing.
		/// </param>
		/// <param name="value">
		/// The data to be written into the buffer. This must be 32 bits or
		/// fewer.
		/// </param>
		/// <param name="bits">
		/// The number of bits being written into the buffer.
		/// </param>
		public static void Write(OggpackBuffer b, uint value, int bits) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
//			if(value > 32)
//				throw new ArgumentException("Uint value must be <=32!");
			oggpack_write(ref b.oggpack_buffer, value, bits);
			b.changed = true;
		}
		#endregion

		#region oggpack_look
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpack_look(
			[In]	ref oggpack_buffer	b,
			int bits);
		/// <summary>
		/// This function looks at a specified number of bits inside the buffer
		/// without advancing the location pointer. 
		/// </summary>
		/// <remarks>
		/// The specified number of bits are read, starting from the location
		/// pointer.
		/// <para>
		/// This function can be used to read 32 or fewer bits.
		/// </para>
		/// </remarks>
		/// <param name="b">
		/// Pointer to <see cref="OggpackBuffer"/> to be read.
		/// </param>
		/// <param name="bits">
		/// Number of bits to look at. For this function, must be 32 or fewer. 
		/// </param>
		/// <returns>Represents the requested bits.</returns>
		public static int Look(OggpackBuffer b, int bits) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			int retval = oggpack_look(ref b.oggpack_buffer, bits);
			return retval;
		}
		#endregion

		#region oggpack_look1
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpack_look1(
			[In] ref oggpack_buffer b);
		/// <summary>
		/// This function looks at the next bit without advancing the location
		/// pointer.
		/// </summary>
		/// <remarks>
		/// The next bit is read starting from the location pointer. 
		/// </remarks>
		/// <param name="b">
		/// <see cref="OggpackBuffer"/> class containing our buffer.
		/// </param>
		/// <returns>
		/// Represents the value of the next bit after the location pointer.
		/// </returns>
		public static int Look1(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			int retval = oggpack_look1(ref b.oggpack_buffer);
			return retval;
		}
		#endregion

		#region oggpack_adv
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_adv(
			[In,Out]	ref oggpack_buffer	b,
			int bits);
		/// <summary>
		/// This function advances the location pointer by the specified number
		/// of bits without reading any data.
		/// </summary>
		/// <param name="b">The current <see cref="OggpackBuffer"/>.</param>
		/// <param name="bits">Number of bits to advance.</param>
		public static void Adv(OggpackBuffer b, int bits) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_adv(ref b.oggpack_buffer, bits);
			b.changed = true;
		}
		#endregion

		#region oggpack_adv1
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpack_adv1(
			[In,Out]	ref oggpack_buffer	b);
		/// <summary>
		/// This function advances the location pointer by one bit without
		/// reading any data.
		/// </summary>
		/// <param name="b">The current <see cref="OggpackBuffer"/>.</param>
		public static void Adv1(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			oggpack_adv1(ref b.oggpack_buffer);
			b.changed = true;
		}
		#endregion

		#region oggpack_read
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpack_read(
			[In,Out] ref oggpack_buffer b, int bits);
		/// <summary>
		/// This function reads the requested number of bits from the buffer
		/// and advances the location pointer.
		/// </summary>
		/// <remarks>
		/// Before reading, the buffer should be initialized using
		/// <see cref="Readinit"/>.
		/// </remarks> 
		/// <param name="b">
		/// An <see cref="OggpackBuffer"/> class containing buffered data to be
		/// read.
		/// </param>
		/// <param name="bits">Number of bits to read.</param>
		/// <returns>Represents the requested bits.</returns>
		public static int Read(OggpackBuffer b, int bits) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			int retval = oggpack_read(ref b.oggpack_buffer, bits);
			b.changed = true;
			return retval;
		}
		#endregion

		#region oggpack_read1
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpack_read1(
			[In,Out]	ref oggpack_buffer	b);
		/// <summary>
		/// This function reads one bit from the oggpack_buffer data buffer and
		/// advances the location pointer. 
		/// </summary>
		/// <remarks>
		/// Before reading, the buffer should be initialized using
		/// <see cref="Readinit"/>.
		/// </remarks>
		/// <param name="b">
		/// An <see cref="OggpackBuffer"/> class containing buffered data to be
		/// read.
		/// </param>
		/// <returns>The bit read by this function.</returns>
		public static int Read1(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			int retval = oggpack_read1(ref b.oggpack_buffer);
			b.changed = true;
			return retval;
		}
		#endregion

		#region oggpack_bytes
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpack_bytes(
			[In]	ref oggpack_buffer	b);
		/// <summary>
		/// This function returns the total number of bytes currently in the
		/// <see cref="OggpackBuffer"/>'s internal buffer. 
		/// </summary>
		/// <remarks>
		/// The return value is the number of <b>complete</b> bytes in the buffer.
		/// There may be extra (&lt;8) bits. 
		/// </remarks>
		/// <param name="b">
		/// <see cref="OggpackBuffer"/> class to be checked.
		/// </param>
		/// <returns>
		/// The total number of bytes within the current buffer. 
		/// </returns>
		public static int Bytes(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			int retval = oggpack_bytes(ref b.oggpack_buffer);
			return retval;
		}
		#endregion

		#region oggpack_bits
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpack_bits(
			[In]	ref oggpack_buffer	b);
		/// <summary>
		/// This function returns the total number of bits currently in the
		/// <see cref="OggpackBuffer"/>'s internal buffer.
		/// </summary>
		/// <param name="b">
		/// <see cref="OggpackBuffer"/> class to be.
		/// </param>
		/// <returns>
		/// The total number of bits within the current buffer.
		/// </returns>
		public static int Bits(OggpackBuffer b) 
		{
			if(b == null)
				throw new ArgumentException("OggpackBuffer b cannot be null!");
			int retval = oggpack_bits(ref b.oggpack_buffer);
			return retval;
		}
		#endregion

		#region oggpack_get_buffer TODO
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr	oggpack_get_buffer(
			[In]	ref oggpack_buffer	b); // unsigned char *
//		/// <summary>
//		/// This function returns a pointer to the data buffer within the given
//		/// <see cref="OggpackBuffer"/> class.
//		/// </summary>
//		/// <param name="b">The current <see cref="OggpackBuffer"/></param>
//		/// <returns></returns>
//		public static byte[] GetBuffer(OggpackBuffer b) 
//		{
//			if(b == null)
//				throw new ArgumentException("OggpackBuffer b cannot be null!");
//			throw new NotImplementedException();
//			oggpack_get_buffer(ref b.oggpack_buffer);
//		}
		#endregion

		#endregion
	}

	internal abstract class OggpackB 
	{
		#region Ogg BITSTREAM PRIMITIVES: bitstream ************************
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_writeinit(
			[In,Out] ref oggpack_buffer b);
		private static void Writeinit(OggpackBuffer b) 
		{
			oggpackB_writeinit(ref b.oggpack_buffer);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_writetrunc(
			[In,Out] ref oggpack_buffer b, int bits);
		private static void Writetrunc(OggpackBuffer b, int bits) 
		{
			oggpackB_writetrunc(ref b.oggpack_buffer, bits);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_writealign(
			[In,Out] ref oggpack_buffer b);
		private static void Writealign(OggpackBuffer b) 
		{
			oggpackB_writealign(ref b.oggpack_buffer);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_writecopy(
			[In,Out] ref oggpack_buffer b, IntPtr source, int bits); // void *
		//		private static void Writecopy(OggpackBuffer b, IntPtr source, int bits) 
		//		{
		//			b.changed = true;
		//			throw new NotImplementedException();
		//		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_reset(
			[In,Out] ref oggpack_buffer b);
		private static void Reset(OggpackBuffer b) 
		{
			oggpackB_reset(ref b.oggpack_buffer);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_writeclear(
			[In,Out] ref oggpack_buffer b);
		private static void Writeclear(OggpackBuffer b) 
		{
			oggpackB_writeclear(ref b.oggpack_buffer);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_readinit(
			[In,Out] ref oggpack_buffer b, IntPtr buf, int bytes); // unsigned char *
		//		private static void Readinit(OggpackBuffer b, IntPtr buf, int bytes) 
		//		{
		//			b.changed = true;
		//			throw new NotImplementedException();
		//		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_write(
			[In,Out] ref oggpack_buffer b, uint value, int bits);
		private static void Write(OggpackBuffer b, uint value, int bits) 
		{
			oggpackB_write(ref b.oggpack_buffer, value, bits);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpackB_look(
			[In] ref oggpack_buffer b, int bits);
		private static int Look(OggpackBuffer b, int bits) 
		{
			return oggpackB_look(ref b.oggpack_buffer, bits);
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpackB_look1(
			[In] ref oggpack_buffer b);
		private static int Look1(OggpackBuffer b) 
		{
			return oggpackB_look1(ref b.oggpack_buffer);
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_adv(
			[In,Out] ref oggpack_buffer b, int bits);
		private static void Adv(OggpackBuffer b, int bits) 
		{
			oggpackB_adv(ref b.oggpack_buffer, bits);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	oggpackB_adv1(
			[In,Out] ref oggpack_buffer b);
		private static void Adv1(OggpackBuffer b) 
		{
			oggpackB_adv1(ref b.oggpack_buffer);
			b.changed = true;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpackB_read(
			[In,Out] ref oggpack_buffer b, int bits);
		private static int Read(OggpackBuffer b, int bits) 
		{
			int retval = oggpackB_read(ref b.oggpack_buffer, bits);
			b.changed = true;
			return retval;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpackB_read1(
			[In,Out] ref oggpack_buffer b);
		private static int Read1(OggpackBuffer b) 
		{
			int retval = oggpackB_read1(ref b.oggpack_buffer);
			b.changed = true;
			return retval;
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpackB_bytes(
			[In] ref oggpack_buffer b);
		private static int Bytes(OggpackBuffer b) 
		{
			return oggpackB_bytes(ref b.oggpack_buffer);
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	oggpackB_bits(
			[In] ref oggpack_buffer b);
		private static int Bits(OggpackBuffer b) 
		{
			return oggpackB_bits(ref b.oggpack_buffer);
		}

		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr	oggpackB_get_buffer(
			[In] ref oggpack_buffer b);	// unsigned char *
		//		private static byte[] GetBuffer(OggpackBuffer b) 
		//		{
		//			throw new NotImplementedException();
		//		}
		#endregion
	}
}
