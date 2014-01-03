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
	/// ogg_packet is used to encapsulate the data and metadata belonging
	/// to a single raw Ogg/Vorbis packet
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ogg_packet
	{
		internal IntPtr	packet;	// unsigned char *
		internal int	bytes;
		internal int	b_o_s;
		internal int	e_o_s;

		internal long	granulepos;
  
		/// <summary>
		/// sequence number for decode; the framing
		/// knows where there's a hole in the data,
		/// but we need coupling so that the codec
		/// (which is in a seperate abstraction
		/// layer) also knows about the gap
		/// </summary>
		internal long	packetno;
	}

	/// <summary>
	/// The OggPacket class encapsulates the data for a single raw packet of
	/// data and is used to transfer data between the ogg framing layer and the
	/// handling codec. 
	/// </summary>
	public class OggPacket
	{
		#region Variables
		internal ogg_packet ogg_packet = new ogg_packet();
		private bool memAllocated = false;
		internal bool changed = true;
		private byte[] packet = null;
		#endregion

		#region Constructor(s) and Destructor
		/// <summary>
		/// </summary>
		~OggPacket() 
		{
			if(this.memAllocated)
                Marshal.FreeHGlobal(this.ogg_packet.packet);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This is treated as an opaque type by the ogg layer.
		/// </summary>
		public byte [] Packet 
		{
			get 
			{
				if(this.changed) 
				{
					packet = new byte[this.Bytes];
					Marshal.Copy(this.ogg_packet.packet, packet, 0,
						this.Bytes);
					this.changed = false;
				}
				return packet;
			}
			set 
			{
				if(this.memAllocated)
					Marshal.FreeHGlobal(this.ogg_packet.packet);
				else
					this.memAllocated = true;
				this.ogg_packet.packet = Marshal.AllocHGlobal(this.Bytes);
				Marshal.Copy(value, 0, this.ogg_packet.packet, this.Bytes);
			}
		}
		/// <summary>
		/// Indicates the size of the packet data in bytes. Packets can be of
		/// arbitrary size.
		/// </summary>
		public int Bytes 
		{
			get { return this.ogg_packet.bytes; }
			set { this.ogg_packet.bytes = value; }
		}
		/// <summary>
		/// Flag indicating whether this packet begins a logical bitstream. 1
		/// indicates this is the first packet, 0 indicates any other position
		/// in the stream. 
		/// </summary>
		public int B_o_s 
		{
			get { return this.ogg_packet.b_o_s; }
			set { this.ogg_packet.b_o_s = value; }
		}
		/// <summary>
		/// Flag indicating whether this packet ends a bitstream. 1 indicates
		/// the last packet, 0 indicates any other position in the stream.
		/// </summary>
		public int E_o_s 
		{
			get { return this.ogg_packet.e_o_s; }
			set { this.ogg_packet.e_o_s = value; }
		}
		/// <summary>
		/// A number indicating the position of this packet in the decoded
		/// data. This is the last sample, frame or other unit of information
		/// ('granule') that can be completely decoded from this packet.
		/// </summary>
		public long Granulepos 
		{
			get { return this.ogg_packet.granulepos; }
			set { this.ogg_packet.granulepos = value; }
		}
		/// <summary>
		/// Sequential number of this packet in the ogg bitstream.
		/// </summary>
		public long	Packetno 
		{
			get { return this.ogg_packet.packetno; }
			set { this.ogg_packet.packetno = value; }
		}
		#endregion

		#region Ogg BITSTREAM PRIMITIVES: general ***************************

		#region ogg_packet_clear
		[DllImport(Ogg.DllFile, CallingConvention=CallingConvention.Cdecl)]
		private static extern void	ogg_packet_clear(
			[In,Out]	ref ogg_packet	op);
		/// <summary>
		/// This function clears the memory used by the <see cref="OggPacket"/>
		/// class, and frees the internal allocated memory, but does not free
		/// the class itself. 
		/// </summary>
		/// <param name="op">
		/// The <see cref="OggPacket"/> class to be cleared.
		/// </param>
		public static void Clear(OggPacket op) 
		{
			if(op == null)
				throw new ArgumentException("OggPacket op cannot be null!");
			ogg_packet_clear(ref op.ogg_packet);
			op.changed = true;
		}
		#endregion

		#endregion
	}
}
