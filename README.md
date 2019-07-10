# AmbiqSuite_SeggerIDE
Tool for automatically adding projects for Segger Embedded Studio in AmbiqSuite of Ambiq Micro downloadable at http://ambiqmicro.com. Debugging is usable with J-Link and with evaluation boards with J-Link OB on board. 

See also:
- Ambiq Micro MCU SDK "AmbiqSuite" download: http://ambiqmicro.com
- Segger Embedded Studio download: https://www.segger.com/downloads/embedded-studio/
- Segger J-Link: www.segger.com/j-link (starting with J-Link EDU Mini available for around 20$)

EVB with J-Link OB:
- Apollo1 EVB: https://shop.feeu.com/Shops/es966226/Products/AMAP1EVB (Includes J-Link OB)
- Apollo2 EVB: https://shop.feeu.com/Shops/es966226/Products/AMAPHEVB (Includes J-Link OB)
- Apollo2Blue EVB: https://shop.feeu.com/Shops/es966226/Products/AMA2BEVB (Includes J-Link OB)
- Apollo3Blue EVB: https://shop.feeu.com/Shops/es966226/Products/AMA3BEVB (Includes J-Link OB)

ClickBeetle (BlueBeetle1 with Apollo1 and BlueBeetle5 with Apollo3Blue)
- Entry page: http://www.feeu.com/clickbeetle (requires external J-Link)

## Instructions

![Step 1](/instructions/images/01.png)

Download Segger Embedded Studio from https://www.segger.com/downloads/embedded-studio/

![Step 2](/instructions/images/02.png)

Download AmbiqSuite from the Ambiq Micro website http://ambiqmicro.com/mcu

![Step 3](/instructions/images/03.png)

Extract the donwloaded AmbiqSuite

![Step 4](/instructions/images/04.png)

Download AmbiqSuiteSeggerProjectCreator.exe from this repository https://github.com/schreinerman/AmbiqSuite_SeggerIDE/raw/master/AmbiqSuiteSeggerProjectCreator.exe

![Step 5](/instructions/images/05.png)

Place AmbiqSuiteSeggerProjectCreator.exe into the root folder of the extracted AmbiqSuite

![Step 6](/instructions/images/06.png)

Double click AmbiqSuiteSeggerProjectCreator.exe

![Step 7](/instructions/images/07.png)

Wait program finishes its process

![Step 8](/instructions/images/08.png)

Open an example by double click \<AmbiqSuite\>\boards\<boardname>\examples\<example>\segger\\*.EmProject

## License

Created by Manuel Schreiner, Fujitsu Electronics Europe GmbH
Copyright © 2019 Fujitsu Electronics Europe GmbH. All rights reserved.

1. Redistributions of binary or source code must retain the above copyright notice, this condition and the following disclaimer.

This software is provided by the copyright holder and contributors "AS IS"
and any warranties related to this software are DISCLAIMED.
The copyright owner or contributors be NOT LIABLE for any damages caused
by use of this software.
