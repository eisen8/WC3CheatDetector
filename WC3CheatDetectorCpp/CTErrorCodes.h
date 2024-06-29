/*
	Functions for setting and getting CT (Cheat tool) specific error codes.
*/

#pragma once
#include <windows.h>

namespace CT
{
	#define CTSTATUS DWORD 
	#define CT_SUCCESS(Status) (((CTSTATUS)(Status)) >= 0)

	extern "C"
	{
		__declspec(dllexport) CTSTATUS GetLastCTError(WCHAR* messageBuffer, rsize_t bufferLength);
	}

	void SetLastError(CTSTATUS errorStatusCode);
	void SetLastError(CTSTATUS errorStatusCode, const CHAR* errorMessage);
}