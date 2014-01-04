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
	/// ogg_stream_state contains the current encode/decode state of a logical
	/// Ogg bitstream
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ogg_stream_state
	{
		/// <summary>
		/// bytes from packet bodies
		/// </summary>
		internal IntPtr		body_data;	// unsigned char *
		/// <summary>
		/// storage elements allocated
		/// </summary>
		internal int		body_storage;
		/// <summary>
		/// elements stored; fill mark
		/// </summary>
		internal int		body_fill;
		/// <summary>
		/// elements of fill returned
		/// </summary>
		internal int		body_returned;
		/// <summary>
		/// The values that will go to the segment table
		/// </summary>
		internal IntPtr		lacing_vals;	// int *
		/// <summary>
		/// granulepos values for headers. Not compact this way, but it is
		/// simple coupled to the lacing fifo
		/// </summary>
		internal IntPtr		granule_vals;	// ogg_int64_t *
		internal int		lacing_storage;
		internal int		lacing_fill;
		internal int		lacing_packet;
		internal int		lacing_returned;
		/// <summary>
		/// working space for header encode
		/// </summary>
		internal IntPtr		header;
		internal int		header_fill;
		/// <summary>
		/// set when we have buffered the last packet in the logical bitstream
		/// </summary>
		internal int		e_o_s;
		/// <summary>
		/// set after we've written the initial page of a logical bitstream
		/// </summary>
		internal int		b_o_s;
		internal int		serialno;
		internal int		pageno;
		/// <summary>
		/// sequence number for decode; the framing knows where there's a hole
		/// in the data, but we need coupling so that the codec (which is in a
		/// seperate abstraction layer) also knows about the gap
		/// </summary>
		internal long		packetno;
		internal long		granulepos;
	}


	/// <summary>
	/// The OggStreamState class tracks the current encode/decode state of the
	/// current logical bitstream.
	/// </summary>
	public class OggStreamState
	{
		#region Variables
		internal ogg_stream_state ogg_stream_state = new ogg_stream_state();
		internal bool changed = true;
		#endregion

		#region Properties
		/// <summary>
		/// Marker set when the last packet of the logical bitstream has been
		/// buffered. 
		/// </summary>
		public int E_o_s 
		{
			get { return this.ogg_stream_state.e_o_s; }
			set { this.ogg_stream_state.e_o_s = value; }
		}
		#endregion

		#region Ogg BITSTREAM PRIMITIVES: encoding **************************
		
		#region ogg_stream_packetin
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_packetin(
			[In,Out]	ref ogg_stream_state	os,
			[In]		ref ogg_packet			op);
		/// <summary>
		/// This function takes a packet and submits it to the bitstream. After
		/// this is called, we can continue submitting packets, or we can write
		/// out pages. 
		/// </summary>
		/// <remarks>
		/// In a typical decoding situation, this should be used after filling
		/// a packet with data.
		/// </remarks>
		/// <param name="os">
		/// A previously declared <see cref="OggStreamState"/> class. 
		/// </param>
		/// <param name="op">
		/// The packet we are putting into the bitstream.
		/// </param>
		/// <returns>0 is always returned.</returns>
		public static int Packetin(OggStreamState os, OggPacket op) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			if(op == null)
				throw new ArgumentException("OggPacket op cannot be null!");
			int retval = ogg_stream_packetin(ref os.ogg_stream_state,
				ref op.ogg_packet);
			os.changed = true;
			return retval;
		}
		#endregion
		
		#region ogg_stream_pageout
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_pageout(
			[In,Out]	ref ogg_stream_state	os,
			[In,Out]	ref ogg_page			og); // uses ogg_stream_flush
		/// <summary>
		/// This function forms packets into pages.
		/// </summary>
		/// <remarks>
		/// In a typical encoding situation, this would be called after using
		/// <see cref="Packetin"/> to submit data packets to the bitstream.
		/// <para>
		/// Internally, this function breaks the page into packet segments in
		/// preparation for outputting a valid packet to the codec decoding
		/// layer.
		/// </para>
		/// </remarks>
		/// <param name="os">
		/// A previously declared <see cref="OggStreamState"/> class, which
		/// represents the current logical bitstream.
		/// </param>
		/// <param name="og">
		/// A page of data. The data inside this page is being submitted to the
		/// streaming layer in order to be allocated into packets.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// Zero means that insufficient data has accumulated to fill a page.
		/// </description></item>
		/// <item><description>
		/// Non-zero means that a page has been completed and flushed. 
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Pageout(OggStreamState os, OggPage og) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_stream_pageout(ref os.ogg_stream_state,
				ref og.ogg_page);
			os.changed = true;
			og.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_flush
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_flush(
			[In,Out]	ref ogg_stream_state	os,
			[In,Out]	ref ogg_page			og);
		/// <summary>
		/// This function checks for remaining packets inside the stream and
		/// forces remaining packets into a page, regardless of the size of the
		/// page. 
		/// </summary>
		/// <remarks>
		/// This should only be used when you want to flush an undersized page
		/// from the middle of the stream. Otherwise, <see cref="Pageout"/>
		/// should always be used.
		/// <para>
		/// This function can be used to verify that all packets have been
		/// flushed. If the return value is 0, all packets have been placed
		/// into a page. 
		/// </para>
		/// </remarks>
		/// <param name="os">
		/// A previously declared <see cref="OggStreamState"/> class, which
		/// represents the current logical bitstream.
		/// </param>
		/// <param name="og">
		/// A page of data. The remaining packets in the stream will be placed
		/// into this page, if any remain. 
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// 0 means that all packet data has already been flushed into pages,
		/// and there are no packets to put into the page.
		/// </description></item>
		/// <item><description>
		/// Nonzero means that remaining packets have successfully been flushed
		/// into the page. 
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Flush(OggStreamState os, OggPage og) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_stream_flush(ref os.ogg_stream_state, ref og.ogg_page);

			os.changed = true;
			og.changed = true;
			return retval;
		}
		#endregion

		#endregion

		#region Ogg BITSTREAM PRIMITIVES: decoding **************************

		#region ogg_stream_pagein
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_pagein(
			[In,Out]	ref ogg_stream_state	os,
			[In]		ref ogg_page			og);
		/// <summary>
		/// This function adds a complete page to the bitstream.
		/// </summary>
		/// <remarks>
		/// In a typical decoding situation, this function would be called
		/// after using <see cref="OggSyncState.Pageout"/> to create a valid
		/// <see cref="OggPage"/> class. 
		/// <para>
		/// Internally, this function breaks the page into packet segments in
		/// preparation for outputting a valid packet to the codec decoding
		/// layer.
		/// </para>
		/// </remarks>
		/// <param name="os">
		/// A previously declared <see cref="OggStreamState"/> class, which
		/// represents the current logical bitstream. 
		/// </param>
		/// <param name="og">
		/// A page of data. The data inside this page is being submitted to the
		/// streaming layer in order to be allocated into packets. 
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// -1 indicates failure. This means that the serial number of the page
		/// did not match the serial number of the bitstream, or that the page
		/// version was incorrect.
		/// </description></item>
		/// <item><description>
		/// 0 means that the page was successfully submitted to the bitstream.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Pagein(OggStreamState os, OggPage og) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			if(og == null)
				throw new ArgumentException("OggPage og cannot be null!");
			int retval = ogg_stream_pagein(ref os.ogg_stream_state, ref og.ogg_page);
			os.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_packetout
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_packetout(
			[In,Out]	ref ogg_stream_state	os,
			[In,Out]	ref ogg_packet			op);
		/// <summary>
		/// This function assembles a raw data packet for output to the codec
		/// decoding engine. The data is already in the stream and broken into
		/// packet segments.
		/// </summary>
		/// <remarks>
		/// In a typical decoding situation, this should be used after calling
		/// <see cref="Pagein"/> to submit a page of data to the bitstream.
		/// </remarks>
		/// <param name="os">
		/// A previously declared <see cref="OggStreamState"/> class. Before
		/// this function is called, an <see cref="OggPage"/> should be
		/// submitted to the stream using <see cref="Pagein"/>. 
		/// </param>
		/// <param name="op">
		/// The packet that will be submitted to the decoding layer after this
		/// function is called.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// -1 if we are out of sync and there is a gap in the data. Usually
		/// this will not be a fatal error.
		/// </description></item>
		/// <item><description>
		/// 1 in all other cases.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Packetout(OggStreamState os, OggPacket op) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			int retval = ogg_stream_packetout(ref os.ogg_stream_state,
				ref op.ogg_packet);
			os.changed = true;
			op.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_packetpeek
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_packetpeek(
			[In,Out]	ref ogg_stream_state	os,
			[In,Out]	ref ogg_packet			op);
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_packetpeek(
			[In,Out]	ref ogg_stream_state	os,
			IntPtr op);
		/// <summary>
		/// This function attempts to assemble a raw data packet and returns it
		/// without advancing decoding.
		/// </summary>
		/// <remarks>
		/// In a typical situation, this would be called speculatively after
		/// <see cref="Pagein"/> to check the packet contents before handing it
		/// off to a codec for decompression. To advance page decoding and
		/// remove the packet from the sync structure, call
		/// <see cref="Packetout"/>.
		/// </remarks>
		/// <param name="os">
		/// A previously declared <see cref="OggStreamState"/> class. Before
		/// this function is called, an <see cref="OggPage"/> should be
		/// submitted to the stream using <see cref="Pagein"/>. 
		/// </param>
		/// <param name="op">
		/// The next packet available in the bitstream, if any. A NULL value
		/// may be passed in the case of a simple "is there a packet?" check. 
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// -1 if there's no packet available due to lost sync or a hole in the
		/// data.
		/// </description></item>
		/// <item><description>
		/// 1 if a packet is available.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Packetpeek(OggStreamState os, OggPacket op) 
		{
			int retval;
			if(op == null)
				retval = ogg_stream_packetpeek(ref os.ogg_stream_state,
					IntPtr.Zero);
			else 
			{
				retval = ogg_stream_packetpeek(ref os.ogg_stream_state,
					ref op.ogg_packet);
				op.changed = true;
			}
			os.changed = true;
			return retval;
		}
		#endregion

		#endregion

		#region Ogg BITSTREAM PRIMITIVES: general ***************************

		#region ogg_stream_init
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_init(
			[In,Out]	ref ogg_stream_state	os,
			int serialno);
		/// <summary>
		/// This function is used to initialize an <see cref="OggStreamState"/>
		/// class and allocates appropriate memory in preparation for encoding
		/// or decoding. 
		/// </summary>
		/// <remarks>
		/// It also assigns the stream a given serial number.
		/// </remarks>
		/// <param name="os">
		/// Pointer to the <see cref="OggStreamState"/> class that we will be
		/// initializing. 
		/// </param>
		/// <param name="serialno">
		/// Serial number that we will attach to this stream.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// 0 if successful.
		/// </description></item>
		/// <item><description>
		/// -1 if unsuccessful. If this fails, the <see cref="OggStreamState"/>
		/// was not properly declared before calling this function.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Init(OggStreamState os, int serialno) 
		{
			int retval = ogg_stream_init(ref os.ogg_stream_state, serialno);
			os.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_clear
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_clear(
			[In,Out]	ref ogg_stream_state	os);
		/// <summary>
		/// This function clears the memory used by the
		/// <see cref="OggStreamState"/> class, but does not free it.
		/// </summary>
		/// <param name="os">
		/// The <see cref="OggStreamState"/> class to be cleared.
		/// </param>
		/// <returns>
		/// 0 is always returned.
		/// </returns>
		public static int Clear(OggStreamState os) 
		{
			int retval = ogg_stream_clear(ref os.ogg_stream_state);
			os.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_reset
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_reset(
			[In,Out]	ref ogg_stream_state	os);
		/// <summary>
		/// This function sets values in the <see cref="OggStreamState"/>
		/// class back to initial values.
		/// </summary>
		/// <param name="os">
		/// The <see cref="OggStreamState"/> class to be cleared.
		/// </param>
		/// <returns>0 is always returned.</returns>
		private static int Reset(OggStreamState os) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			int retval = ogg_stream_reset(ref os.ogg_stream_state);
			os.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_reset_serialno
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_reset_serialno(
			[In,Out]	ref ogg_stream_state	os,
			int serialno);
		/// <summary>
		/// This function reinitializes the values in the 
		/// <see cref="OggStreamState"/>, just like <see cref="Reset"/>.
		/// Additionally, it sets the stream serial number to the given value.
		/// </summary>
		/// <param name="os">
		/// The <see cref="OggStreamState"/> class to be cleared.
		/// </param>
		/// <param name="serialno">New stream serial number to use.</param>
		/// <returns>0 is always returned.</returns>
		public static int ResetSerialno(OggStreamState os, int serialno) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			int retval = ogg_stream_reset_serialno(ref os.ogg_stream_state,
				serialno);
			os.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_destroy
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_destroy(
			[In,Out]	ref ogg_stream_state	os);	// free os
		/// <summary>
		/// This function frees the memory used by the
		/// <see cref="OggStreamState"/> class. 
		/// </summary>
		/// <remarks>
		/// This should be called when you are done working with an ogg stream.
		/// It can also be called to make sure that the struct does not exist.
		/// </remarks>
		/// <param name="os">
		/// Pointer to the <see cref="OggStreamState"/> class to be destroyed. 
		/// </param>
		/// <returns>0 is always returned.</returns>
		public static int Destroy(OggStreamState os) 
		{
			int retval = ogg_stream_destroy(ref os.ogg_stream_state);
			os.changed = true;
			return retval;
		}
		#endregion

		#region ogg_stream_eos
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_stream_eos(
			[In]		ref ogg_stream_state	os);
		/// <summary>
		/// This function indicates whether we have reached the end of the
		/// stream or not.
		/// </summary>
		/// <param name="os">
		/// The current <see cref="OggStreamState"/> class.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// 1 if we are at the end of the stream.
		/// </description></item>
		/// <item><description>
		/// 0 if we have not yet reached the end of the stream. 
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Eos(OggStreamState os) 
		{
			if(os == null)
				throw new ArgumentException("OggStreamState os cannot be null!");
			int retval = ogg_stream_eos(ref os.ogg_stream_state);
			return retval;
		}
		#endregion

		#endregion
	}
}