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
using System.Collections.Generic;

namespace AmbiqSuiteSeggerProjectCreator
{
	/// <summary>
	/// Description of GccStartupParser.
	/// </summary>
	public class GccStartupParser
	{
		public List<String> IRQs;
		
		
		public GccStartupParser(String FileName)
		{
			IRQs = new List<string>();
			
			int irq_count = 0;
			
			//
			// Prepare core interrupts
			//
			IRQs.Add("//");
			IRQs.Add("// Core interrupts");
			IRQs.Add("//");	
			//IRQs.Add("ISR_HANDLER Reset_Handler"); //already handled
			IRQs.Add("ISR_HANDLER NMI_Handler");
			IRQs.Add("ISR_HANDLER HardFault_Handler");
			IRQs.Add("ISR_HANDLER MemManage_Handler");
			IRQs.Add("ISR_HANDLER BusFault_Handler");
			IRQs.Add("ISR_HANDLER UsageFault_Handler");
			IRQs.Add("ISR_HANDLER SecureFault_Handler");
			IRQs.Add("ISR_RESERVED");
			IRQs.Add("ISR_RESERVED");
			IRQs.Add("ISR_RESERVED");
			IRQs.Add("ISR_HANDLER SVC_Handler");
			IRQs.Add("ISR_HANDLER DebugMon_Handler");
			IRQs.Add("ISR_RESERVED");
			IRQs.Add("ISR_HANDLER PendSV_Handler");
			IRQs.Add("ISR_HANDLER SysTick_Handler");
			IRQs.Add("//");
			IRQs.Add("// Peripheral interrupts");
			IRQs.Add("//");	
			
			//
			// Start parsing other IRQs
			//
			String[] DataLines = File.ReadAllLines(FileName);
			for(int i = 0; i < DataLines.Length;i++)
			{
				if (DataLines[i].Contains("(void (*)(void))((uint32_t)g_pui32Stack + sizeof(g_pui32Stack)),")) //interrupt vector table start
				{
					i++;
					for(;i < DataLines.Length;i++)
					{
						if (DataLines[i].Contains("};")) //interrupt vector table end
						{
							break;
						}
						
						if (!DataLines[i].Trim().StartsWith("//"))
						{
							irq_count++;
							if (irq_count >= 17) //ignore core irqs and just start with the others
							{
								if (DataLines[i].Trim().StartsWith("0")) //0 means IRQ reserved
								{
									IRQs.Add("ISR_RESERVED");
								} else
								{
									String[] irgline = DataLines[i].Trim().Split(new char[]{','});
									IRQs.Add("ISR_HANDLER " + irgline[0]); 
								}
							}
						}
					}
				}
			}
		}
		
		public void Create(String FileName)
		{
			String StartupContent = global::AmbiqSuiteSeggerProjectCreator.Templates.startup; // loading template
			String IRQList = "";
			
			//
			// Generate IRQ list
			//
			foreach(String IRQ in IRQs)
			{
				IRQList += IRQ + "\r\n";
			}
			
			//
			// Replace placeholder {IRQS} by the IRQ list
			//
			StartupContent = StartupContent.Replace("{IRQS}",IRQList);
			
			//
			// Write file
			//
			File.WriteAllText(FileName,StartupContent);
		}
	}
}
