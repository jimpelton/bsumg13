

#ifndef __Export_h
#define __Export_h

#ifdef _WIN32

#ifdef UG_DLL_EXPORTS
#define DllExport __declspec(dllexport)
#else 
#define DllExport __declspec(dllimport)
#endif

#endif /* ifdef _WIN32 */

#endif /* ifndef __Export_h */
