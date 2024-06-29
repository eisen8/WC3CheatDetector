/*
	Functions for setting and getting PU (ProcessUtility) specific error codes.
*/

#pragma once
#include <windows.h>

namespace PU
{
	#define PU_STATUS DWORD 
	#define PU_SUCCESS(Status) (((PU_STATUS)(Status)) >= 0)

	extern "C"
	{
		__declspec(dllexport) PU_STATUS GetLastPUError(WCHAR* messageBuffer, rsize_t bufferLength);
	}

	void SetLastError(PU_STATUS errorStatusCode);
	void SetLastError(PU_STATUS errorStatusCode, const CHAR* errorMessage);
}