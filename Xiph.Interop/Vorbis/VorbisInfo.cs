/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2001             *
 * by the XIPHOPHORUS Company http://www.xiph.org/                  *

 ********************************************************************

 function: libvorbis codec headers

 ********************************************************************/

/* C#/.NET interop-port
 * 
 * Copyright 2004 Klaus Prückl <klaus.prueckl@aon.at>
 */

using System;
using System.Runtime.InteropServices;

namespace Xiph.Interop.Vorbis
{
	/// <summary>
	/// vorbis_info contains all the setup information specific to the
	/// specific compression/decompression mode in progress (eg,
	/// psychoacoustic settings, channel setup, options, codebook
	/// etc). vorbis_info and substructures are in backends.h.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct vorbis_info
	{
		internal int version;
		internal int channels;
		internal int rate;

		/*	The below bitrate declarations are *hints*.
			Combinations of the three values carry the following implications:

			all three set to the same value: 
				implies a fixed rate bitstream
			only nominal set: 
				implies a VBR stream that averages the nominal bitrate.  No hard 
				upper/lower limit
			upper and or lower set: 
				implies a VBR bitstream that obeys the bitrate limits. nominal 
				may also be set to give a nominal rate.
			none set:
				the coder does not care to speculate.
		*/

		internal int bitrate_upper;
		internal int bitrate_nominal;
		internal int bitrate_lower;
		internal int bitrate_window;

		internal IntPtr codec_setup;	// void *
	}

	/// <summary>
	/// The VorbisInfo class contains information about a vorbis bitstream.
	/// Most of the information in this class is more complex and in-depth than
	/// we need when using basic API calls. 
	/// </summary>
	public class VorbisInfo
	{
		#region variables
		internal vorbis_info vorbis_info;
		internal bool changed = true;
		#endregion

		#region properties
		/// <summary>
		/// Vorbis encoder version used to create this bitstream. 
		/// </summary>
		public int Version 
		{
			get { return this.vorbis_info.version; }
		}
		/// <summary>
		/// Int signifying number of channels in bitstream.
		/// </summary>
		public int Channels 
		{
			get { return this.vorbis_info.channels; }
		}
		/// <summary>
		/// Sampling rate of the bitstream.
		/// </summary>
		public int Rate 
		{
			get { return this.vorbis_info.rate; }
		}
		/// <summary>
		/// Specifies the upper limit in a VBR bitstream. If the value matches
		/// the <see cref="BitrateNominal"/> and <see cref="BitrateLower"/>
		/// parameters, the stream is fixed bitrate. May be unset if no limit
		/// exists. 
		/// </summary>
		public int BitrateUpper 
		{
			get { return this.vorbis_info.bitrate_upper; }
		}
		/// <summary>
		/// Specifies the average bitrate for a VBR bitstream. May be unset. If
		/// the <see cref="BitrateUpper"/> and <see cref="BitrateLower"/>
		/// parameters match, the stream is fixed bitrate. 
		/// </summary>
		public int BitrateNominal 
		{
			get { return this.vorbis_info.bitrate_nominal; }
		}
		/// <summary>
		/// Specifies the lower limit in a VBR bitstream. If the value matches
		/// the <see cref="BitrateNominal"/> and <see cref="BitrateUpper"/>
		/// parameters, the stream is fixed bitrate. May be unset if no limit
		/// exists. 
		/// </summary>
		public int BitrateLower 
		{
			get { return this.vorbis_info.bitrate_lower; }
		}
		#endregion

		#region Vorbis PRIMITIVES: general ***************************************

		#region vorbis_info_init
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_info_init(
			[In,Out]	ref vorbis_info		vi);
		/// <summary>
		/// </summary>
		/// <param name="vi"></param>
		public static void Init(VorbisInfo vi) 
		{
			if(vi == null)
				throw new ArgumentException("VorbisInfo vi cannot be null!");
			vorbis_info_init(ref vi.vorbis_info);
			vi.changed = true;
		}
		#endregion

		#region vorbis_info_clear
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_info_clear(
			[In,Out]	ref vorbis_info		vi);
		/// <summary>
		/// </summary>
		/// <param name="vi"></param>
		public static void Clear(VorbisInfo vi) 
		{
			if(vi == null)
				throw new ArgumentException("VorbisInfo vi cannot be null!");
			vorbis_info_init(ref vi.vorbis_info);
			vi.changed = true;
		}
		#endregion

		#region vorbis_info_blocksize
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_info_blocksize(
			[In]	ref vorbis_info	vi, int zo);
		/// <summary>
		/// </summary>
		/// <param name="vi"></param>
		/// <param name="zo"></param>
		/// <returns></returns>
		public static int Blocksize(VorbisInfo vi, int zo) 
		{
			if(vi == null)
				throw new ArgumentException("VorbisInfo vi cannot be null!");
			int retval = vorbis_info_blocksize(ref vi.vorbis_info, zo);
			return retval;
		}
		#endregion

		#endregion
	}
}
