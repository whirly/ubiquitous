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
	/// the comments are not part of vorbis_info so that vorbis_info can be
	/// static storage
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct vorbis_comment
	{
		/// <summary>
		/// unlimited user comment fields.  libvorbis writes 'libvorbis'
		/// whatever vendor is set to in encode
		/// </summary>
		internal IntPtr	user_comments;	// char **
		internal IntPtr	comment_lengths;	// int *
		internal int	comments;
		[MarshalAs(UnmanagedType.LPStr)]
		internal string	vendor;
	}

	/// <summary>
	/// The VorbisComment class defines an Ogg Vorbis comment.
	/// </summary>
	/// <remarks>
	/// Only the fields the program needs must be defined. If a field isn't
	/// defined by the application, it will either be blank (if it's a string
	/// value) or set to some reasonable default (usually 0).
	/// </remarks>
	public class VorbisComment
	{
		#region Variables
		internal vorbis_comment vorbis_comment = new vorbis_comment();
		internal bool changed = true;
		private VorbisComments userComments;
		private VorbisCommentLengths commentLengths;
		#endregion

		private void MarshalData() 
		{
			if(this.changed) 
			{
				this.commentLengths = new VorbisCommentLengths(
					this.vorbis_comment.comment_lengths,
					this.vorbis_comment.comments);
				this.userComments = new VorbisComments(
					this.vorbis_comment.user_comments, this.commentLengths,
					this.vorbis_comment.comments);
				this.changed = false;
			}
		}
		
		#region Properties
		/// <summary>
		/// Unlimited user comment array. The individual strings in the array
		/// are 8 bit clean, by the Vorbis specification, and as such the
		/// <see cref="CommentLengths"/> array should be consulted to determine
		/// string length. For convenience, each string is also NULL-terminated
		/// by the decode library (although Vorbis comments are not NULL
		/// terminated within the bitstream itself).
		/// </summary>
		public VorbisComments UserComments 
		{
			get 
			{ 
				this.MarshalData();
				return this.userComments;
			}
		}
		/// <summary>
		/// An int array that stores the length of each comment string.
		/// </summary>
		public VorbisCommentLengths CommentLengths 
		{
			get 
			{
				this.MarshalData();
				return this.commentLengths;
			}
		}
		/// <summary>
		/// Int signifying number of user comments in
		/// <see cref="UserComments"/> field. 
		/// </summary>
		public int Comments 
		{
			get { return this.vorbis_comment.comments; }
		}
		/// <summary>
		/// Information about the creator of the file. Stored in a standard C
		/// 0-terminated string.
		/// </summary>
		public string Vendor 
		{
			get { return this.vorbis_comment.vendor; }
		}
		#endregion

		#region Vorbis PRIMITIVES: general ***************************************

		#region vorbis_comment_init
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_comment_init(
			[In,Out]	ref vorbis_comment	vc);
		/// <summary>
		/// </summary>
		/// <param name="vc"></param>
		public static void Init(VorbisComment vc) 
		{
			if(vc == null)
				throw new ArgumentException("VorbisComment vc cannot be null!");
			vorbis_comment_init(ref vc.vorbis_comment);
			vc.changed = true;
		}
		#endregion

		#region vorbis_comment_add
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_comment_add(
			[In,Out]	ref vorbis_comment	vc, IntPtr comment); // char *
		/// <summary>
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="comment"></param>
		public static void Add(VorbisComment vc, byte[] comment) 
		{
			if(vc == null)
				throw new ArgumentException("VorbisComment vc cannot be null!");
			IntPtr buffer = Marshal.AllocHGlobal(comment.Length);
			Marshal.Copy(comment, 0, buffer, comment.Length);
			vorbis_comment_add(ref vc.vorbis_comment, buffer);
			vc.changed = true;
			Marshal.FreeHGlobal(buffer);
		}
		#endregion

		#region vorbis_comment_add_tag
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_comment_add_tag([In,Out] ref vorbis_comment vc, 
			[In] string tag, IntPtr contents); // char *
		/// <summary>
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="tag"></param>
		/// <param name="contents"></param>
		public static void AddTag(VorbisComment vc, string tag, byte[] contents) 
		{
			if(vc == null)
				throw new ArgumentException("VorbisComment vc cannot be null!");
			IntPtr buffer = Marshal.AllocHGlobal(contents.Length);
			Marshal.Copy(contents, 0, buffer, contents.Length);
			vorbis_comment_add_tag(ref vc.vorbis_comment, tag, buffer);
			vc.changed = true;
			Marshal.FreeHGlobal(buffer);
		}
		#endregion

		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr	vorbis_comment_query([In] ref vorbis_comment vc, IntPtr tag, int count); // char	*
//		private static IntPtr Query(VorbisComment vc, IntPtr tag, int count) 
//		{
//		}
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern int	vorbis_comment_query_count([In] ref vorbis_comment vc, IntPtr tag); // char *
//		private static int QueryCount(VorbisComment vc, IntPtr tag) 
//		{
//		}

		#region vorbis_comment_clear
		[DllImport(Vorbis.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	vorbis_comment_clear(
			[In,Out]	ref vorbis_comment	vc);
		/// <summary>
		/// </summary>
		/// <param name="vc"></param>
		public static void Clear(VorbisComment vc) 
		{
			vorbis_comment_clear(ref vc.vorbis_comment);
			vc.changed = true;
		}
		#endregion

		#endregion
	}
}
