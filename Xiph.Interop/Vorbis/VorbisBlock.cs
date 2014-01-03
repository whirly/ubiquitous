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

using Xiph.Interop.Ogg;

namespace Xiph.Interop.Vorbis
{
	/// <summary>
	/// vorbis_block is a single block of data to be processed as part of
	/// the analysis/synthesis stream; it belongs to a specific logical
	/// bitstream, but is independant from other vorbis_blocks belonging to
	/// that logical bitstream.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct vorbis_block
	{
		/* necessary stream state for linking to the framing abstraction */
		/// <summary>this is a pointer into local storage</summary>
		internal IntPtr		pcm;	// float  **
		internal oggpack_buffer	opb;
  
		internal int		lW;
		internal int		W;
		internal int		nW;
		internal int		pcmend;
		internal int		mode;

		internal int		eofflag;
		internal long		granulepos;
		internal long		sequence;
		/// <summary>For read-only access of configuration</summary>
		internal IntPtr		vd;

		/// <summary>
		/// local storage to avoid remallocing; it's up to the mapping to
		/// structure it
		/// </summary>
		internal IntPtr		localstore;	// void *
		internal int		localtop;
		internal int		localalloc;
		internal int		totaluse;
		internal IntPtr		reap;

		#region bitmetrics for the frame
		internal int	glue_bits;
		internal int	time_bits;
		internal int	floor_bits;
		internal int	res_bits;
		#endregion

		internal IntPtr	pinternal;	// void *internal;
	}

	/// <summary>
	/// </summary>
	public class VorbisBlock
	{
		#region Variables
		internal vorbis_block vorbis_block = new vorbis_block();
		internal bool changed = true;
//		private VorbisDspState vd;
		#endregion

		#region Properties
//		public vorbis_dsp_state vd 
//		{
//			get { return (vorbis_dsp_state) Marshal.PtrToStructure(this.pvd, typeof(vorbis_dsp_state));	}
//		}
//
//		internal alloc_chain reap 
//		{
//			get { return (alloc_chain) Marshal.PtrToStructure(this.preap, typeof(alloc_chain));	}
//		}
		#endregion

		#region Vorbis PRIMITIVES: general ***************************************

		#region vorbis_block_init
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_block_init(
			[In]		ref vorbis_dsp_state	v,
			[In,Out]	ref vorbis_block		vb);
		/// <summary>
		/// </summary>
		/// <param name="v"></param>
		/// <param name="vb"></param>
		/// <returns></returns>
		public static int Init(VorbisDspState v, VorbisBlock vb) 
		{
			if(v == null)
				throw new ArgumentException("VorbisDspState v cannot be null!");
			if(vb == null)
				throw new ArgumentException("VorbisBlock vb cannot be null!");
			int retval = vorbis_block_init(ref v.vorbis_dsp_state, ref vb.vorbis_block);
			vb.changed = true;
			return retval;
		}
		#endregion

		#region vorbis_block_clear
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_block_clear(
			[In,Out]	ref vorbis_block	vb);
		/// <summary>
		/// </summary>
		/// <param name="vb"></param>
		/// <returns></returns>
		public static int Clear(VorbisBlock vb) 
		{
			if(vb == null)
				throw new ArgumentException("VorbisBlock vb cannot be null!");
			int retval = vorbis_block_clear(ref vb.vorbis_block);
			vb.changed = true;
			return retval;
		}
		#endregion

		#endregion
	}
}
