namespace MathCore.PE.Headers;

public static class IMAGE_DIRECTORY_ENTRY
{
    /// <summary>Export Directory</summary>
    public const int EXPORT = 0;
    /// <summary>Import Directory</summary>
    public const int IMPORT = 1;
    /// <summary>Resource Directory</summary>
    public const int RESOURCE = 2;
    /// <summary>Exception Directory</summary>
    public const int EXCEPTION = 3;
    /// <summary>Security Directory</summary>
    public const int SECURITY = 4;
    /// <summary>Base Relocation Table</summary>
    public const int BASERELOC = 5;
    /// <summary>Debug Directory</summary>
    public const int DEBUG = 6;
    /// <summary>Architecture Specific Data (X86 usage)</summary>
    public const int COPYRIGHT = 7;
    /// <summary>Architecture Specific Data</summary>
    public const int ARCHITECTURE = 7;
    /// <summary>RVA of GP</summary>
    public const int GLOBALPTR = 8;
    /// <summary>TLS Directory</summary>
    public const int TLS = 9;
    /// <summary>Load Configuration Directory</summary>
    public const int LOAD_CONFIG = 10;
    /// <summary>Bound Import Directory in headers</summary>
    public const int BOUND_IMPORT = 11;
    /// <summary>Import Address Table</summary>
    public const int IAT = 12;
    /// <summary>Delay Load Import Descriptors</summary>
    public const int DELAY_IMPORT = 13;
    /// <summary>COM Runtime descriptor</summary>
    public const int COM_DESCRIPTOR = 14;
}