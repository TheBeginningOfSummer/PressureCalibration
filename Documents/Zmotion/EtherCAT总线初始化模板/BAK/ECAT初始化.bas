'*******************************************************ECAT���߳�ʼ��
global CONST BUS_TYPE = 0				 '�������͡���������λ�����ֵ�ǰ��������
global CONST MAX_AXISNUM = 16			 '�������
global CONST Bus_Slot	= 0				'��λ��0�������߿�����ȱʡ0��
global CONST PUL_AxisStart	 = 0		'������������ʼ���
global CONST PUL_AxisNum	 = 0		'����������������
global CONST Bus_AxisStart	 = 0	    '��������ʼ���
global CONST Bus_NodeNum	 = 1	    '�������ýڵ�����,�����ж�ʵ�ʼ�⵽�Ĵ�վ�����Ƿ�һ��

global Bus_InitStatus			'���߳�ʼ�����״̬
Bus_InitStatus = -1
global  Bus_TotalAxisnum		'���ɨ���������

delay(3000)				'��ʱ3S�ȴ��������ϵ磬��ͬ�����������ϵ�ʱ�䲻ͬ���������������������ʱ

?"����ͨѶ���ڣ�",SERVO_PERIOD,"us"
Ecat_Init()			'��ʼ��ECAT���� 

while (Bus_InitStatus = 0)
	Ecat_Init()
wend

end

'***************************************************ECAT���߳�ʼ****************************************
'��ʼ����:  slot_scan��ɨ�����ߣ� ->   ��վ�ڵ�ӳ����/io  ->  SLOT_START���������ߣ� -> ��ʼ���ɹ�
'******************************************************************************************************
global sub Ecat_Init()
	local Node_Num,Temp_Axis,Drive_Vender,Drive_Device,Drive_Alias
	RAPIDSTOP(2)
	for i=0 to MAX_AXISNUM - 1								'��ʼ����ԭ������					
		AXIS_ENABLE(i) = 0
		atype(i)=0	
		AXIS_ADDRESS(i) =0
		DELAY(10)											'��ֹ����������ȫ��ͬʱ�л�ʹ�ܵ���˲���������
	next

	Bus_InitStatus = -1
	Bus_TotalAxisnum = 0	
	SLOT_STOP(Bus_Slot)				
	delay(200)
	slot_scan(Bus_Slot)											'ɨ������
	if return then 
		?"����ɨ��ɹ�","���Ӵ�վ�豸����"NODE_COUNT(Bus_Slot)
		if NODE_COUNT(Bus_Slot) <> Bus_NodeNum then		'�ж����߼�������Ƿ�Ϊʵ�ʽ�������
			?""	
			?"ɨ��ڵ��������������������һ��!" ,"��������:"Bus_NodeNum,"���������"NODE_COUNT(Bus_Slot)
			Bus_InitStatus = 0		'��ʼ��ʧ�ܡ�������ʾ
			return
		endif 	
		
		
		'"��ʼӳ�����"
		for Node_Num=0 to NODE_COUNT(Bus_Slot)-1						'����ɨ�赽�����д�վ�ڵ�
			Drive_Vender = NODE_INFO(Bus_Slot,Node_Num,0)				'��ȡ����������
			Drive_Device = NODE_INFO(Bus_Slot,Node_Num,1)				'��ȡ�豸���
			Drive_Alias = NODE_INFO(Bus_Slot,Node_Num,3)					'��ȡ�豸����ID
			
			if NODE_AXIS_COUNT(Bus_Slot,Node_Num) <> 0  then				'�жϵ�ǰ�ڵ��Ƿ��е��
				for j=0 to NODE_AXIS_COUNT(Bus_Slot,Node_Num)-1			'���ݽڵ���ĵ������ѭ�����������(���һ�϶�������)
					IF Drive_Vender = $83 THEN
						Sub_SetPdo(Node_Num,Drive_Vender,Drive_Device)	'�趨PDO����									
					ELSE					
						Temp_Axis = Bus_AxisStart + Bus_TotalAxisnum		'��Ű�NODE˳�����
						'Temp_Axis = Drive_Alias							'��Ű��������趨�Ĳ�����䣨һ�϶���Ҫ���⴦��					
						base(Temp_Axis)
						AXIS_ADDRESS= Bus_TotalAxisnum+1					'ӳ�����
						ATYPE=65											'���ÿ���ģʽ 65-λ�� 66-�ٶ� 67-ת�� 
						DRIVE_PROFILE = 0
						'Sub_SetPdo(Node_Num,Drive_Vender,Drive_Device)					'�趨PDO����
