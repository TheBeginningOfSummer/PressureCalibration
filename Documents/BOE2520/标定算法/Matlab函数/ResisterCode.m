% alpha_OTP A_OTP b40 b31 b30 b22 b21 b20 b12 b11 b10 b02 b01 b00
% 1 2 3 4 5 6 7 8 9 10 11 12 13 14
%从上级函数获取ab与nbs值

function [newbytes] = ResisterCode(ab,nbs)
bbs={};

for q=1:length(ab) % for each of the 14 parameters  
bij=ab(q);
bbs{q}=dec2bin(bij,nbs(q)); % convert to binary, taking the number of reserved bits per parameter  
end

% generate bytes: for 28 addresses (starting 34 and ending 4f)
newbytes='22222222'; % initialize this variable as a string of 8 characters and do not use 0 or 1 in that string. 

newbytes(1,:)=[bbs{7}(end-7:end)]; % 0x34, to be added, b21  
newbytes(2,:)=[bbs{13}(end-7:end) ]; % 0x35, b01
newbytes(3,:)=[bbs{12}(end-7:end) ]; % 0x36, b02
newbytes(4,:)=[bbs{11}(end-7:end) ]; % 0x37, b10
newbytes(5,:)=[bbs{3}(end-7:end) ]; % 0x38, b40

newbytes(6,:)=[bbs{10}(end-7:end) ]; % 0x39, b11
newbytes(7,:)=[bbs{9}(end-7:end) ]; % 0x3A, b12
newbytes(8,:)=[bbs{5}(end-7:end) ]; % 0x3B, b30

newbytes(9,:)=[bbs{14}(end-7:end) ]; % 0x3C, b00
newbytes(10,:)=[bbs{8}(end-7:end) ]; % 0x3D, b20
newbytes(11,:)=[bbs{2}(end-7:end)]; % 0x3E, A
newbytes(12,:)=[bbs{1}(end-7:end) ]; % 0x3F, alpha

newbytes(13,:)=['00' bbs{2}(end-13:end-8) ]; % 0x40, A
newbytes(14,:)=['0' bbs{6}(end-6:end)]; % 0x41, b22
newbytes(15,:)=[bbs{14}(end-15:end-8) ]; % 0x42, b00  
newbytes(16,:)=[bbs{9}(end-8) bbs{7}(end-10:end-8) bbs{14}(end-19:end-16)]; % 0x43, b12, b21, b00

newbytes(17,:)=[bbs{8}(end-15:end-8) ]; % 0x44, b20  
newbytes(18,:)=[bbs{1}(end-12:end-8) '0' bbs{8}(end-17:end-16) ]; % 0x45,  alpha, b20
newbytes(19,:)=[bbs{3}(end-8) bbs{5}(end-14:end-8)]; % 0x46, b40, b30
newbytes(20,:)=[bbs{4}(end-7:end-0) ]; % 0x47, b31

newbytes(21,:)=[bbs{4}(end-9:end-8) bbs{13}(end-13:end-8) ]; % 0x48, b31,  b01
newbytes(22,:)=['00000000']; % 0x49, NOTHING to FUSE: cdc_offset
newbytes(23,:)=[bbs{12}(end-10:end-8) bbs{10}(end-12:end-8)]; % 0x4A, b02  and b11
newbytes(24,:)=['00000000']; % 0x4B, osc_slow: do not overwrite

newbytes(25,:)=['00000000' ]; % 0x4C=cdc_ref + osc_fast  
newbytes(26,:)=['00000000' ]; % 0x4D=ibias + cdc_ref  
newbytes(27,:)=[bbs{11}(end-11:end-8) '0000']; % 0x4E
newbytes(28,:)=['0' bbs{11}(end-18:end-12) ]; % 0x4F, b10

end
