﻿#region Copyright & License Information
/*
 * Copyright 2007-2012 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.IO;
using OpenRA.FileFormats;
using OpenRA.Traits;
using OpenRA.Graphics;

namespace OpenRA.Mods.RA
{
	class PaletteFromR8Info : ITraitInfo
	{
		[Desc("Internal palette name")]
		public readonly string Name = null;
		[Desc("Filename to load")]
		public readonly string Filename = null;
		[Desc("Palette byte offset")]
		public readonly long Offset = 0;
		public readonly bool AllowModifiers = true;
		
		public object Create(ActorInitializer init) { return new PaletteFromR8(this); }
	}

	class PaletteFromR8 : IPalette
	{
		readonly PaletteFromR8Info info;
		public PaletteFromR8(PaletteFromR8Info info) { this.info = info; }

		public void InitPalette(WorldRenderer wr)
		{
			var colors = new uint[256];
			using (var s = FileSystem.Open(info.Filename))
			{
				s.Seek(info.Offset, SeekOrigin.Begin);

				for (var i = 0; i < 256; i++)
				{
					// The custom palette is scaled into the range 0-128.
					// This makes the move-flash match the original game, but may not be correct in other cases.
					var packed = s.ReadUInt16();
					colors[i] = (uint)((255 << 24) | ((packed & 0xF800) << 7) | ((packed & 0x7E0) << 4) | ((packed & 0x1f) << 2));
				}
			}

			wr.AddPalette(info.Name, new Palette(colors), info.AllowModifiers);
		}
	}
}
