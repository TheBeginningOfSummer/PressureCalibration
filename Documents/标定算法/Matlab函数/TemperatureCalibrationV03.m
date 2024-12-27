function [result] = TemperatureCalibrationV03(aquilaTs,T12s)
temp_out=((aquilaTs+273.15)*128);
[X]=Temp2Raw(temp_out, 0, 0);
lambda0=[150 900];
options = optimset('TolX',1e-19,'TolFun',1e-19,'MaxIter',29199,'MaxFunEvals',5930,'Display','Iter');  
[lam,fval]=fminsearch(@(lambda)findvals(lambda, X, T12s),lambda0,options);

Avalue=lam(2);
ALPHAvalue=lam(1);

result = [ALPHAvalue,Avalue];

end