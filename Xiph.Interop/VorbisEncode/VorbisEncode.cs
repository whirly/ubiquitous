/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2001             *
 * by the XIPHOPHORUS Company http://www.xiph.org/                  *
 *                                                                  *
 ********************************************************************

 function: vorbis encode-engine setup

 ********************************************************************/

/* C#/.NET interop-port
 * 
 * Copyright 2004 Klaus Prückl <klaus.prueckl@aon.at>
 */

using System;
using System.Configuration;
using System.Runtime.InteropServices;

using Xiph.Interop.Vorbis;

namespace Xiph.Interop.VorbisEncode
{
	/// <summary>
	/// </summary>
	public enum OvEctl
	{
		/// <summary>
		/// </summary>
		RatemanageGet		= 0x10,

		/// <summary>
		/// </summary>
		RatemanageSet		= 0x11,
		/// <summary>
		/// </summary>
		RatemanageAvg		= 0x12,
		/// <summary>
		/// </summary>
		RatemanageHard		= 0x13,

		/// <summary>New in Vorbis 1.1.0</summary>
		Ratemanage2Get		= 0x14,
		/// <summary>New in Vorbis 1.1.0</summary>
		Ratemanage2Set		= 0x15,

		/// <summary>
		/// </summary>
		LowpassGet			= 0x20,
		/// <summary>
		/// </summary>
		LowpassSet			= 0x21,

		/// <summary>
		/// </summary>
		IBlockGet			= 0x30,
		/// <summary>
		/// </summary>
		IBlockSet			= 0x31,
	}

	/// <summary>
	/// </summary>
	public abstract class VorbisEncode
	{
		// lots of asserts are thrown if debug-dll is used
		internal const string DllFile = @"libvorbis.dll";

		#region vorbis_encode_init
		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_init([In,Out] ref vorbis_info vi,
			int channels, int rate,		      
			int max_bitrate, int nominal_bitrate, int min_bitrate);
		/// <summary>
		/// This is the primary function within libvorbisenc. This is used to
		/// properly set up an encoding environment using libvorbisenc. 
		/// </summary>
		/// <remarks>
		/// Before this function is called, the <see cref="VorbisInfo"/> class
		/// object should be initialized by using <see cref="VorbisInfo.Init"/>
		/// from the libvorbis API. After encoding,
		/// <see cref="VorbisInfo.Clear"/> should be called.
		/// <para>
		/// The <c>maxBitrate</c>, <c>nominalBitrate</c>, and <c>minBitrate</c>
		/// settings are used to set constraints for the encoded file. This
		/// function uses these settings to select the appropriate encoding
		/// mode and set it up.
		/// </para>
		/// </remarks>
		/// <param name="vi">
		/// An initialized <see cref="VorbisInfo"/> class object.
		/// </param>
		/// <param name="channels">
		/// The number of channels to be encoded.
		/// </param>
		/// <param name="rate">The sampling rate of the source audio.</param>
		/// <param name="maxBitrate">Desired maximum bitrate (limit).</param>
		/// <param name="nominalBitrate">
		/// Desired average, or central, bitrate.
		/// </param>
		/// <param name="minBitrate">Desired minimum bitrate.</param>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>
		/// 0 for success.
		/// </description></item>
		/// <item><description>
		/// Less than zero for failure:
		/// <para>
		/// OV_EFAULT - Internal logic fault; indicates a bug or heap/stack corruption. 
		/// </para>
		/// </description></item>
		/// </list>
		/// </returns>
		private static int Init(VorbisInfo vi,	int channels, int rate,			      
			int maxBitrate, int nominalBitrate, int minBitrate) 
		{
			if(vi == null)
				throw new ArgumentException("VorbisInfo vi cannot be null!");
			int retval = vorbis_encode_init(ref vi.vorbis_info, channels, rate,
				maxBitrate, nominalBitrate, minBitrate);
			vi.changed = true;
			return retval;
		}
		#endregion

		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_setup_managed(
			[In,Out]	ref vorbis_info	vi,
			int channels, int rate,
			int max_bitrate, int nominal_bitrate, int min_bitrate);
		/// <summary>
		/// </summary>
		public static int	SetupManaged(VorbisInfo vi, int channels, int rate,
			int max_bitrate, int nominal_bitrate, int min_bitrate) 
		{
			int retval = vorbis_encode_setup_managed(ref vi.vorbis_info,
				channels, rate, max_bitrate, nominal_bitrate, min_bitrate);
			vi.changed = true;
			return retval;
		}
  
		/// <summary>
		/// </summary>
		/// <param name="vi"></param>
		/// <param name="channels"></param>
		/// <param name="rate"></param>
		/// <param name="quality">Quality level from 0. (lo) to 1. (hi)</param>
		/// <returns></returns>
		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_setup_vbr(
			[In,Out]	ref vorbis_info	vi,
			int channels, int rate, float quality);
		/// <summary>
		/// </summary>
		public static int SetupVbr(VorbisInfo vi, int channels, int rate, float quality) 
		{
			int retval = vorbis_encode_setup_vbr(ref vi.vorbis_info, channels,
				rate, quality);
			vi.changed = true;
			return retval;
		}

		/// <summary>
		/// </summary>
		/// <param name="vi"></param>
		/// <param name="channels"></param>
		/// <param name="rate"></param>
		/// <param name="base_quality">Quality level from 0. (lo) to 1. (hi).</param>
		/// <returns></returns>
		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_init_vbr(
			[In,Out]	ref vorbis_info	vi,
			int channels, int rate, float base_quality);
		/// <summary>
		/// </summary>
		public static int InitVbr(VorbisInfo vi, int channels, int rate, float base_quality) 
		{
			int retval = vorbis_encode_init_vbr(ref vi.vorbis_info, channels, rate, base_quality);
			vi.changed = true;
			return retval;
		}

		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_setup_init(
			[In,Out]	ref vorbis_info	vi);
		/// <summary>
		/// </summary>
		public static int SetupInit(VorbisInfo vi) 
		{
			int retval = vorbis_encode_setup_init(ref vi.vorbis_info);
			vi.changed = true;
			return retval;
		}

		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_ctl(
			[In]	ref vorbis_info	vi, OvEctl number, ref double arg); // void *
		/// <summary>
		/// </summary>
		public static int Ctl(VorbisInfo vi, OvEctl number, ref double arg) 
		{
			return vorbis_encode_ctl(ref vi.vorbis_info, number, ref arg);
		}
		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_ctl(
			[In]		ref vorbis_info				vi, OvEctl number,
			[In,Out]	ref ovectl_ratemanage_arg	arg); // void *
		[DllImport(DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_encode_ctl(
			[In]		ref vorbis_info				vi, OvEctl number,
			[In]		IntPtr						ptr); // void *
		/// <summary>
		/// </summary>
		public static int Ctl(VorbisInfo vi, OvEctl number, OvectlRatemanageArg arg) 
		{
			int retval;
			if(arg == null)
				retval = vorbis_encode_ctl(ref vi.vorbis_info, number, IntPtr.Zero);
			else 
			{
				retval = vorbis_encode_ctl(ref vi.vorbis_info, number, ref arg.ovectl_ratemanage_arg);
				arg.changed = true;
			}
			return retval;
		}
	}
}
