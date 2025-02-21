'*******************************************************ECAT总线初始化
global CONST BUS_TYPE = 0				 '总线类型。可用于上位机区分当前总线类型
global CONST MAX_AXISNUM = 16			 '最大轴数
global CONST Bus_Slot	= 0				'槽位号0（单总线控制器缺省0）
global CONST PUL_AxisStart	 = 0		'本地脉冲轴起始轴号
global CONST PUL_AxisNum	 = 0		'本地脉冲轴轴数量
global CONST Bus_AxisStart	 = 0	    '总线轴起始轴号
global CONST Bus_NodeNum	 = 1	    '总线配置节点数量,用于判断实际检测到的从站数量是否一致

global Bus_InitStatus			'总线初始化完成状态
Bus_InitStatus = -1
global  Bus_TotalAxisnum		'检查扫描的总轴数

delay(3000)				'延时3S等待驱动器上电，不同驱动器自身上电时间不同，具体根据驱动器调整延时

?"总线通讯周期：",SERVO_PERIOD,"us"
Ecat_Init()			'初始化ECAT总线 

while (Bus_InitStatus = 0)
	Ecat_Init()
wend

end

'***************************************************ECAT总线初始****************************************
'初始流程:  slot_scan（扫描总线） ->   从站节点映射轴/io  ->  SLOT_START（启动总线） -> 初始化成功
'******************************************************************************************************
global sub Ecat_Init()
	local Node_Num,Temp_Axis,Drive_Vender,Drive_Device,Drive_Alias
	RAPIDSTOP(2)
	for i=0 to MAX_AXISNUM - 1								'初始化还原轴类型					
		AXIS_ENABLE(i) = 0
		atype(i)=0	
		AXIS_ADDRESS(i) =0
		DELAY(10)											'防止所有驱动器全部同时切换使能导致瞬间电流过大
	next

	Bus_InitStatus = -1
	Bus_TotalAxisnum = 0	
	SLOT_STOP(Bus_Slot)				
	delay(200)
	slot_scan(Bus_Slot)											'扫描总线
	if return then 
		?"总线扫描成功","连接从站设备数："NODE_COUNT(Bus_Slot)
		if NODE_COUNT(Bus_Slot) <> Bus_NodeNum then		'判断总线检测数量是否为实际接线数量
			?""	
			?"扫描节点数量与程序配置数量不一致!" ,"配置数量:"Bus_NodeNum,"检测数量："NODE_COUNT(Bus_Slot)
			Bus_InitStatus = 0		'初始化失败。报警提示
			return
		endif 	
		
		
		'"开始映射轴号"
		for Node_Num=0 to NODE_COUNT(Bus_Slot)-1						'遍历扫描到的所有从站节点
			Drive_Vender = NODE_INFO(Bus_Slot,Node_Num,0)				'读取驱动器厂商
			Drive_Device = NODE_INFO(Bus_Slot,Node_Num,1)				'读取设备编号
			Drive_Alias = NODE_INFO(Bus_Slot,Node_Num,3)					'读取设备拨码ID
			
			if NODE_AXIS_COUNT(Bus_Slot,Node_Num) <> 0  then				'判断当前节点是否有电机
				for j=0 to NODE_AXIS_COUNT(Bus_Slot,Node_Num)-1			'根据节点带的电机数量循环配置轴参数(针对一拖多驱动器)
					IF Drive_Vender = $83 THEN
						Sub_SetPdo(Node_Num,Drive_Vender,Drive_Device)	'设定PDO参数									
					ELSE					
						Temp_Axis = Bus_AxisStart + Bus_TotalAxisnum		'轴号按NODE顺序分配
						'Temp_Axis = Drive_Alias							'轴号按驱动器设定的拨码分配（一拖多需要特殊处理）					
						base(Temp_Axis)
						AXIS_ADDRESS= Bus_TotalAxisnum+1					'映射轴号
						ATYPE=65											'设置控制模式 65-位置 66-速度 67-转矩 
						DRIVE_PROFILE = 0
						'Sub_SetPdo(Node_Num,Drive_Vender,Drive_Device)					'设定PDO参数
'						Sub_SetDriverIo(Drive_Vender,Temp_Axis,32 + 32*Temp_Axis)		'映射驱动器IO  IO映射到控制器IO32-以后每个驱动器间隔32点			
'						Sub_SetNodePara(Node_Num,Drive_Vender,Drive_Device,j)			'设置特殊总线参数
						disable_group(Temp_Axis)											'每轴单独分组
						Bus_TotalAxisnum=Bus_TotalAxisnum+1									'总轴数+1
					ENDIF
				next
			else														'IO扩展模块
