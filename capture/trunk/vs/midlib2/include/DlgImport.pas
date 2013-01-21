unit DlgImport;

interface

uses windows,Messages,midlib2;

//**************************************************************************************       
// Copyright 2009 Aptina Imaging Corporation. All rights reserved.                             
//                                                                                             
//                                                                                             
// No permission to use, copy, modify, or distribute this software and/or                      
// its documentation for any purpose has been granted by Aptina Imaging Corporation.           
// If any such permission has been granted ( by separate agreement ), it                       
// is required that the above copyright notice appear in all copies and                        
// that both that copyright notice and this permission notice appear in                        
// supporting documentation, and that the name of Aptina Imaging Corporation or any            
// of its trademarks may not be used in advertising or publicity pertaining                    
// to distribution of the software without specific, written prior permission.                 
//                                                                                             
//                                                                                             
//      This software and any associated documentation are provided “AS IS” and                
//      without warranty of any kind.   APTINA IMAGING CORPORATION EXPRESSLY DISCLAIMS         
//      ALL WARRANTIES EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO, NONINFRINGEMENT       
//      OF THIRD PARTY RIGHTS, AND ANY IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS        
//      FOR A PARTICULAR PURPOSE.  APTINA DOES NOT WARRANT THAT THE FUNCTIONS CONTAINED        
//      IN THIS SOFTWARE WILL MEET YOUR REQUIREMENTS, OR THAT THE OPERATION OF THIS SOFTWARE   
//      WILL BE UNINTERRUPTED OR ERROR-FREE.  FURTHERMORE, APTINA DOES NOT WARRANT OR          
//      MAKE ANY REPRESENTATIONS REGARDING THE USE OR THE RESULTS OF THE USE OF ANY            
//      ACCOMPANYING DOCUMENTATION IN TERMS OF ITS CORRECTNESS, ACCURACY, RELIABILITY,         
//      OR OTHERWISE.                                                                          
//*************************************************************************************/       

const
  PLUGIN_DLG_API=5;

  WM_PLUGIN_HIDE=(WM_APP+$211);
  WM_MONITOR=(WM_APP+$111);

type
  DEVW_Camera_t = record
    sensorName     : pchar;
    sensorType     : mi_sensor_types;
    fullWidth      : mi_u32;
    fullHeight     : mi_u32;
    sensorVersion  : mi_u32;
    partNumber     : pchar;
    productID      : mi_product_ids;
    productVersion : mi_u32;
    productName    : pchar;
    firmwareVersion: mi_u32;
    pCamera        : Pmi_camera_t
  end;

type
  PDEVW_Camera_t = ^DEVW_Camera_t;

type
  devw_ReadRegister=function(regname:pchar ;fieldname:pchar; cached:mi_u32): mi_u32; stdcall;
  devw_readRegisterAddr=function( nAddr : mi_u32; nBitmask: mi_u32; addrSpace: mi_u32; addrType:mi_addr_type; cached:mi_u32): mi_u32; stdcall;
  devw_writeRegister=procedure(regname:pchar ;fieldname:pchar; value:mi_u32); stdcall;
  devw_writeRegisterAddr=procedure( nAddr : mi_u32; nBitmask: mi_u32; naddrSpace: mi_u32; addrType:mi_addr_type; value:mi_u32); stdcall;
  devw_PeekRegister=function ( nBaseAddr : mi_u32; nAddr : mi_u32; addrSize:integer; dataSize:integer): mi_u32; stdcall;
  devw_PokeRegister=procedure( nBaseAddr : mi_u32; nAddr : mi_u32; value:mi_u32; addrSize:integer; dataSize:integer); stdcall;
  devw_SyncChanges=procedure(sync:mi_u32); stdcall;
  devw_Stop=procedure(stop:mi_u32); stdcall;
  devw_MonitorRegister=procedure(regname:pchar ;id:mi_u32;handle:HWND); stdcall;
  devw_GetState=function(state:pchar): integer; stdcall;
  devw_SetState=procedure(state:pchar;nVal:integer);stdcall;
  devw_PokeRegisters=procedure(nBaseAddr:mi_u32; nAddr:mi_u32; nCount:integer; pValues:Pointer; addrSize:integer; dataSize:integer); stdcall;
  devw_Pause=procedure(pause:mi_u32); stdcall;
  devw_PeekRegisters=procedure(nBaseAddr:mi_u32; nAddr:mi_u32; nCount:integer; pValues:Pointer; addrSize:integer; dataSize:integer); stdcall;
  devw_GetStateStr=function(state:pchar):pchar; stdcall;
  devw_SetStateStr=procedure(state:pchar; szVal:pchar); stdcall;

type
  tag_devw_callbacks_t = record
      readRegister      : devw_ReadRegister;
      readRegisterAddr  : devw_readRegisterAddr;
      writeRegister     : devw_writeRegister;
      writeRegisterAddr : devw_writeRegisterAddr;
      PeekRegister      : devw_PeekRegister;
      PokeRegister      : devw_PokeRegister;
      SyncChanges       : devw_SyncChanges;
      MonitorRegister   : devw_MonitorRegister;
      Stop		: devw_Stop;
      GetState		: devw_GetState;
      SetState		: devw_SetState;
      PokeRegisters	: devw_PokeRegisters;
      Pause		: devw_Pause;
      PeekRegisters     : devw_PeekRegisters;
      GetStateStr       : devw_GetStateStr;
      SetStateStr       : devw_SetStateStr;
  end;

type
  Ptag_devw_callbacks_t = ^tag_devw_callbacks_t;


implementation

end.
