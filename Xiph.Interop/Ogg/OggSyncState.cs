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
	internal struct ogg_sync_state
	{
		internal IntPtr	data;	// unsigned char *
		internal int	storage;
		internal int	fill;
		internal int	returned;

		internal int	unsynced;
		internal int	headerbytes;
		internal int	bodybytes;
	}

	/// <summary>
	/// The OggSyncState class tracks the synchronization of the current page.
	/// </summary>
	/// <remarks>
	/// It is used during decoding to track the status of data as it is read
	/// in.
	/// </remarks>
	public class OggSyncState
	{
		#region Variables
		internal ogg_sync_state ogg_sync_state = new ogg_sync_state();
		internal IntPtr pbuffer;
		internal byte[] buffer = null;
		internal bool changed = true;
		#endregion

		#region Ogg BITSTREAM PRIMITIVES: decoding **************************
		
		#region ogg_sync_init
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_init(
			[In,Out]	ref ogg_sync_state	oy);
		/// <summary>
		/// This function is used to initialize an <see cref="OggSyncState"/>
		/// class to a known initial value in preparation for manipulation of
		/// an Ogg bitstream.
		/// </summary>
		/// <remarks>
		/// The <see cref="OggSyncState"/> class is important when decoding, as
		/// it synchronizes retrieval and return of data.
		/// </remarks>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class. After this
		/// function call, this struct has been initialized.
		/// </param>
		/// <returns>0 is always returned.</returns>
		public static int Init(OggSyncState oy) 
		{
			int retval = ogg_sync_init(ref oy.ogg_sync_state);
			oy.changed = true;
			return retval;
		}
		#endregion

		#region ogg_sync_clear
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_clear(
			[In,Out]	ref ogg_sync_state	oy);	// free oy.data
		/// <summary>
		/// This function is used to free the internal storage of an
		/// <see cref="OggSyncState"/> class and resets the struct to the
		/// initial state. To free the entire struct, <see cref="Destroy"/>
		/// should be used instead. In situations where the class needs to be
		/// reset but the internal storage does not need to be freed,
		/// <see cref="Reset"/> should be used.
		/// </summary>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class. 
		/// </param>
		/// <returns>0 is always returned.</returns>
		public static int Clear(OggSyncState oy) 
		{
			int retval = ogg_sync_clear(ref oy.ogg_sync_state);
			oy.changed = true;
			return retval;
		}
		#endregion

		#region ogg_sync_destroy
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_destroy(
			[In,Out]	ref ogg_sync_state	oy);	// free oy
		/// <summary>
		/// This function is used to destroy an <see cref="OggSyncState"/>
		/// class and free all memory used.
		/// </summary>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class.
		/// </param>
		/// <returns>0 is always returned.</returns>
		public static int Destroy(OggSyncState oy) 
		{
			int retval = ogg_sync_destroy(ref oy.ogg_sync_state);
			oy.changed = true;
			return retval;
		}
		#endregion

		#region ogg_sync_reset
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_reset(
			[In,Out]	ref ogg_sync_state	oy);
		/// <summary>
		/// This function is used to reset the internal counters of the
		/// <see cref="OggSyncState"/> class to initial values. 
		/// </summary>
		/// <remarks>
		/// It is a good idea to call this before seeking within a bitstream. 
		/// </remarks>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class. 
		/// </param>
		/// <returns>0 is always returned.</returns>
		public static int Reset(OggSyncState oy) 
		{
			if(oy == null)
				throw new ArgumentException("OggSyncState oy cannot be null!");
			int retval = ogg_sync_reset(ref oy.ogg_sync_state);
			oy.changed = true;
			return retval;
		}
		#endregion

		#region ogg_sync_buffer
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr	ogg_sync_buffer(
			[In,Out]	ref ogg_sync_state	oy,
			int size);
		/// <summary>
		/// This function is used to provide a properly-sized buffer for
		/// writing.
		/// </summary>
		/// <remarks>
		/// Buffer space which has already been returned is cleared, and the
		/// buffer is extended as necessary by the size plus some additional
		/// bytes. Within the current implementation, an extra 4096 bytes are
		/// allocated, but applications should not rely on this additional
		/// buffer space.
		/// <para>
		/// The buffer exposed by this function is empty internal storage from
		/// the <see cref="OggSyncState"/> class, beginning at the fill mark
		/// within the class. 
		/// </para>
		/// <para>
		/// A buffer is returned to be used by the calling application.
		/// </para>
		/// </remarks>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class.</param>
		/// <param name="size">
		/// Size of the desired buffer. The actual size of the buffer returned
		/// will be this size plus some extra bytes (currently 4096).
		/// </param>
		/// <returns>Returns the newly allocated buffer.</returns>
		public static byte[] Buffer(OggSyncState oy, int size) 
		{
			if(oy == null)
				throw new ArgumentException("OggSyncState oy cannot be null!");
			oy.pbuffer = ogg_sync_buffer(ref oy.ogg_sync_state, size);
			oy.buffer = new byte[size];
			oy.changed = true;
			return oy.buffer;
		}
		#endregion

		#region ogg_sync_wrote
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_wrote(
			[In,Out]	ref ogg_sync_state	oy,
			int bytes);
		/// <summary>
		/// This function is used to tell the <see cref="OggSyncState"/> class
		/// how many bytes we wrote into the buffer.
		/// </summary>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class.
		/// </param>
		/// <param name="bytes"></param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// -1 if the number of bytes written overflows the internal storage of
		/// the <see cref="OggSyncState"/> class.
		/// </description></item>
		/// <item><description>
		/// 0 in all other cases.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Wrote(OggSyncState oy, int bytes) 
		{
			if(oy == null)
				throw new ArgumentException("OggSyncState oy cannot be null!");
			Marshal.Copy(oy.buffer, 0, oy.pbuffer, bytes);
			int retval = ogg_sync_wrote(ref oy.ogg_sync_state, bytes);
			oy.changed = true;
			return retval;
		}
		#endregion

		#region ogg_sync_pageseek
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_pageseek(
			[In,Out]	ref ogg_sync_state	oy,
			[In,Out]	ref ogg_page		og);
		/// <summary>
		/// This function synchronizes the <see cref="OggSyncState"/> class to
		/// the next <see cref="OggPage"/>. 
		/// </summary>
		/// <remarks>
		/// This is useful when seeking within a bitstream.
		/// <see cref="Pageseek"/> will synchronize to the next page in the
		/// bitstream and return information about how many bytes we advanced
		/// or skipped in order to do so. 
		/// </remarks>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class.
		/// </param>
		/// <param name="og">
		/// A page (or an incomplete page) of data. This is the page we are
		/// attempting to sync.
		/// </param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// -n means that we skipped n bytes within the bitstream.
		/// </description></item>
		/// <item><description>
		/// 0 means that the page isn't ready and we need more data. No bytes
		/// have been skipped. 
		/// </description></item>
		/// <item><description>
		/// n means that the page was synced at the current location, with a
		/// page length of n bytes.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Pageseek(OggSyncState oy, OggPage og) 
		{
			if(oy == null)
				throw new ArgumentException("OggSyncState oy cannot be null!");
			int retval = ogg_sync_pageseek(ref oy.ogg_sync_state, ref og.ogg_page);
			oy.changed = true;
			og.changed = true;
			return retval;
		}
		#endregion

		#region ogg_sync_pageout
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	ogg_sync_pageout(
			[In,Out]	ref ogg_sync_state	oy,
			[In,Out]	ref ogg_page		og);
		/// <summary>
		/// This function takes the data stored in the buffer of the
		/// <see cref="OggSyncState"/> class and inserts them into an
		/// <see cref="OggPage"/>.
		/// </summary>
		/// <remarks>
		/// In an actual decoding loop, this function should be called first to
		/// ensure that the buffer is cleared. The example code below
		/// illustrates a clean reading loop which will fill and output pages.
		/// <para>
		/// <b>Caution:</b> This function should be called before reading into
		/// the buffer to ensure that data does not remain in the
		/// <see cref="OggSyncState"/> class. Failing to do so may result in a
		/// memory leak. See the example code below for details.
		/// </para>
		/// <code>
		/// if (OggSyncState.Pageout(oy, og) != 1) 
		///	{
		///		buffer = OggSyncState.Buffer(oy, 8192);
		///		bytes = stdin.Read(buffer, 0, 8192);
		///		OggSyncState.Wrote(oy, bytes);
		///	}
		/// </code>
		/// </remarks>
		/// <param name="oy">
		/// A previously declared <see cref="OggSyncState"/> class. Normally,
		/// the internal storage of this class should be filled with newly read
		/// data and verified using <see cref="Wrote"/>. 
		/// </param>
		/// <param name="og">Page class filled by this function.</param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// -1 if we were not properly synced and had to skip some bytes. 
		/// </description></item>
		/// <item><description>
		/// 0 if we need more data to verify a page. 
		/// </description></item>
		/// <item><description>
		/// 1 if we have a page.
		/// </description></item>
		/// </list>
		/// </returns>
		public static int Pageout(OggSyncState oy, OggPage og) 
		{
			if(oy == null)
				throw new ArgumentException("OggSyncState oy cannot be null!");
			int retval = ogg_sync_pageout(ref oy.ogg_sync_state, ref og.ogg_page);
			oy.changed = true;
			og.changed = true;
			return retval;
		}
		#endregion

		#endregion
	}
}
