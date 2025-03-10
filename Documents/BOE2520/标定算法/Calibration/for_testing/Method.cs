/*
* MATLAB Compiler: 23.2 (R2023b)
* Date: Tue Jul  2 19:39:12 2024
* Arguments:
* "-B""macro_default""-W""dotnet:Calibration,Method,4.0,private,version=2.0""-T""link:lib"
* "-d""C:\Users\Summer\Desktop\Cal\Calibration\for_testing""-v""class{Method:C:\Users\Summ
* er\Desktop\Cal\findvals.m,C:\Users\Summer\Desktop\Cal\PresCalCode_V0.m,C:\Users\Summer\D
* esktop\Cal\PresCalCode_V1.m,C:\Users\Summer\Desktop\Cal\Raw2Temp.m,C:\Users\Summer\Deskt
* op\Cal\ResisterCode.m,C:\Users\Summer\Desktop\Cal\Temp2Raw.m,C:\Users\Summer\Desktop\Cal
* \TemperatureCalibrationV02.m,C:\Users\Summer\Desktop\Cal\TemperatureCalibrationV03.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace Calibration
{

  /// <summary>
  /// The Method class provides a CLS compliant, MWArray interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\findvals.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\PresCalCode_V0.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\PresCalCode_V1.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\Raw2Temp.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\ResisterCode.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\Temp2Raw.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\TemperatureCalibrationV02.m
  /// <newpara></newpara>
  /// C:\Users\Summer\Desktop\Cal\TemperatureCalibrationV03.m
  /// </summary>
  /// <remarks>
  /// @Version 2.0
  /// </remarks>
  public class Method : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static Method()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

		  int lastDelimiter = ctfFilePath.LastIndexOf(@"/");

	      if (lastDelimiter == -1)
		  {
		    lastDelimiter = ctfFilePath.LastIndexOf(@"\");
		  }

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "Calibration.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the Method class.
    /// </summary>
    public Method()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~Method()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the findvals MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray findvals()
    {
      return mcr.EvaluateFunction("findvals", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the findvals MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="lam">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray findvals(MWArray lam)
    {
      return mcr.EvaluateFunction("findvals", lam);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the findvals MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="lam">Input argument #1</param>
    /// <param name="X">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray findvals(MWArray lam, MWArray X)
    {
      return mcr.EvaluateFunction("findvals", lam, X);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the findvals MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="lam">Input argument #1</param>
    /// <param name="X">Input argument #2</param>
    /// <param name="T12s">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray findvals(MWArray lam, MWArray X, MWArray T12s)
    {
      return mcr.EvaluateFunction("findvals", lam, X, T12s);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the findvals MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] findvals(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "findvals", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the findvals MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lam">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] findvals(int numArgsOut, MWArray lam)
    {
      return mcr.EvaluateFunction(numArgsOut, "findvals", lam);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the findvals MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lam">Input argument #1</param>
    /// <param name="X">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] findvals(int numArgsOut, MWArray lam, MWArray X)
    {
      return mcr.EvaluateFunction(numArgsOut, "findvals", lam, X);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the findvals MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lam">Input argument #1</param>
    /// <param name="X">Input argument #2</param>
    /// <param name="T12s">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] findvals(int numArgsOut, MWArray lam, MWArray X, MWArray T12s)
    {
      return mcr.EvaluateFunction(numArgsOut, "findvals", lam, X, T12s);
    }


    /// <summary>
    /// Provides an interface for the findvals function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void findvals(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("findvals", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V0()
    {
      return mcr.EvaluateFunction("PresCalCode_V0", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="P_ref">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V0(MWArray P_ref)
    {
      return mcr.EvaluateFunction("PresCalCode_V0", P_ref);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V0(MWArray P_ref, MWArray digT)
    {
      return mcr.EvaluateFunction("PresCalCode_V0", P_ref, digT);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <param name="digC">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V0(MWArray P_ref, MWArray digT, MWArray digC)
    {
      return mcr.EvaluateFunction("PresCalCode_V0", P_ref, digT, digC);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V0(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V0", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="P_ref">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V0(int numArgsOut, MWArray P_ref)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V0", P_ref);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V0(int numArgsOut, MWArray P_ref, MWArray digT)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V0", P_ref, digT);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the PresCalCode_V0 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <param name="digC">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V0(int numArgsOut, MWArray P_ref, MWArray digT, MWArray 
                              digC)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V0", P_ref, digT, digC);
    }


    /// <summary>
    /// Provides an interface for the PresCalCode_V0 function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void PresCalCode_V0(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("PresCalCode_V0", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V1()
    {
      return mcr.EvaluateFunction("PresCalCode_V1", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="P_ref">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V1(MWArray P_ref)
    {
      return mcr.EvaluateFunction("PresCalCode_V1", P_ref);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V1(MWArray P_ref, MWArray digT)
    {
      return mcr.EvaluateFunction("PresCalCode_V1", P_ref, digT);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <param name="digC">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray PresCalCode_V1(MWArray P_ref, MWArray digT, MWArray digC)
    {
      return mcr.EvaluateFunction("PresCalCode_V1", P_ref, digT, digC);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V1(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V1", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="P_ref">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V1(int numArgsOut, MWArray P_ref)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V1", P_ref);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V1(int numArgsOut, MWArray P_ref, MWArray digT)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V1", P_ref, digT);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the PresCalCode_V1 MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="P_ref">Input argument #1</param>
    /// <param name="digT">Input argument #2</param>
    /// <param name="digC">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] PresCalCode_V1(int numArgsOut, MWArray P_ref, MWArray digT, MWArray 
                              digC)
    {
      return mcr.EvaluateFunction(numArgsOut, "PresCalCode_V1", P_ref, digT, digC);
    }


    /// <summary>
    /// Provides an interface for the PresCalCode_V1 function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void PresCalCode_V1(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("PresCalCode_V1", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the Raw2Temp MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Raw2Temp()
    {
      return mcr.EvaluateFunction("Raw2Temp", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the Raw2Temp MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="raw">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Raw2Temp(MWArray raw)
    {
      return mcr.EvaluateFunction("Raw2Temp", raw);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the Raw2Temp MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="raw">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Raw2Temp(MWArray raw, MWArray alpha_trim)
    {
      return mcr.EvaluateFunction("Raw2Temp", raw, alpha_trim);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the Raw2Temp MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="raw">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <param name="A_trim">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Raw2Temp(MWArray raw, MWArray alpha_trim, MWArray A_trim)
    {
      return mcr.EvaluateFunction("Raw2Temp", raw, alpha_trim, A_trim);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the Raw2Temp MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Raw2Temp(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "Raw2Temp", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the Raw2Temp MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="raw">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Raw2Temp(int numArgsOut, MWArray raw)
    {
      return mcr.EvaluateFunction(numArgsOut, "Raw2Temp", raw);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the Raw2Temp MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="raw">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Raw2Temp(int numArgsOut, MWArray raw, MWArray alpha_trim)
    {
      return mcr.EvaluateFunction(numArgsOut, "Raw2Temp", raw, alpha_trim);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the Raw2Temp MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="raw">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <param name="A_trim">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Raw2Temp(int numArgsOut, MWArray raw, MWArray alpha_trim, MWArray 
                        A_trim)
    {
      return mcr.EvaluateFunction(numArgsOut, "Raw2Temp", raw, alpha_trim, A_trim);
    }


    /// <summary>
    /// Provides an interface for the Raw2Temp function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void Raw2Temp(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("Raw2Temp", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the ResisterCode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ResisterCode()
    {
      return mcr.EvaluateFunction("ResisterCode", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the ResisterCode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <param name="ab">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ResisterCode(MWArray ab)
    {
      return mcr.EvaluateFunction("ResisterCode", ab);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the ResisterCode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <param name="ab">Input argument #1</param>
    /// <param name="nbs">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ResisterCode(MWArray ab, MWArray nbs)
    {
      return mcr.EvaluateFunction("ResisterCode", ab, nbs);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the ResisterCode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ResisterCode(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "ResisterCode", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the ResisterCode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ab">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ResisterCode(int numArgsOut, MWArray ab)
    {
      return mcr.EvaluateFunction(numArgsOut, "ResisterCode", ab);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the ResisterCode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ab">Input argument #1</param>
    /// <param name="nbs">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ResisterCode(int numArgsOut, MWArray ab, MWArray nbs)
    {
      return mcr.EvaluateFunction(numArgsOut, "ResisterCode", ab, nbs);
    }


    /// <summary>
    /// Provides an interface for the ResisterCode function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// generate bytes: for 28 addresses (starting 34 and ending 4f)
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void ResisterCode(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("ResisterCode", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the Temp2Raw MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Temp2Raw()
    {
      return mcr.EvaluateFunction("Temp2Raw", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the Temp2Raw MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="temp_out">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Temp2Raw(MWArray temp_out)
    {
      return mcr.EvaluateFunction("Temp2Raw", temp_out);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the Temp2Raw MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="temp_out">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Temp2Raw(MWArray temp_out, MWArray alpha_trim)
    {
      return mcr.EvaluateFunction("Temp2Raw", temp_out, alpha_trim);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the Temp2Raw MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="temp_out">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <param name="A_trim">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray Temp2Raw(MWArray temp_out, MWArray alpha_trim, MWArray A_trim)
    {
      return mcr.EvaluateFunction("Temp2Raw", temp_out, alpha_trim, A_trim);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the Temp2Raw MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Temp2Raw(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "Temp2Raw", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the Temp2Raw MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="temp_out">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Temp2Raw(int numArgsOut, MWArray temp_out)
    {
      return mcr.EvaluateFunction(numArgsOut, "Temp2Raw", temp_out);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the Temp2Raw MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="temp_out">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Temp2Raw(int numArgsOut, MWArray temp_out, MWArray alpha_trim)
    {
      return mcr.EvaluateFunction(numArgsOut, "Temp2Raw", temp_out, alpha_trim);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the Temp2Raw MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="temp_out">Input argument #1</param>
    /// <param name="alpha_trim">Input argument #2</param>
    /// <param name="A_trim">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] Temp2Raw(int numArgsOut, MWArray temp_out, MWArray alpha_trim, 
                        MWArray A_trim)
    {
      return mcr.EvaluateFunction(numArgsOut, "Temp2Raw", temp_out, alpha_trim, A_trim);
    }


    /// <summary>
    /// Provides an interface for the Temp2Raw function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void Temp2Raw(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("Temp2Raw", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the
    /// TemperatureCalibrationV02 MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray TemperatureCalibrationV02()
    {
      return mcr.EvaluateFunction("TemperatureCalibrationV02", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the
    /// TemperatureCalibrationV02 MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ensTs">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray TemperatureCalibrationV02(MWArray ensTs)
    {
      return mcr.EvaluateFunction("TemperatureCalibrationV02", ensTs);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the
    /// TemperatureCalibrationV02 MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ensTs">Input argument #1</param>
    /// <param name="T12s">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray TemperatureCalibrationV02(MWArray ensTs, MWArray T12s)
    {
      return mcr.EvaluateFunction("TemperatureCalibrationV02", ensTs, T12s);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the TemperatureCalibrationV02
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] TemperatureCalibrationV02(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "TemperatureCalibrationV02", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the TemperatureCalibrationV02
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ensTs">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] TemperatureCalibrationV02(int numArgsOut, MWArray ensTs)
    {
      return mcr.EvaluateFunction(numArgsOut, "TemperatureCalibrationV02", ensTs);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the TemperatureCalibrationV02
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ensTs">Input argument #1</param>
    /// <param name="T12s">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] TemperatureCalibrationV02(int numArgsOut, MWArray ensTs, MWArray 
                                         T12s)
    {
      return mcr.EvaluateFunction(numArgsOut, "TemperatureCalibrationV02", ensTs, T12s);
    }


    /// <summary>
    /// Provides an interface for the TemperatureCalibrationV02 function in which the
    /// input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void TemperatureCalibrationV02(int numArgsOut, ref MWArray[] argsOut, 
                                MWArray[] argsIn)
    {
      mcr.EvaluateFunction("TemperatureCalibrationV02", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the
    /// TemperatureCalibrationV03 MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray TemperatureCalibrationV03()
    {
      return mcr.EvaluateFunction("TemperatureCalibrationV03", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the
    /// TemperatureCalibrationV03 MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="aquilaTs">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray TemperatureCalibrationV03(MWArray aquilaTs)
    {
      return mcr.EvaluateFunction("TemperatureCalibrationV03", aquilaTs);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the
    /// TemperatureCalibrationV03 MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="aquilaTs">Input argument #1</param>
    /// <param name="T12s">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray TemperatureCalibrationV03(MWArray aquilaTs, MWArray T12s)
    {
      return mcr.EvaluateFunction("TemperatureCalibrationV03", aquilaTs, T12s);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the TemperatureCalibrationV03
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] TemperatureCalibrationV03(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "TemperatureCalibrationV03", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the TemperatureCalibrationV03
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="aquilaTs">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] TemperatureCalibrationV03(int numArgsOut, MWArray aquilaTs)
    {
      return mcr.EvaluateFunction(numArgsOut, "TemperatureCalibrationV03", aquilaTs);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the TemperatureCalibrationV03
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="aquilaTs">Input argument #1</param>
    /// <param name="T12s">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] TemperatureCalibrationV03(int numArgsOut, MWArray aquilaTs, MWArray 
                                         T12s)
    {
      return mcr.EvaluateFunction(numArgsOut, "TemperatureCalibrationV03", aquilaTs, T12s);
    }


    /// <summary>
    /// Provides an interface for the TemperatureCalibrationV03 function in which the
    /// input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void TemperatureCalibrationV03(int numArgsOut, ref MWArray[] argsOut, 
                                MWArray[] argsIn)
    {
      mcr.EvaluateFunction("TemperatureCalibrationV03", numArgsOut, ref argsOut, argsIn);
    }



    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
