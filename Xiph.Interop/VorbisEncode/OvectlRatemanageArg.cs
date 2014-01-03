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
using System.Runtime.InteropServices;

namespace Xiph.Interop.VorbisEncode
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ovectl_ratemanage_arg 
	{
		internal int	management_active;

		internal int	bitrate_hard_min;
		internal int	bitrate_hard_max;
		internal double	bitrate_hard_window;

		internal int	bitrate_av_lo;
		internal int	bitrate_av_hi;
		internal double	bitrate_av_window;
		internal double	bitrate_av_window_center;
	}

	/// <summary>
	/// </summary>
	public class OvectlRatemanageArg
	{
		#region Variables
		internal ovectl_ratemanage_arg ovectl_ratemanage_arg = new ovectl_ratemanage_arg();
		internal bool changed = true;
		#endregion

		/// <summary>
		/// </summary>
		public int ManagementActive 
		{
			get { return this.ovectl_ratemanage_arg.management_active; }
			set { this.ovectl_ratemanage_arg.management_active = value; }
		}
		/// <summary>
		/// </summary>
		public int BitrateHardMin
		{
			get { return this.ovectl_ratemanage_arg.bitrate_hard_min; }
			set { this.ovectl_ratemanage_arg.bitrate_hard_min = value; }
		}
		/// <summary>
		/// </summary>
		public int BitrateHardMax
		{
			get { return this.ovectl_ratemanage_arg.bitrate_hard_max; }
			set { this.ovectl_ratemanage_arg.bitrate_hard_max = value; }
		}
		/// <summary>
		/// </summary>
		public double BitrateHardWindow
		{
			get { return this.ovectl_ratemanage_arg.bitrate_hard_window; }
			set { this.ovectl_ratemanage_arg.bitrate_hard_window = value; }
		}
		/// <summary>
		/// </summary>
		public int BitrateAvLo 
		{
			get { return this.ovectl_ratemanage_arg.bitrate_av_lo; }
			set { this.ovectl_ratemanage_arg.bitrate_av_lo = value; }
		}
		/// <summary>
		/// </summary>
		public int BitrateAvHi 
		{
			get { return this.ovectl_ratemanage_arg.bitrate_av_hi; }
			set { this.ovectl_ratemanage_arg.bitrate_av_hi = value; }
		}
		/// <summary>
		/// </summary>
		public double BitrateAvWindow 
		{
			get { return this.ovectl_ratemanage_arg.bitrate_av_window; }
			set { this.ovectl_ratemanage_arg.bitrate_av_window = value; }
		}
		/// <summary>
		/// </summary>
		public double BitrateAvWindowCenter 
		{
			get { return this.ovectl_ratemanage_arg.bitrate_av_window_center; }
			set { this.ovectl_ratemanage_arg.bitrate_av_window_center = value; }
		}
	}
}
