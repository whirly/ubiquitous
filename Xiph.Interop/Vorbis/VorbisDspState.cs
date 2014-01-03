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
	/// vorbis_dsp_state buffers the current vorbis audio
	/// analysis/synthesis state.  The DSP state belongs to a specific
	/// logical bitstream
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct vorbis_dsp_state
	{
		internal int		analysisp;
		internal IntPtr		vi;

		internal IntPtr		pcm;	// float **
		internal IntPtr		pcmret;	// float **
		internal int		pcm_storage;
		internal int		pcm_current;
		internal int		pcm_returned;

		internal int		preextrapolate;
		internal int		eofflag;

		internal int		lW;
		internal int		W;
		internal int		nW;
		internal int		centerW;

		internal long	granulepos;
		internal long	sequence;

		internal long	glue_bits;
		internal long	time_bits;
		internal long	floor_bits;
		internal long	res_bits;

		internal IntPtr	backend_state;	// void *
	}

	/// <summary>
	/// </summary>
	public class VorbisDspState
	{
		#region Variables
		internal vorbis_dsp_state vorbis_dsp_state = new vorbis_dsp_state();
		private VorbisInfo vi;
		internal bool changed = true;
		#endregion

		#region Constructor(s) & Destructor
		/// <summary>
		/// </summary>
		/// <param name="vi"></param>
		public VorbisDspState(VorbisInfo vi) 
		{
			this.vi = vi;
		}
		#endregion

		#region Properties
		/// <summary>
		/// </summary>
		public VorbisInfo Vi 
		{
			get 
			{
				if(this.changed) 
				{
					this.vi.vorbis_info = (vorbis_info) Marshal.PtrToStructure(
						this.vorbis_dsp_state.vi, typeof(vorbis_info));
					this.changed = false;
				}
				return this.vi;
			}
		}
		/// <summary>
		/// </summary>
		public int PcmStorage 
		{
			get { return this.vorbis_dsp_state.pcm_storage; }
		}
		/// <summary>
		/// </summary>
		public int PcmCurrent 
		{
			get { return this.vorbis_dsp_state.pcm_current; }
		}
		/// <summary>
		/// </summary>
		public long Sequence 
		{
			get { return this.vorbis_dsp_state.sequence; }
		}
		#endregion

		#region Vorbis PRIMITIVES: general ***************************************

		#region vorbis_dsp_clear
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_dsp_clear([In,Out] ref vorbis_dsp_state v);
		/// <summary>
		/// </summary>
		/// <param name="v"></param>
		public static void Clear(VorbisDspState v) 
		{
			vorbis_dsp_clear(ref v.vorbis_dsp_state);
			v.changed = true;
		}
		#endregion

		#endregion
	}
}
