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
using System.Collections;
using System.Collections.Generic;
namespace AmbiqSuiteCrossworksProjectCreator
{
	/// <summary>
	/// Description of GccMakefileParser.
	/// </summary>
	public class GccMakefileParser
	{
		
		public DirectoryInfo Root;
		public SortedList<String,String> Vars;
		public SortedList<String,FileInfo> SourceFiles;
		public SortedList<String,DirectoryInfo> IncludePaths;
		public List<String> Defines;
		public SortedList<String,FileInfo> LibraryFiles;
		public String CpuString = "";
		public String FpuType = "";
		public String FloatType = "";
		
		public GccMakefileParser(String MakeFile)
		{
			FileInfo fIfoMakeFile = new FileInfo(MakeFile);
			Root = fIfoMakeFile.Directory;
			Vars = new SortedList<string, string>();
			SourceFiles = new SortedList<string, FileInfo>();
			IncludePaths = new SortedList<string, DirectoryInfo>();
			LibraryFiles = new SortedList<string, FileInfo>();
			Defines = new List<string>();
			
			String[] DataLines = File.ReadAllLines(MakeFile);
			
			//
			// Process all data-lines
			//
			foreach(String DataLine in DataLines)
			{
				// split makefile lines by spaces => results in words
				String[] DataItems = DataLine.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
				
				if (DataItems.Length > 0)
				{
					//
					// sometimes +=: does not have spaces, so if first word contains +=: which should be in DataItems[1]
					// replace +=: with " += : " and split again
					//
					if (DataItems[0].Contains("+=:")) 
					{
						String DataLineTmp = DataLine.Replace(DataItems[0],DataItems[0].Replace("+=:"," += :"));
						DataItems = DataLineTmp.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
					}
					//
					// sometimes += does not have spaces, so if first word contains += which should be in DataItems[1]
					// replace += with " += " and split again
					//
					if (DataItems[0].Contains("+="))
					{
						String DataLineTmp = DataLine.Replace(DataItems[0],DataItems[0].Replace("+="," += "));
						DataItems = DataLineTmp.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
					} 
					
					//
					// sometimes = does not have spaces, so if first word contains = which should be in DataItems[1]
					// replace = with " = " and split again
					//
					else if (DataItems[0].Contains("="))
					{
						String DataLineTmp = DataLine.Replace(DataItems[0],DataItems[0].Replace("="," = "));
						DataItems = DataLineTmp.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
					}
				}
				if (DataItems.Length > 2)
				{
					//
					// check second word is "=" which starts a new variable
					//
					if (DataItems[1].Equals("="))
					{
						if (Vars.ContainsKey(DataItems[0]))
						{
							Vars.Remove(DataItems[0]);
						} 
						String Val = DataLine.Substring(DataLine.IndexOf('=')+1);
						Vars.Add(DataItems[0],Val.TrimStart());
					}
					
					//
					// check second word is "+=" which appends a variable
					//
					else if (DataItems[1].Equals("+="))
					{
						if (!Vars.ContainsKey(DataItems[0]))
						{
							Vars.Add(DataItems[0],"");
						} 
						String Val = DataLine.Substring(DataLine.IndexOf('=')+1);
						Vars[DataItems[0]] += " " + Val.TrimStart();
					}
				}
				
			}
			
			//
			// Search for source files in the specified VPATH variable
			//
			String[] VPATH = Vars["VPATH"].Replace(":","").Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
			String[] SRC = Vars["SRC"].Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
			foreach(String SourceFile in SRC)
			{
				foreach(String SearchDir in VPATH)
				{
					String absSearchDir = Path.Combine(Root.FullName,SearchDir.Replace("/","\\"));
					absSearchDir = Path.GetFullPath((new Uri(absSearchDir)).LocalPath);
					String checkFilePath = Path.Combine(absSearchDir,SourceFile);
					if (File.Exists(checkFilePath))
					{
						SourceFiles.Add(SourceFile,new FileInfo(checkFilePath));
						break;
					}
				}
			}
			
			//
			// Process include paths
			//
			String[] INCLUDES = Vars["INCLUDES"].Split(new String[]{"-I"},StringSplitOptions.RemoveEmptyEntries);
			Vars.Add("INCLUDESSTRING","");
			foreach(String Include in INCLUDES)
			{
				String IncludePath = Path.Combine(Root.FullName,Include.TrimStart().Replace("/","\\"));
				IncludePath = Path.GetFullPath((new Uri(IncludePath)).LocalPath);
				IncludePaths.Add(Include.TrimStart(),new DirectoryInfo(IncludePath));
				Vars["INCLUDESSTRING"] += Include.Trim() + ";";
			}
			
			//
			// Process libraries
			//
			if (Vars.ContainsKey("LIBS"))
			{
				String[] LIBS = Vars["LIBS"].Split(new String[]{" "},StringSplitOptions.RemoveEmptyEntries);
				Vars.Add("LIBSSTRING","");
				foreach(String Lib in LIBS)
				{
					String LibPath = Path.Combine(Root.FullName,Lib.TrimStart().Replace("/","\\"));
				    LibPath = Path.GetFullPath((new Uri(LibPath)).LocalPath);
					Vars["LIBSSTRING"] += Lib.Trim() + ";";
					LibraryFiles.Add(Lib,new FileInfo(LibPath));
				}
			}
			
			//
			// Generate defines string
			//
			String[] DEFINES = Vars["DEFINES"].Split(new String[]{"-D"},StringSplitOptions.RemoveEmptyEntries);
			Vars.Add("DEFINESSTRING","");
			foreach(String Define in DEFINES)
			{
				if (!String.IsNullOrEmpty(Define.Trim()))
				{
				    Defines.Add(ReplaceVars(Define.Trim()));
				    String DefineVar = ReplaceVars(Define.Trim());
				    if (DefineVar.Contains("="))
				    {
				    	Vars["DEFINESSTRING"] += DefineVar + ";";
				    }
				    else{
				    	Vars["DEFINESSTRING"] += DefineVar + "=1;";
				    }
				}
			}
			
			Vars.Add("CFLAGSSTRING",ReplaceVars(Vars["CFLAGS"]));
		}
		
		/// <summary>
		/// Replace variables in a string
		/// </summary>
		/// <param name="InputString">Input string</param>
		/// <returns>Output string</returns>
		public String ReplaceVars(String InputString)
		{
			foreach(KeyValuePair<String,String> kpVar in Vars)
			{
				InputString = InputString.Replace("$(" + kpVar.Key + ")",kpVar.Value);
			}
			return InputString;
		}
		
	}
}