'						Sub_SetDriverIo(Drive_Vender,Temp_Axis,32 + 32*Temp_Axis)		'ӳ��������IO  IOӳ�䵽������IO32-�Ժ�ÿ�����������32��			
'						Sub_SetNodePara(Node_Num,Drive_Vender,Drive_Device,j)			'�����������߲���
						disable_group(Temp_Axis)											'ÿ�ᵥ������
						Bus_TotalAxisnum=Bus_TotalAxisnum+1									'������+1
					ENDIF
				next
			else														'IO��չģ��
'				Sub_SetNodeIo(Node_Num,Drive_Vender,Drive_Device,1024 + 32*Node_Num)		'ӳ����չģ��IO	
			endif
		next
		?"���ӳ�����","������������"Bus_TotalAxisnum
		
		DELAY 200
		SLOT_START(Bus_Slot)				'��������
		if return then 
			
			wdog=1							'ʹ���ܿ���
			
			'?"��ʼ�������������"
			for i= Bus_AxisStart to Bus_AxisStart + Bus_TotalAxisnum - 1 
				BASE(i)
				DRIVE_CLEAR(0)
				DELAY 50
	
				'?"����������������"
				datum(0)						'�����������״̬����"
				DELAY 100	
				
				'"��ʹ��"
				AXIS_ENABLE=1
			next
			Bus_InitStatus  = 1
			?"��ʹ�����"
			
			'��������������
			for i = 0 to PUL_AxisNum - 1
				base(PUL_AxisStart + i)
				AXIS_ADDRESS  = (-1<<16) + i
				ATYPE = 4
			next
			?"���߿����ɹ�"			
		else
			?"���߿���ʧ��"
			Bus_InitStatus = 0
		endif	
	else
		?"����ɨ��ʧ��"
		Bus_InitStatus = 0
	endif

end sub

'***************************************************��վ�ڵ������������********************************
'ͨ��SDO��ʽ�޸Ķ�Ӧ�����ֵ��ֵ�޸Ĵ�վ����(��������ֵ�鿴�������ֲ�)
'******************************************************************************************************
global sub Sub_SetNodePara(iNode,iVender,iDevice,Iaxis)
	if	iVender = $41B and iDevice = $1ab0	 then		'���˶�24088������չ��
		SDO_WRITE(Bus_Slot,iNode,$6011,Iaxis*$800,5,4)			'������չ������ATYPE����
		SDO_WRITE(Bus_Slot,iNode,$6012,Iaxis*$800,6,0)			'������չ������INVERT_STEP�������ģʽ
		NODE_IO(Bus_Slot,iNode) = 32 + 32*iNode					'����240808��IO����ʼӳ���ַ				
	elseif iVender = $66f then							'����������
		SDO_WRITE(Bus_Slot,iNode,$3401,0,4,$10101)				'����λ��ƽ $818181
		SDO_WRITE(Bus_Slot,iNode,$3402,0,4,$20202)				'����λ��ƽ $828282
		
		SDO_WRITE(Bus_Slot,iNode,$6091,1,7,1)						'���ֱ�
		SDO_WRITE(Bus_Slot,iNode,$6091,2,7,1)	
		SDO_WRITE(Bus_Slot,iNode,$6092,1,7,10000)					'���һȦ������
		SDO_WRITE(Bus_Slot,iNode,$607E,0,5,224)					'�������0  ��ת224 	
		SDO_WRITE(Bus_Slot,iNode,$6085,0,7,4290000000)			'�쳣���ٶ�
		'SDO_WRITE(Bus_Slot,iNode,$1010,1,7,$65766173)				'дEPPROM(дEPPROM����������Ҫ�����ϵ�)		
	elseif iVender = $100000 then							'�㴨������
		SDO_WRITE(Bus_Slot,iNode,$6091,1,7,1)						'���ֱ�
		SDO_WRITE(Bus_Slot,iNode,$6091,2,7,1)	
	endif
end sub