'				Sub_SetNodeIo(Node_Num,Drive_Vender,Drive_Device,1024 + 32*Node_Num)		'映射扩展模块IO	
			endif
		next
		?"轴号映射完成","连接总轴数："Bus_TotalAxisnum
		
		DELAY 200
		SLOT_START(Bus_Slot)				'启动总线
		if return then 
			
			wdog=1							'使能总开关
			
			'?"开始清除驱动器错误"
			for i= Bus_AxisStart to Bus_AxisStart + Bus_TotalAxisnum - 1 
				BASE(i)
				DRIVE_CLEAR(0)
				DELAY 50
	
				'?"驱动器错误清除完成"
				datum(0)						'清除控制器轴状态错误"
				DELAY 100	
				
				'"轴使能"
				AXIS_ENABLE=1
			next
			Bus_InitStatus  = 1
			?"轴使能完成"
			
			'本地脉冲轴配置
			for i = 0 to PUL_AxisNum - 1
				base(PUL_AxisStart + i)
				AXIS_ADDRESS  = (-1<<16) + i
				ATYPE = 4
			next
			?"总线开启成功"			
		else
			?"总线开启失败"
			Bus_InitStatus = 0
		endif	
	else
		?"总线扫描失败"
		Bus_InitStatus = 0
	endif

end sub

'***************************************************从站节点特殊参数配置********************************
'通过SDO方式修改对应对象字典的值修改从站参数(具体对象字典查看驱动器手册)
'******************************************************************************************************
global sub Sub_SetNodePara(iNode,iVender,iDevice,Iaxis)
	if	iVender = $41B and iDevice = $1ab0	 then		'正运动24088脉冲扩展轴
		SDO_WRITE(Bus_Slot,iNode,$6011,Iaxis*$800,5,4)			'设置扩展脉冲轴ATYPE类型
		SDO_WRITE(Bus_Slot,iNode,$6012,Iaxis*$800,6,0)			'设置扩展脉冲轴INVERT_STEP脉冲输出模式
		NODE_IO(Bus_Slot,iNode) = 32 + 32*iNode					'设置240808上IO的起始映射地址				
	elseif iVender = $66f then							'松下驱动器
		SDO_WRITE(Bus_Slot,iNode,$3401,0,4,$10101)				'正限位电平 $818181
		SDO_WRITE(Bus_Slot,iNode,$3402,0,4,$20202)				'负限位电平 $828282
		
		SDO_WRITE(Bus_Slot,iNode,$6091,1,7,1)						'齿轮比
		SDO_WRITE(Bus_Slot,iNode,$6091,2,7,1)	
		SDO_WRITE(Bus_Slot,iNode,$6092,1,7,10000)					'电机一圈脉冲数
		SDO_WRITE(Bus_Slot,iNode,$607E,0,5,224)					'电机方向0  反转224 	
		SDO_WRITE(Bus_Slot,iNode,$6085,0,7,4290000000)			'异常减速度
		'SDO_WRITE(Bus_Slot,iNode,$1010,1,7,$65766173)				'写EPPROM(写EPPROM后驱动器需要重新上电)		
	elseif iVender = $100000 then							'汇川驱动器
		SDO_WRITE(Bus_Slot,iNode,$6091,1,7,1)						'齿轮比
		SDO_WRITE(Bus_Slot,iNode,$6091,2,7,1)	
	endif
end sub

'***************************************************总线驱动IO映射**************************************
'通过DRIVE_IO指令映射驱动器对象字典中60FD,60FE输入输出状态，要设置正确的DRIVE_PROFILEE或者POD后才可以正常映射
'DRIVE_PROFILE模式包含60FD/60FE
'iAxis - 轴号  iVender - 驱动器类型  i_IoNum - 输入输出起始编号
'******************************************************************************************************
global sub Sub_SetDriverIo(iVender,Iaxis,i_IoNum)
	if	iVender = $66f then		'松下驱动器
		DRIVE_PROFILE(iAxis) = 5			'设定对应的带IO映射的PDO模式
		DRIVE_IO(iAxis) = i_IoNum
		
		REV_IN(iAxis) = i_IoNum				'负限位应60FD BIT0
		FWD_IN(iAxis) = i_IoNum + 1			'正限位先对应60FD BIT1
		DATUM_IN(iAxis) = i_IoNum + 2			'原点先对应60FD BIT2
		
		INVERT_IN(i_IoNum,ON)					'特殊信号有效电平反转
		INVERT_IN(i_IoNum + 1,ON)
		INVERT_IN(i_IoNum + 2,ON)
	else
		DRIVE_PROFILE(iAxis) = 12				'不带转矩获取的模式,总线步进驱动器IO可以使用
		DRIVE_IO(iAxis) = i_IoNum
		
		REV_IN(iAxis) = i_IoNum				
		FWD_IN(iAxis) = i_IoNum + 1			
		DATUM_IN(iAxis) = i_IoNum + 2			
		
		INVERT_IN(i_IoNum,ON)					
		INVERT_IN(i_IoNum + 1,ON)
		INVERT_IN(i_IoNum + 2,ON)	
	endif

