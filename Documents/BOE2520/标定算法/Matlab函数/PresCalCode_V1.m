function [result] = PresCalCode_V1(P_ref,digT,digC)  %%% This digT need to input the Value after Temprature Calibrated
C_poly = (-4682800+((2^43)./((digC-349526)./2)))/(2^21);
T_poly = (digT-30145)/(2^12);
P_poly = ((100*P_ref)-75000)/(2^16);

M=[];
orderY = [ 3 2 2 1 1 1 0 0 0];
orderT = [ 0 1 0 2 1 0 2 1 0];
for q = 1:numel(orderY)
M=[M (C_poly.^orderY(q)).*(T_poly.^orderT(q))];
end
b = pinv(M)*P_poly*2^18;
b40 = 0;
b31 = 0;
b30 = b(1);
b22 = 0;
b21 = b(2);
b20 = b(3);
b12 = b(4);
b11 = b(5);
b10 = b(6);
b02 = b(7);
b01 = b(8);
b00 = b(9);

result = [b40,b31,b30,b22,b21,b20,b12,b11,b10,b02,b01,b00];

end