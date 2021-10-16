#include "SafeHandle.h"

SafeHandle::SafeHandle(HANDLE handle)
{
	Handle = handle;
}

SafeHandle::SafeHandle(USHORT handle)
{
	Handle = (HANDLE) handle;
}

SafeHandle::~SafeHandle()
{
	if (!IsNullOrInvalid())
	{
		CloseHandle(Handle);
		Handle = NULL;
	}
}

bool SafeHandle::IsNullOrInvalid()
{
	return (Handle == NULL || Handle == INVALID_HANDLE_VALUE || Handle == nullptr);
}