end sub

'***************************************************总线IO模块映射**************************************
'通过NODE_IO(Bus_Slot,Node_Num)分配模块IO起始地址
'******************************************************************************************************
global sub Sub_SetNodeIo(iNode,iVender,iDevice,i_IoNum)
	if	iVender = $41B and iDevice = $130	 then		'正运动EIO1616
		NODE_IO(Bus_Slot,iNode) = i_IoNum
	endif

end sub

'***************************************************手动配置PDO****************************************
'部分特殊品牌可能需要手动配置，大部分只需要通过DRIVE_PROFILE设置自动配置相应的POD参数即可
'******************************************************************************************************
global sub Sub_SetPdo(iNode,iVender,iDevice)
'	IF  iVender = 0 then									'自定义PDO											
		SDO_WRITE (Bus_Slot, iNode, $1c12, 0 ,5 ,0)					'禁用PDO,禁用后才可以修改内容
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c13, 0 ,5 ,0)
		DELAY(50)

		SDO_WRITE (Bus_Slot, iNode, $1600, $0 ,5 ,0)					'RxPDO 配置对应参数
		SDO_WRITE (Bus_Slot, iNode, $1600, $1 ,7 ,$60400010)			'控制字
		SDO_WRITE (Bus_Slot, iNode, $1600, $2 ,7 ,$607a0020)			'目标位置
		SDO_WRITE (Bus_Slot, iNode, $1600, $3 ,7 ,$60fe0120)			'驱动器IO输入
		SDO_WRITE (Bus_Slot, iNode, $1600, $4 ,7 ,$60980001)			'
		SDO_WRITE (Bus_Slot, iNode, $1600, $0 ,5 ,4)
		
		SDO_WRITE (Bus_Slot, iNode, $1a00, $0 ,5 ,0)					'TxPDO 
		SDO_WRITE (Bus_Slot, iNode, $1a00, $1 ,7 ,$60410010)			'状态字
		SDO_WRITE (Bus_Slot, iNode, $1a00, $2 ,7 ,$60640020)			'反馈位置
		SDO_WRITE (Bus_Slot, iNode, $1a00, $3 ,7 ,$60fd0020)			'驱动器IO输出
		SDO_WRITE (Bus_Slot, iNode, $1a00, $0 ,5 ,3)		

		SDO_WRITE (Bus_Slot, iNode, $1c12, 1 ,6 ,$1600)				'pdo分配对象			
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c13, 1 ,6 ,$1a00)				'
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c12, 0 ,5 ,1)					'启用PDO
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c13, 0 ,5 ,1)

		SDO_WRITE (Bus_Slot, iNode, $1C32, $1 ,6 ,2)					'设置DC同步模式
		SDO_WRITE (Bus_Slot, iNode, $1C33, $1 ,6 ,2)
		
		DRIVE_PROFILE = -1												'使用驱动缺省PDO配置
	?"PDO配置成功"
'	elseif iVender = $66f then
'		DRIVE_PROFILE = 4											'	
'	else
'		DRIVE_PROFILE = 0												'										
'	endif
end sub

'***************************************************总线驱动器回零**************************************
'驱动器回零
'******************************************************************************************************
global sub Sub_SetDriverHome(iAxis,Imode)

	TRIGGER
	local home_sp1,home_sp2,home_mode,home_offset,home_acc
	home_sp1 = 10000
	home_sp2 = 10000
	home_acc = 1000000
	home_mode = Imode
	home_offset  = 0
	
	base(iAxis)
	units = 1000
	AXIS_STOPREASON = 0
	DATUM(21,Imode)
	wait IDLE
	if AXIS_STOPREASON = 0 then
		?"回零成功"
	else
		?"回零失败"	,"停止原因：",AXIS_STOPREASON,"状态字0X",HEX(DRIVE_STATUS)
	endif

end sub


