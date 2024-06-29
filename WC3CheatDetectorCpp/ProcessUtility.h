/*
	Utility functions for getting information about processess.
*/

#pragma once
#include <windows.h>

extern "C"
{
	__declspec(dllexport) DWORD FindProcessId(const WCHAR* processName);
	__declspec(dllexport) BOOL FindOpenFile(DWORD processID, const WCHAR* fileNameSubstring, WCHAR* fileDirBuffer, int bufferLength);
}
