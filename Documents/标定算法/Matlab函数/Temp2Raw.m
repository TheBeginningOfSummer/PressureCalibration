function [X]=Temp2Raw(temp_out, alpha_trim, A_trim)  
alpha = alpha_trim * 512 + 8388608;
A = A_trim * 8192;
X4 =	((temp_out)+2029)*4;  
X3 = (X4 ./ (1 + A / 2^30));
X31 = (X3 ./ (1 + 23068672 / 2^27));
X2=X31*(2^25)/alpha;  
X1 = (2^43 ./ X2);
X = ((X1 - alpha) / 32);
end