/*
    Definitions related to using the NT APIS.
*/

#pragma once
#include <string>
#include <windows.h>
#include <tlhelp32.h>

// The following includes aren't needed but are useful to look into to see various definitions
//#include <Winternl.h>
//#include <Ntdef.h>
//#include <ntstatus.h>

namespace NTDefinitions
{

    #define NT_SUCCESS(Status) (((NTSTATUS)(Status)) >= 0) // Defined originally in Ntdef.h

    // Defined originally in ntstatus.h
    #define STATUS_INFO_LENGTH_MISMATCH 0xC0000004L // The specified information record length does not match the length required for the specified information class.
    #define STATUS_BUFFER_TOO_SMALL 0xC0000023L // The buffer is too small to contain the entry. No information has been written to the buffer.
    #define STATUS_BUFFER_OVERFLOW 0x80000005L // The data was too large to fit into the specified buffer.

     // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/class.htm
    #define SystemHandleInformation 0x10

    // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ob/obquery/class.htm
    typedef enum _OBJECT_INFORMATION_CLASS
    {
        ObjectBasicInformation = 0,
        ObjectNameInformation = 1,
        ObjectTypeInformation = 2,
    } OBJECT_INFORMATION_CLASS;

    // https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntquerysysteminformation
    typedef NTSTATUS(NTAPI* NtQuerySystemInformation)
        (
            ULONG SystemInformationClass,
            PVOID SystemInformation,
            ULONG SystemInformationLength,
            PULONG ReturnLength // Optional
            );

    // http://undocumented.ntinternals.net/index.html?page=UserMode%2FUndocumented%20Functions%2FNT%20Objects%2FType%20independed%2FNtDuplicateObject.html
    // https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-duplicatehandle
    typedef NTSTATUS(NTAPI* NtDuplicateObject)
        (
            HANDLE SourceProcessHandle,
            HANDLE SourceHandle, // This is actually a Handle (like in the microsoft link) NOT PHandle (like in ntinternals).
            HANDLE TargetProcessHandle,
            PHANDLE TargetHandle,
            ACCESS_MASK DesiredAccess, //Optional
            BOOL InheritHandle,
            ULONG Options
            );

    // https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryobject
    typedef NTSTATUS(NTAPI* NtQueryObject)
        (
            HANDLE Handle,
            ULONG ObjectInformationClass,
            PVOID ObjectInformation,
            ULONG ObjectInformationLength,
            PULONG ReturnLength // Optional
            );

    // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/inc/shared/ntdef/unicode_string.htm
    typedef struct _UNICODE_STRING
    {
        USHORT Length;
        USHORT MaximumLength;
        PWCHAR Buffer;
    } UNICODE_STRING, *PUNICODE_STRING;

    // http://undocumented.ntinternals.net/index.html?page=UserMode%2FUndocumented%20Functions%2FNT%20Objects%2FType%20independed%2FOBJECT_INFORMATION_CLASS.html
    typedef struct _OBJECT_NAME_INFORMATION
    {
        UNICODE_STRING Name;
        WCHAR NameBuffer[1];
    } OBJECT_NAME_INFORMATION, *POBJECT_NAME_INFORMATION;

    // https://stackoverflow.com/questions/2547561/system-handle-information-structure
    // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/handle_table_entry.htm?ts=0,286
    typedef struct _SYSTEM_HANDLE
    {
        ULONG ProcessId;
        UCHAR ObjectTypeIndex;
        UCHAR HandleAttributes;
        USHORT HandleValue;
        PVOID Object;
        ULONG GrantedAccess;
    } SYSTEM_HANDLE, *PSYSTEM_HANDLE;

    // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/handle.htm
    typedef struct _SYSTEM_HANDLE_INFORMATION
    {
        ULONG NumberOfHandles;
        SYSTEM_HANDLE Handles[1];
    } SYSTEM_HANDLE_INFORMATION, *PSYSTEM_HANDLE_INFORMATION;
}