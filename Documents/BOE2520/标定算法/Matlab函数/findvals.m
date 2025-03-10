function [err]=findvals(lam, X, T12s)  
atrim=lam(2);
alphatrim=lam(1);
[T]=Raw2Temp(X, alphatrim, atrim);
AquilaAdjusted=(T-34963.2)/128;
err=sum((AquilaAdjusted-T12s).^2);
end