'***************************************************��������IOӳ��**************************************
'ͨ��DRIVE_IOָ��ӳ�������������ֵ���60FD,60FE�������״̬��Ҫ������ȷ��DRIVE_PROFILEE����POD��ſ�������ӳ��
'DRIVE_PROFILEģʽ����60FD/60FE
'iAxis - ���  iVender - ����������  i_IoNum - ���������ʼ���
'******************************************************************************************************
global sub Sub_SetDriverIo(iVender,Iaxis,i_IoNum)
	if	iVender = $66f then		'����������
		DRIVE_PROFILE(iAxis) = 5			'�趨��Ӧ�Ĵ�IOӳ���PDOģʽ
		DRIVE_IO(iAxis) = i_IoNum
		
		REV_IN(iAxis) = i_IoNum				'����λӦ60FD BIT0
		FWD_IN(iAxis) = i_IoNum + 1			'����λ�ȶ�Ӧ60FD BIT1
		DATUM_IN(iAxis) = i_IoNum + 2			'ԭ���ȶ�Ӧ60FD BIT2
		
		INVERT_IN(i_IoNum,ON)					'�����ź���Ч��ƽ��ת
		INVERT_IN(i_IoNum + 1,ON)
		INVERT_IN(i_IoNum + 2,ON)
	else
		DRIVE_PROFILE(iAxis) = 12				'����ת�ػ�ȡ��ģʽ,���߲���������IO����ʹ��
		DRIVE_IO(iAxis) = i_IoNum
		
		REV_IN(iAxis) = i_IoNum				
		FWD_IN(iAxis) = i_IoNum + 1			
		DATUM_IN(iAxis) = i_IoNum + 2			
		
		INVERT_IN(i_IoNum,ON)					
		INVERT_IN(i_IoNum + 1,ON)
		INVERT_IN(i_IoNum + 2,ON)	
	endif

end sub

'***************************************************����IOģ��ӳ��**************************************
'ͨ��NODE_IO(Bus_Slot,Node_Num)����ģ��IO��ʼ��ַ
'******************************************************************************************************
global sub Sub_SetNodeIo(iNode,iVender,iDevice,i_IoNum)
	if	iVender = $41B and iDevice = $130	 then		'���˶�EIO1616
		NODE_IO(Bus_Slot,iNode) = i_IoNum
	endif

end sub

'***************************************************�ֶ�����PDO****************************************
'��������Ʒ�ƿ�����Ҫ�ֶ����ã��󲿷�ֻ��Ҫͨ��DRIVE_PROFILE�����Զ�������Ӧ��POD��������
'******************************************************************************************************
global sub Sub_SetPdo(iNode,iVender,iDevice)
'	IF  iVender = 0 then									'�Զ���PDO											
		SDO_WRITE (Bus_Slot, iNode, $1c12, 0 ,5 ,0)					'����PDO,���ú�ſ����޸�����
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c13, 0 ,5 ,0)
		DELAY(50)

		SDO_WRITE (Bus_Slot, iNode, $1600, $0 ,5 ,0)					'RxPDO ���ö�Ӧ����
		SDO_WRITE (Bus_Slot, iNode, $1600, $1 ,7 ,$60400010)			'������
		SDO_WRITE (Bus_Slot, iNode, $1600, $2 ,7 ,$607a0020)			'Ŀ��λ��
		SDO_WRITE (Bus_Slot, iNode, $1600, $3 ,7 ,$60fe0120)			'������IO����
		SDO_WRITE (Bus_Slot, iNode, $1600, $4 ,7 ,$60980001)			'
		SDO_WRITE (Bus_Slot, iNode, $1600, $0 ,5 ,4)
		
		SDO_WRITE (Bus_Slot, iNode, $1a00, $0 ,5 ,0)					'TxPDO 
		SDO_WRITE (Bus_Slot, iNode, $1a00, $1 ,7 ,$60410010)			'״̬��
		SDO_WRITE (Bus_Slot, iNode, $1a00, $2 ,7 ,$60640020)			'����λ��
		SDO_WRITE (Bus_Slot, iNode, $1a00, $3 ,7 ,$60fd0020)			'������IO���
		SDO_WRITE (Bus_Slot, iNode, $1a00, $0 ,5 ,3)		

		SDO_WRITE (Bus_Slot, iNode, $1c12, 1 ,6 ,$1600)				'pdo�������			
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c13, 1 ,6 ,$1a00)				'
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c12, 0 ,5 ,1)					'����PDO
		DELAY(50)
		SDO_WRITE (Bus_Slot, iNode, $1c13, 0 ,5 ,1)

		SDO_WRITE (Bus_Slot, iNode, $1C32, $1 ,6 ,2)					'����DCͬ��ģʽ
		SDO_WRITE (Bus_Slot, iNode, $1C33, $1 ,6 ,2)
		
		DRIVE_PROFILE = -1												'ʹ������ȱʡPDO����
	?"PDO���óɹ�"
'	elseif iVender = $66f then
'		DRIVE_PROFILE = 4											'	
'	else
'		DRIVE_PROFILE = 0												'										
'	endif
end sub

'***************************************************��������������**************************************
'����������
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
		?"����ɹ�"
	else
		?"����ʧ��"	,"ֹͣԭ��",AXIS_STOPREASON,"״̬��0X",HEX(DRIVE_STATUS)
	endif

end sub


