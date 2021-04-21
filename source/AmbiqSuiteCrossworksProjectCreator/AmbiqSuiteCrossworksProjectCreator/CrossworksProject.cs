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
	/// Description of CrossworksProject.
	/// </summary>
	public class CrossworksProject
	{
		public String ProjectName;
		public String ProjectFolder;
		public GccMakefileParser makeFile;
		public String DevName;
		public int RamStart;
		public int RamSize;
		public int RomStart;
		public int RomSize;
		public String StartupFileName;
		public Boolean IsLibrary;
		public int ApolloVersion=-1;
		
		public CrossworksProject(String ProjectName, String ProjectFolder, GccMakefileParser makeFile, GccLinkerfileParser linkerFile, Boolean IsLibrary)
		{
			this.makeFile = makeFile;
			this.ProjectName = ProjectName;
			this.ProjectFolder = ProjectFolder;
			this.IsLibrary = IsLibrary;
			//
			// Set device name and memory options dependend by the MCU series name specified in the makefile
			//
			if (makeFile.Vars["PART"].Equals("apollo3blue"))
			{
				this.DevName = "AMA3B1KK-KBR";
			    this.RomStart = 0x0000C000;
				this.RomSize = 1024 * 1024 - this.RomStart;
				this.RamStart = 0x10000000;
				this.RamSize = 384 * 1024;
				this.ApolloVersion = 3;
			} else if (makeFile.Vars["PART"].Equals("apollo3"))
			{
				DevName = "AMA3B1KK-KBR";
				this.RomStart = 0x0000C000;
				this.RomSize = 1024 * 1024 - this.RomStart;
				this.RamStart = 0x10000000;
				this.RamSize = 384 * 1024;
				this.ApolloVersion = 3;
			}else if (makeFile.Vars["PART"].Equals("apollo2"))
			{
				DevName = "AMAPH1KK-KBR";
				this.RomStart = 0x00000000;
				this.RomSize = 1024 * 1024 - this.RomStart;
				this.RamStart = 0x10000000;
				this.RamSize = 256 * 1024;
				this.ApolloVersion = 2;
			} else if (makeFile.Vars["PART"].Equals("apollo2blue"))
			{
				DevName = "AMAPH1KK-KBR";
				this.RomStart = 0x00000000;
				this.RomSize = 1024 * 1024 - this.RomStart;
				this.RamStart = 0x10000000;
				this.RamSize = 256 * 1024;
				this.ApolloVersion = 2;
			}else if (makeFile.Vars["PART"].Equals("apollo1"))
			{
				DevName = "APOLLO512-KBR";
				this.RomStart = 0x00000000;
				this.RomSize = 512 * 1024 - this.RomStart;
				this.RamStart = 0x10000000;
				this.RamSize = 64 * 1024;
				this.ApolloVersion = 1;
			}else if (makeFile.Vars["PART"].Equals("apollo"))
			{
				DevName = "APOLLO512-KBR";
				this.RomStart = 0x00000000;
				this.RomSize = 512 * 1024 - this.RomStart;
				this.RamStart = 0x10000000;
				this.RamSize = 64 * 1024;
				this.makeFile.Vars["PART"] = "apollo1";
				this.ApolloVersion = 1;
			} else
			{
				if (linkerFile != null)
				{
					this.RomStart = (int)linkerFile.RomStart;
					this.RomSize = (int)linkerFile.RomSize;
					this.RamStart = (int)linkerFile.RamStart;
					this.RamSize = (int)linkerFile.RamSize;
					if (this.makeFile.Vars.ContainsKey("PART"))
					{
						DevName = this.makeFile.Vars["PART"];
					}
				}
			}
			this.StartupFileName = ReplaceVars("startup_{PART}.s");
			
		}
		
		public String ReplaceVars(String Input)
		{
			Input = Input.Replace("{DevName}",DevName);
			Input = Input.Replace("{RomStart}",RomStart.ToString());
			Input = Input.Replace("{RomStart:X}",RomStart.ToString("X"));
			Input = Input.Replace("{RomSize}",RomSize.ToString());
			Input = Input.Replace("{RomSize:X}",RomSize.ToString("X"));
			Input = Input.Replace("{RamStart}",RamStart.ToString());
			Input = Input.Replace("{RamStart:X}",RamStart.ToString("X"));
			Input = Input.Replace("{RamSize}",RamSize.ToString());
			Input = Input.Replace("{RamSize:X}",RamSize.ToString("X"));
			foreach(KeyValuePair<String,String> Var in makeFile.Vars)
			{
				Input = Input.Replace("{" + Var.Key + "}",Var.Value);
			}
			return Input;
		}
		
		public void Create()
		{
			SortedList<String,String> ProjectDependencies = new SortedList<string, string>();
			String LibsString = "";
			String IncludesString = "";
				
			//
			// Process libraries
			//
			foreach(KeyValuePair<string,FileInfo> kp in makeFile.LibraryFiles)
			{
				String Name = Path.GetFileNameWithoutExtension (kp.Value.Name);
				DirectoryInfo dInfoHal = kp.Value.Directory.Parent.Parent;
				DirectoryInfo dInfoHalCrossworks = new DirectoryInfo(Path.Combine(dInfoHal.FullName,"crossworks"));
				FileInfo dCrossworksProject = new FileInfo(Path.Combine(dInfoHalCrossworks.FullName,Name + ".hzp"));
				
                //
				// If a Crossworks project exists within the library path, 
				// use the project as dependency instead of the library
				//
				// Otherwise use the GCC library
				//
				if (dCrossworksProject.Exists) 
				{
					Uri uriRoot = new Uri(ProjectFolder);
					Uri uriFile = new Uri(dCrossworksProject.FullName);
					String FilePath = uriRoot.MakeRelativeUri(uriFile).ToString().Replace("\\","/");
					ProjectDependencies.Add(Name,FilePath);
				}
				else
				{
					Uri uriRoot = new Uri(ProjectFolder);
					Uri uriFile = new Uri(kp.Value.FullName);
					String FilePath = uriRoot.MakeRelativeUri(uriFile).ToString().Replace("\\","/");
					LibsString += "../" + FilePath + ";";
				}
			}
			
			//
			// Generate include paths string
			//			
			foreach(DirectoryInfo dInfoInclude in makeFile.IncludePaths.Values)
			{
				Uri uriRoot = new Uri(ProjectFolder);
				Uri uriFile = new Uri(dInfoInclude.FullName);
				String FilePath = "../" + uriRoot.MakeRelativeUri(uriFile).ToString().Replace("\\","/");
				IncludesString += FilePath + ";";
			}
			
			StreamWriter sWriter = new StreamWriter(Path.Combine(ProjectFolder,ProjectName + ".hzp"));
			
			sWriter.WriteLine("<!DOCTYPE CrossStudio_Project_File>");
			sWriter.WriteLine("<solution Name=\"" + ProjectName + "\" target=\"8\" version=\"2\">");
			sWriter.WriteLine("  <project Name=\"" + ProjectName + "\">");
			
			#region Common configuration
			sWriter.WriteLine("    <configuration");
			sWriter.WriteLine("      Name=\"Common\"");
			sWriter.WriteLine("      Target=\"" + DevName + "\"");
			sWriter.WriteLine("      arm_architecture=\"v7EM\"");
			sWriter.WriteLine("      arm_core_type=\"Cortex-M4\"");
			sWriter.WriteLine("      arm_endian=\"Little\"");
			sWriter.WriteLine("      arm_fp_abi=\"Hard\"");
			sWriter.WriteLine("      arm_fpu_type=\"FPv4-SP-D16\"");
			sWriter.WriteLine(ReplaceVars("      arm_simulator_memory_simulation_parameter=\"RX {RomStart:X},{RomSize:X},FFFFFFFF;RWX {RamStart:X},{RamSize:X},CDCDCDCD\""));
			sWriter.WriteLine("      arm_target_device_name=\"" + DevName + "\"");
			sWriter.WriteLine("      arm_target_interface_type=\"SWD\"");
			sWriter.WriteLine("      debug_start_from_entry_point_symbol=\"Yes\"");
			sWriter.WriteLine("      debug_target_connection=\"J-Link\"");
			sWriter.WriteLine("      linker_section_placement_file=\"$(ProjectDir)/flash_placement.xml\"");
			sWriter.WriteLine(ReplaceVars("      linker_section_placements_segments=\"FLASH RX 0x{RomStart:X} 0x{RomSize:X};SRAM RWX 0x{RamStart:X} 0x{RamSize:X}\""));
			sWriter.WriteLine("      project_directory=\"\"");
			
			sWriter.WriteLine("      arm_target_debug_interface_type=\"ADIv5\"");  //added for Crossworks
			sWriter.WriteLine("      target_reset_script=\"Reset();\"");  //added for Crossworks
            sWriter.WriteLine("      target_get_partname_script=\"GetPartName()\"");  //added for Crossworks
            sWriter.WriteLine("      target_match_partname_script=\"MatchPartName(&quot;$(Target)&quot;)\"");	  //added for Crossworks
			if (IsLibrary)
			{
			    sWriter.WriteLine("      project_type=\"Library\" />");
			}
			else
			{
			    sWriter.WriteLine("      project_type=\"Executable\" />");
			    sWriter.WriteLine("      <folder Name=\"Internal Files\">");
			    sWriter.WriteLine("        <file file_name=\"$(StudioDir)/source/thumb_crt0.s\" />");
			    sWriter.WriteLine("      </folder>");
			}
			sWriter.WriteLine("      <folder Name=\"Script Files\">");  //added for Crossworks
            sWriter.WriteLine("          <file file_name=\"$(TargetsDir)/Apollo/Scripts/Apollo_Target.js\">");  //added for Crossworks
            sWriter.WriteLine("                <configuration Name=\"Common\" file_type=\"Reset Script\" />");  //added for Crossworks
            sWriter.WriteLine("          </file>");  //added for Crossworks
            sWriter.WriteLine("      </folder>");  //added for Crossworks

			#endregion
			
			#region Internal configuration
			sWriter.WriteLine("    <configuration");

			sWriter.WriteLine("        Name=\"Internal\" />");
			sWriter.WriteLine("        c_user_include_directories=\"$(ProjectDir)/../../../../../CMSIS/ARM/Include;$(ProjectDir)/../../../../../CMSIS/AmbiqMicro/Include\" />");
			#endregion
			
			#region Flash configuration
			sWriter.WriteLine("    <configuration");
			sWriter.WriteLine("      Name=\"" + DevName + "_ROM\"");

			sWriter.WriteLine("      arm_architecture=\"v7EM\"");
			sWriter.WriteLine("      arm_core_type=\"Cortex-M4\"");
			sWriter.WriteLine("      arm_endian=\"Little\"");
			sWriter.WriteLine("      arm_fp_abi=\"Hard\"");
			sWriter.WriteLine("      arm_fpu_type=\"FPv4-SP-D16\"");
			if (RomStart != 0)
			{
				sWriter.WriteLine(ReplaceVars("          JLinkExcludeFlashCacheRange=\"0x0,0x{RomStart:X}\""));
			}
			sWriter.WriteLine(ReplaceVars("      arm_simulator_memory_simulation_parameter=\"RX {RomStart:X},{RomSize:X},FFFFFFFF;RWX {RamStart:X},{RamSize:X},CDCDCDCD\""));
			sWriter.WriteLine("      arm_target_device_name=\"" + DevName + "\"");
			sWriter.WriteLine("      build_intermediate_directory=\"output/release/obj\"");
			if (IsLibrary)
			{
			    sWriter.WriteLine("      build_output_directory=\"bin\"");
			} else
			{
			    sWriter.WriteLine("      build_output_directory=\"output/release/exe\"");
			}
			
			if (ProjectDependencies.Count > 0)
			{
				sWriter.Write("      project_dependencies=\"");
				foreach(KeyValuePair<String,String> kpProjectDependency in ProjectDependencies)
				{
					sWriter.Write(kpProjectDependency.Key +  "(" + kpProjectDependency.Key + ");");
				}
				sWriter.WriteLine("\"");
		    }
			
			sWriter.WriteLine("      arm_linker_heap_size=\"1024\"");
			sWriter.WriteLine("      arm_linker_process_stack_size=\"1024\"");
			sWriter.WriteLine("      arm_linker_stack_size=\"1024\"");
			sWriter.WriteLine(ReplaceVars("      c_preprocessor_definitions=\"{DEFINESSTRING}\""));
			
		    sWriter.WriteLine(ReplaceVars("      c_user_include_directories=\"../../../../../CMSIS/ARM/Include;../../../../../CMSIS/AmbiqMicro/Include;" + IncludesString + "\""));
			
			sWriter.WriteLine("      external_build_file_name=\"$(ProjectDir)/output/release/exe/" + ProjectName + ".elf\"");
			sWriter.WriteLine(ReplaceVars("      external_load_address=\"0x{RomStart:X}\""));
			sWriter.WriteLine(ReplaceVars("linker_additional_files=\"" + LibsString + "\""));

			sWriter.WriteLine("      external_load_file_type=\"Detect\"");
			sWriter.WriteLine(ReplaceVars("      debug_register_definition_file=\"$(ProjectDir)/../../../../../pack/SVD/{PART}.svd\""));
			sWriter.WriteLine(ReplaceVars("      linker_section_placements_segments=\"FLASH RX 0x{RomStart:X} 0x{RomSize:X};SRAM RWX 0x{RamStart:X} 0x{RamSize:X}\" />"));
			#endregion
			
			#region Folder Ambiq_CMSIS
			if (IsLibrary == false)
			{
				sWriter.WriteLine("    <folder Name=\"Ambiq CMSIS\">");
				sWriter.WriteLine(ReplaceVars("        <file file_name=\"startup_{PART}.s\" />"));
				sWriter.WriteLine(ReplaceVars("        <file file_name=\"../../../../../CMSIS/AmbiqMicro/Source/system_{PART}.c\" />"));
				sWriter.WriteLine("    </folder>");
			}
			#endregion
			
			#region Folder Source
			
			SortedList<String,List<String>> FileGroups = new SortedList<string, List<string>>();
			
			//
			// Generate potential file groups as followed
			//
			FileGroups.Add("source",new List<string>());
			FileGroups.Add("utils",new List<string>());
			FileGroups.Add("third_party",new List<string>());
			FileGroups.Add("devices",new List<string>());
			FileGroups.Add("mcu",new List<string>());
			FileGroups.Add("ambiq_ble",new List<string>());
			FileGroups.Add("FreeRTOS9",new List<string>());
			FileGroups.Add("exactle",new List<string>());
			
			//
			// Add readme file
			//
			FileGroups["source"].Add("../README.txt");
            
			//
			// Sort source files into file groups
			//
			foreach(FileInfo fName in makeFile.SourceFiles.Values)
			{
				Uri uriRoot = new Uri(ProjectFolder);
				Uri uriFile = new Uri(fName.FullName);
				String FilePath = uriRoot.MakeRelativeUri(uriFile).ToString().Replace("\\","/");
				Boolean bDone = false;
				foreach(String key in FileGroups.Keys)
				{
					if (FilePath.Contains("/" + key + "/"))
					{
						FileGroups[key].Add(FilePath);
						bDone = true;
						break;
					}
				}
				if (bDone != true)
				{
					FileGroups["source"].Add(FilePath);
				}
				
			}
			
			//
			// Generate file groups + source files entries
			//
			foreach(KeyValuePair<String,List<String>> kp in FileGroups)
			{
				if (kp.Value.Count > 0)
				{
					sWriter.WriteLine("    <folder Name=\"" + kp.Key + "\">");
					foreach(String FilePath in kp.Value)
					{
						sWriter.WriteLine(ReplaceVars("        <file file_name=\"../" + FilePath + "\" />"));
					}
					sWriter.WriteLine("    </folder>");
				}
			}
			#endregion
			
			//
			// close project
			//
			sWriter.WriteLine("  </project>");
			
			
			sWriter.WriteLine("    <configuration");
		    sWriter.WriteLine("      Name=\"" + DevName + "_ROM\"");
		    if (this.ApolloVersion > 0)
			{
				sWriter.WriteLine("    arm_target_flash_loader_file_path=\"$(TargetsDir)/Apollo/Loader/Apollo" + this.ApolloVersion.ToString() + "_Loader.elf\"");
			}
		    sWriter.WriteLine("       inherited_configurations=\"Internal\" />");
			
			
			//
			// Process dependency projects (libraries)
			//
			foreach(KeyValuePair<String,String> kpProjectDependency in ProjectDependencies)
			{
				sWriter.WriteLine("       <import file_name=\"../" + kpProjectDependency.Value + "\" />");
			}
				
			sWriter.WriteLine("</solution>");
			sWriter.Close();
			
			//
			// Write template files
			//
			File.WriteAllText(Path.Combine(ProjectFolder,"flash_placement.xml"),global::AmbiqSuiteCrossworksProjectCreator.Templates.flash_placement_xml);
			File.WriteAllText(Path.Combine(ProjectFolder,"ram_placement.xml"),global::AmbiqSuiteCrossworksProjectCreator.Templates.ram_placement_xml);
		}
	}
}
