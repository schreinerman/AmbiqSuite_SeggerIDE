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
	class Program
	{
		public static void ProcessFolder(String TargetFolder)
		{
			Console.WriteLine("Processing: " + TargetFolder);
			DirectoryInfo dInfoTarget = new DirectoryInfo(TargetFolder);
			
			FileInfo[] fIfoLinker = (new DirectoryInfo(Path.Combine(TargetFolder,"gcc"))).GetFiles("*.ld");
			GccLinkerfileParser lParser = null;
			
			if (fIfoLinker.Length > 0)
			{
				lParser = new GccLinkerfileParser(fIfoLinker[0].FullName);
			}
			
		    //
		    // Parse the makefile and convert into magic data...
		    //
		    GccMakefileParser mParser = new GccMakefileParser(Path.Combine(Path.Combine(Path.Combine(TargetFolder,"gcc"),"Makefile")));
			
			//
			// Create segger sub-directory if not existing
			//
			if (!Directory.Exists(Path.Combine(TargetFolder,"segger")))
			{
				Directory.CreateDirectory(Path.Combine(TargetFolder,"segger"));
			}
			
			if (dInfoTarget.Name.Equals("bsp") || dInfoTarget.Name.Equals("hal"))
			{
				//
				// Process as a library
				//
				SeggerEmProject emPrj = new SeggerEmProject("libam_" + dInfoTarget.Name,Path.Combine(TargetFolder,"segger"),mParser,lParser,true);
			    emPrj.Create();
			} else 
			{
				//
				// Process as a normal project
				//
				SeggerEmProject emPrj = new SeggerEmProject(dInfoTarget.Name,Path.Combine(TargetFolder,"segger"),mParser,lParser,false);
			    emPrj.Create();
			    
			    //
			    // A normal project needs also a Segger-IDE compatible startup - read-in GCC startup and converting to Segger style
			    //
			    FileInfo[] fIfos = (new DirectoryInfo(Path.Combine(TargetFolder,"gcc"))).GetFiles("startup_*.c");
			    if (fIfos.Length > 0)
			    {
				    GccStartupParser sParser = new GccStartupParser(fIfos[0].FullName);
				    sParser.Create(Path.Combine(Path.Combine(TargetFolder, "segger"),emPrj.StartupFileName));
			    } else
			    {
			    	Console.WriteLine("  WARNING: No startup-file found");
			    }
			}
			
		}
		public static void Main(string[] args)
		{
			DirectoryInfo dInfo = null;
			
			//
			// Program started with arguments? If yes, use this as working directory
			// otherwise use the program path as working directory
			//
			if (args.Length > 0)
			{
				if (Directory.Exists(args[0]))
			    {
					dInfo = new DirectoryInfo(args[0]);
					Console.WriteLine("Processing files in specified directory : ");
				    Console.WriteLine(dInfo.FullName);
				}
			} else
			{
				FileInfo fExe = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
				dInfo = fExe.Directory;
				Console.WriteLine("Processing files in executables directory : ");
				Console.WriteLine(dInfo.FullName);
			}
			
			// No valid working directory - end program
			if (dInfo == null) return;
			if (!dInfo.Exists) return;
			
			Console.WriteLine("");
			
			//
			// If there is a sub-folder bsp, create a library project for the bsp package
			//
			if (Directory.Exists(Path.Combine(dInfo.FullName,"bsp"))) ProcessFolder(Path.Combine(dInfo.FullName,"bsp"));
		    
		    //
		    // If there is a subfolder MCU, create the library projects for the MCU hal
		    //
		    if (Directory.Exists(Path.Combine(dInfo.FullName,"mcu")))
			{
		    	DirectoryInfo dInfoMcu = new DirectoryInfo(Path.Combine(dInfo.FullName,"mcu"));
				foreach(DirectoryInfo dInfoMcuSeries in dInfoMcu.GetDirectories())
				{
					if (Directory.Exists(Path.Combine(dInfoMcuSeries.FullName,"hal"))) ProcessFolder(Path.Combine(dInfoMcuSeries.FullName ,"hal"));
				}
			}
				
		    //
		    // If there is a subfolder GCC, process only this example project / library project 
		    //
		    if (Directory.Exists(Path.Combine(dInfo.FullName , "gcc")))
    	    {
				ProcessFolder(dInfo.FullName);
			} 
			//
		    // If this folder is gcc, execute from the parent folder as working directory
            // to process only this example project / library project		    
		    //
			else if (dInfo.Name.Equals("gcc"))
			{
			    ProcessFolder(dInfo.Parent.FullName);
			} 
			
			//
		    // If this folder contains boards as subfolder, process all example projects and board support libraries	    
		    //
		    else if (Directory.Exists(Path.Combine(dInfo.FullName,"boards")))
			{
		    	DirectoryInfo dInfoBoards = new DirectoryInfo(Path.Combine(dInfo.FullName, "boards"));
				foreach(DirectoryInfo dInfoBoard in dInfoBoards.GetDirectories())
		        {
					DirectoryInfo dInfoBsp = new DirectoryInfo(Path.Combine(dInfoBoard.FullName,"bsp"));
				    if (dInfoBsp.Exists) ProcessFolder(dInfoBsp.FullName);
				    
				    DirectoryInfo dInfoExamples = new DirectoryInfo(Path.Combine(dInfoBoard.FullName,"examples"));
				    foreach(DirectoryInfo Example in dInfoExamples.GetDirectories())
				    {
				    	ProcessFolder(Example.FullName);
				    }
		        }
			} 
			
			//
		    // If this folder contains examples as subfolder, process all example projects of a board	    
		    //
		    else if (Directory.Exists(Path.Combine(dInfo.FullName,"examples")))
			{
		    	DirectoryInfo dInfoExamples = new DirectoryInfo(Path.Combine(dInfo.FullName,"examples"));
			    foreach(DirectoryInfo Example in dInfoExamples.GetDirectories())
			    {
			    	ProcessFolder(Example.FullName);
			    }
			    
			}
		    Console.WriteLine("Done, press any key to close this window...");
		    Console.ReadKey();
			
		}
	}
}