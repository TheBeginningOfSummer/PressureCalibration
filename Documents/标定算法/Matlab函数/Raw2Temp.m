function [T]=Raw2Temp(raw, alpha_trim, A_trim)  
alpha = alpha_trim * 512 + 8388608;
A = A_trim * 8192;
X1 = raw * 32 + alpha;  
X2 = (2^43 ./ X1);
X31=X2*alpha/(2^25);
X3 = X31+((X31 * 23068672) / 2^27);  
X4 = X3 + (X3 * A / 2^30);
T = X4/4-2029;
end