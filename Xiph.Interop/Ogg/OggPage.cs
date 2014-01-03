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
	/// <summary>
	/// ogg_page is used to encapsulate the data in one Ogg bitstream page
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ogg_page
	{
		internal IntPtr header;
		internal int header_len;
		internal IntPtr body;
		internal int body_len;
	}

	/// <summary>
	/// The OggPage class encapsulates the data for an Ogg page. 
	/// </summary>
	/// <remarks>
	/// Ogg pages are the fundamental unit of framing and interleave in an ogg
	/// bitstream. They are made up of packet segments of 255 bytes each. There
	/// can be as many as 255 packet segments per page, for a maximum page size
	/// of a little under 64 kB. This is not a practical limitation as the
	/// segments can be joined across page boundaries allowing packets of
	/// arbitrary size. In practice pages are usually around 4 kB. 
	/// <para>
	/// For a complete description of ogg pages and headers, please refer to
	/// the framing document.
	/// </para>
	/// </remarks>
	public class OggPage
	{
		#region Variables
		internal ogg_page ogg_page = new ogg_page();
		internal bool changed = true;
		private byte[] header = null;
		private byte[] body = null;
		#endregion

		private void MarshalData() 
		{
			if(this.changed) 
			{
				this.header = new byte[this.HeaderLen];
				Marshal.Copy(this.ogg_page.header, header, 0, this.HeaderLen);
				this.body = new byte[this.BodyLen];
				Marshal.Copy(this.ogg_page.body, body, 0, this.BodyLen);
				this.changed = false;
			}
		}

		#region Properties
		/// <summary>
		/// The page header for this page. The exact contents of this header
		/// are defined in the framing spec document.
		/// </summary>
		public byte[] Header 
		{
			get 
			{
				this.MarshalData();
				return this.header;
			}
		}
		/// <summary>
		/// Length of the page header in bytes.
		/// </summary>
		public int HeaderLen 
		{
			get { return this.ogg_page.header_len; }
		}
		/// <summary>
		/// The data for this page.
		/// </summary>
		public byte[] Body 
		{
			get 
			{
				this.MarshalData();
				return this.body;
			}
		}
		/// <summary>
		/// Length of the body data in bytes.
		/// </summary>
		public int BodyLen 
		{
			get { return this.ogg_page.body_len; }
		}
		#endregion
		
		#region Ogg BITSTREAM PRIMITIVES: general ***************************

		#region ogg_page_version
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_version(
			[In]	ref ogg_page	og);
		/// <summary>
		/// This function returns the version of <see cref="OggPage"/> used in
		/// this page.
		/// </summary>
		/// <remarks>
		/// In current versions of libogg, all <see cref="OggPage"/> class
		/// objects have the same version, so 0 should always be returned.
		/// </remarks>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object. 
		/// </param>
		/// <returns>
		/// The version number. In the current version of Ogg, the version
		/// number is always 0. Nonzero return values indicate an error in page
		/// encoding. 
		/// </returns>
		public static int Version(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_page_version(ref og.ogg_page);
			return retval;
		}
		#endregion
		
		#region ogg_page_continued
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_continued(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Indicates whether this page contains packet data which has been
		/// continued from the previous page. 
		/// </summary>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// 1 if this page contains packet data continued from the last page.
		/// </description></item>
		/// <item><description>
		/// 0 if this page does not contain continued data.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Continued(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_page_continued(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_packets
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_packets(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Returns the number of packets that are completed on this page. If
		/// the leading packet is begun on a previous page, but ends on this
		/// page, it's counted. 
		/// </summary>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object.
		/// </param>
		/// <returns>
		/// If a page consists of a packet begun on a previous page, and a new
		/// packet begun (but not completed) on this page, the return will be:
		/// <list type="bullet">
		/// <item><description>
		/// OggPage.Packets(page) will return 1,
		/// </description></item>
		/// <item><description>
		/// OggPage.Continued(page) will return non-zero.
		/// </description></item>
		/// </list>
		/// If a page happens to be a single packet that was begun on a
		/// previous page, and spans to the next page (in the case of a three
		/// or more page packet), the return will be:
		/// <list type="bullet">
		/// <item><description>
		/// OggPage.Packets(page) will return 0,
		/// </description></item>
		/// <item><description>
		/// OggPage.Continued(page) will return non-zero.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Packets(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_page_packets(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_bos
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_bos(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Indicates whether this page is at the beginning of the logical
		/// bitstream. 
		/// </summary>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// Greater than 0 if this page is the beginning of a bitstream.
		/// </description></item>
		/// <item><description>
		/// 0 if this page is from any other location in the stream.
		/// </description></item>
		/// </list>
		///</returns>
		public static int Bos(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_page_bos(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_eos
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_eos(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Indicates whether this page is at the end of the logical bitstream.
		/// </summary>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object. 
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// Greater than zero if this page contains the end of a bitstream. 
		/// </description></item>
		/// <item><description>
		/// 0 if this page is from any other location in the stream.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Eos(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_page_eos(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_granulepos
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern long	ogg_page_granulepos(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Returns the exact granular position of the packet data contained at
		/// the end of this page. 
		/// </summary>
		/// <remarks>
		/// This is useful for tracking location when seeking or decoding.
		/// <para>
		/// For example, in audio codecs this position is the pcm sample number
		/// and in video this is the frame number.
		/// </para>
		/// </remarks>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object.
		/// </param>
		/// <returns>
		/// The specific last granular position of the decoded data contained
		/// in the page.
		/// </returns>
		public static long Granulepos(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			long retval = ogg_page_granulepos(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_serialno
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_serialno(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Returns the unique serial number for the logical bitstream of this
		/// page. Each page contains the serial number for the logical
		/// bitstream that it belongs to. 
		/// </summary>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object.
		/// </param>
		/// <returns>The serial number for this page.</returns>
		public static int Serialno(OggPage og) 
		{
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_page_serialno(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_pageno
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_page_pageno(
			[In]	ref ogg_page	og);
		/// <summary>
		/// Returns the sequential page number.
		/// </summary>
		/// <remarks>
		/// This is useful for ordering pages or determining when pages have
		/// been lost. 
		/// </remarks>
		/// <param name="og">
		/// The current <see cref="OggPage"/> class object.
		/// </param>
		/// <returns>The page number for this page.</returns>
		public static int Pageno(OggPage og) 
		{
			int retval = ogg_page_pageno(ref og.ogg_page);
			return retval;
		}
		#endregion

		#region ogg_page_checksum_set
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	ogg_page_checksum_set(
			[In]	ref ogg_page	og); // og.header is modified
		/// <summary>
		/// Checksums an <see cref="OggPage"/>.
		/// </summary>
		/// <param name="og">An <see cref="OggPage"/> class object.</param>
		public static void ChecksumSet(OggPage og) 
		{
			ogg_page_checksum_set(ref og.ogg_page);
			og.changed = true;
		}
		#endregion

		#endregion
	}
}
