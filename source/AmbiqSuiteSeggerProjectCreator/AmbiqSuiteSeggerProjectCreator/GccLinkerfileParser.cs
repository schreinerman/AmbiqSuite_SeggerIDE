/*
 * Created by SharpDevelop.
 * 
 * Created by Manuel Schreiner
 * Copyright © 2021 Manuel Schreiner. All rights reserved.
 *
 * Redistributions of binary or source code must retain the above copyright notice, 
 * this condition and the following disclaimer.
 *
 * This software is provided by the copyright holder and contributors "AS IS" 
 * and any warranties related to this software are DISCLAIMED. 
 * The copyright owner or contributors be NOT LIABLE for any damages caused by use of this software.
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace AmbiqSuiteSeggerProjectCreator
{
	/// <summary>
	/// Description of GccLinkerfileParser.
	/// </summary>
	public class GccLinkerfileParser
	{
		public UInt32 RamStart = 0;
		public UInt32 RamSize = 0;
		public UInt32 RomStart = 0;
		public UInt32 RomSize = 0;
		
		/// <summary>
		/// Convert a number in a string to Uint32 Number
		/// </summary>
		/// <param name="Number">String containing the number</param>
		/// <returns>UInt32 Number</returns>
		static UInt32 ConvertNumber(String Number)
		{
			UInt32 Multiplier = 1;
			Number = Number.Trim();
			if (Number.EndsWith("K",StringComparison.CurrentCultureIgnoreCase))
			{
				Multiplier = 1024;
				Number = Number.Remove(Number.Length - 1,1);
			} else if (Number.EndsWith("KB",StringComparison.CurrentCultureIgnoreCase))
			{
				Multiplier = 1024;
				Number = Number.Remove(Number.Length - 2,2);
			} else if (Number.EndsWith("M",StringComparison.CurrentCultureIgnoreCase))
			{
				Multiplier = 1024 * 1024;
				Number = Number.Remove(Number.Length - 1,1);
			} else if (Number.EndsWith("MB",StringComparison.CurrentCultureIgnoreCase))
			{
				Multiplier = 1024 * 1024;
				Number = Number.Remove(Number.Length - 2,2);
			} else if (Number.EndsWith("G",StringComparison.CurrentCultureIgnoreCase))
			{
				Multiplier = 1024 * 1024 * 1024;
				Number = Number.Remove(Number.Length - 1,1);
			} else if (Number.EndsWith("GB",StringComparison.CurrentCultureIgnoreCase))
			{
				Multiplier = 1024 * 1024 * 1024;
				Number = Number.Remove(Number.Length - 2,2);
			}
			if (Number.StartsWith("0x",StringComparison.CurrentCultureIgnoreCase))
			{
				return Convert.ToUInt32(Number,16) * Multiplier;
			}
			return UInt32.Parse(Number,System.Globalization.NumberStyles.Any) * Multiplier;
		}
		
		public GccLinkerfileParser(String LinkerFile)
		{
			int i;
			//
			// Read the linker file
			//
			String[] DataLines = File.ReadAllLines(LinkerFile);
			
			for(i = 0;i < DataLines.Length;i++)
			{
				//
				// If memory definition starts, process data inside
				//
				if (DataLines[i].Contains("MEMORY"))
				{
					i++;
					for(;i < DataLines.Length;i++)
					{
						//
						// FLASH / RAM definition
						//
						if (DataLines[i].Contains("(rx)") || DataLines[i].Contains("(rwx)"))
						{
							Boolean IsRam = false;
							UInt32 Origin = 0;
							UInt32 Length = 0;
							
							//
							// (rwx) mean RAM
							//
							if (DataLines[i].Contains("(rwx)")) IsRam = true;
							
							//
							// Split name and memory section definition
							//
							String[] Information = DataLines[i].Split(new char[]{':'},StringSplitOptions.RemoveEmptyEntries);
							if (Information.Length > 1)
							{
								//
								// Split memory section parameters
								//
								String[] Parameters = Information[1].Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
								foreach(String Parameter in Parameters)
								{
									//
									// Split Parameter name and value
									//
									String[] ParameterWords = Parameter.Split(new char[]{'='},StringSplitOptions.RemoveEmptyEntries);
									if (ParameterWords.Length > 1)
									{
										if (ParameterWords[0].Contains("ORIGIN"))
										{
											Origin = ConvertNumber(ParameterWords[1]);
										} else if (ParameterWords[0].Contains("LENGTH"))
										{
											Length = ConvertNumber(ParameterWords[1]);
										}
									}
								}
							}
							
							//
							// At the end, trasfer information
							//
							if (IsRam)
							{
								RamStart = Origin;
								RamSize = Length;
							} else
							{
							    RomStart = Origin;
								RomSize = Length;
							}
						}
						if (DataLines[i].Contains("}"))
					    {
							break;
					    }
					}
				}
			}
		}
	}
}
