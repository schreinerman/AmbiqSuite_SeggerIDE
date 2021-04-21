﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AmbiqSuiteSeggerProjectCreator {
	using System;
	
	
	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	// This class was auto-generated by the StronglyTypedResourceBuilder
	// class via a tool like ResGen or Visual Studio.
	// To add or remove a member, edit your .ResX file then rerun ResGen
	// with the /str option, or rebuild your VS project.
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Templates {
		
		private static global::System.Resources.ResourceManager resourceMan;
		
		private static global::System.Globalization.CultureInfo resourceCulture;
		
		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Templates() {
		}
		
		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AmbiqSuiteSeggerProjectCreator.Templates", typeof(Templates).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}
		
		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to &lt;!DOCTYPE Linker_Placement_File&gt;
		///&lt;Root name=&quot;Flash Section Placement&quot;&gt;
		///  &lt;MemorySegment name=&quot;$(FLASH_NAME:FLASH)&quot;&gt;
		///    &lt;ProgramSection alignment=&quot;0x100&quot; load=&quot;Yes&quot; name=&quot;.vectors&quot; start=&quot;$(FLASH_START:)&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.init&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.init_rodata&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.text&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.dtors&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string flash_placement_xml {
			get {
				return ResourceManager.GetString("flash_placement_xml", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to &lt;!DOCTYPE Linker_Placement_File&gt;
		///&lt;Root name=&quot;RAM Section Placement&quot;&gt;
		///  &lt;MemorySegment name=&quot;$(RAM_NAME:RAM);SRAM&quot;&gt;
		///    &lt;ProgramSection alignment=&quot;0x100&quot; load=&quot;Yes&quot; name=&quot;.vectors&quot; start=&quot;$(RAM_START:$(SRAM_START:))&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.fast&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.init&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.init_rodata&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot; load=&quot;Yes&quot; name=&quot;.text&quot; /&gt;
		///    &lt;ProgramSection alignment=&quot;4&quot;  [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string ram_placement_xml {
			get {
				return ResourceManager.GetString("ram_placement_xml", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to // SEGGER Embedded Studio, runtime support.
		/////
		///// Copyright (c) 2014-2018 SEGGER Microcontroller GmbH &amp; Co KG
		///// Copyright (c) 2001-2018 Rowley Associates Limited.
		/////
		///// This file may be distributed under the terms of the License Agreement
		///// provided with this software.
		/////
		///// THIS FILE IS PROVIDED AS IS WITH NO WARRANTY OF ANY KIND, INCLUDING THE
		///// WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
		///
		///.macro ISR_HANDLER name=
		///  .section .vectors, &quot;ax&quot;
		///  .word \name
		///  .se [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string startup {
			get {
				return ResourceManager.GetString("startup", resourceCulture);
			}
		}
	}
}
