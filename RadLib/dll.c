/// @file dll.c
/// This C code is necessary for building a cross-platform runtime library because it defines the appropriate syntax for
/// exporting and importing symbols in a dynamic-link library (DLL) or shared object (SO) file, depending on the target
/// platform.
/// 
/// Different platforms have different syntax for exporting and importing symbols in a DLL or SO file. The code checks
/// the target platform and sets the appropriate syntax for exporting and importing symbols, as well as defining the
/// visibility of the symbols. This ensures that the symbols can be correctly accessed by programs running on the target
/// platform, regardless of the platform on which the library was built.

// If the environment is Windows (or Cygwin)...
#if defined _WIN32 || defined __CYGWIN__
// and if we are building the DLL...
#if defined BUILDING_DLL
// and if we are using GCC...
#if defined __GNUC__
      // then we use GCC's special syntax for exporting symbols.
      #define DLL_PUBLIC __attribute__ ((dllexport))
#else
// otherwise we use the standard syntax for exporting symbols.
#define DLL_PUBLIC __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.
#endif
// otherwise, if we are not building the DLL...
#else
// and if we are using GCC...
#if defined __GNUC__
      // then we use GCC's special syntax for importing symbols.
      #define DLL_PUBLIC __attribute__ ((dllimport))
#else
      // otherwise we use the standard syntax for importing symbols.
      #define DLL_PUBLIC __declspec(dllimport) // Note: actually gcc seems to also supports this syntax.
#endif
#endif
// Define DLL_LOCAL as empty for Windows (or Cygwin).
#define DLL_LOCAL
// otherwise, if the environment is not Windows (or Cygwin)...
#else
// and if we are using GCC >= 4...
#if __GNUC__ >= 4
    // then we use GCC's special syntax for visibility.
    // Define DLL_PUBLIC as visible and DLL_LOCAL as hidden.
    #define DLL_PUBLIC __attribute__ ((visibility ("default")))
    #define DLL_LOCAL  __attribute__ ((visibility ("hidden")))
#else
    // otherwise, if we are not using GCC >= 4...
    // then we define DLL_PUBLIC and DLL_LOCAL as empty because they are not supported nor needed.
    #define DLL_PUBLIC
    #define DLL_LOCAL
#endif
#endif
