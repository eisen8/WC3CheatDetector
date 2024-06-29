/*
	RAII wrapper for Handles
*/

#pragma once
#include <windows.h>
class SafeHandle
{
public:
	HANDLE Handle;
	SafeHandle(HANDLE handle);
	SafeHandle(USHORT handle);
	~SafeHandle();
	bool IsNullOrInvalid();
};

