#include "ProcessUtility.h"
#include "PUErrorCodes.h"
#include <windows.h>
#include <tlhelp32.h>
#include "NTDefinitions.h"
#include "SafeHandle.h"
#include <string>
#include <memory>
#include <system_error>

using namespace NTDefinitions;

struct free_d
{
    void operator()(void* x) { free(x); }
};

std::string getSysErrorMessage(const DWORD errorId)
{
    try
    {
        if (errorId == 0) {
            return std::string(); //No error message has been recorded
        }
        else {
            return std::system_category().message(errorId);
        }
    }
    catch (...)
    {
        return std::string();
    }
}

BOOL isBufferSizeError(NTSTATUS statusCode)
{
    // It is not clear to me the exact differences between these statusCodes and the documentation doesn't explain it either.
    // Experimentally when the buffer isn't big enough the APIs return STATUS_INFO_LENGTH_MISMATCH. I am not sure the use cases
    // that the others are used but documentation suggests they are important and related to the buffer not being large enough.
    return statusCode == STATUS_INFO_LENGTH_MISMATCH || statusCode == STATUS_BUFFER_TOO_SMALL || statusCode == STATUS_BUFFER_OVERFLOW;
}

/// <summary>
/// Find a process id from a given name. 
/// </summary>
/// <param name="processName">The name of the process</param>
/// <returns>The process Id if found or 0 if not found.</returns>
DWORD FindProcessId(const WCHAR* processName)
{
    try
    {
        // Get snapshot
        SafeHandle snap = SafeHandle(CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL));
        if (snap.IsNullOrInvalid())
        {
            PU::SetLastError(2);
            return 0;
        }

        // Look at each process to find the one matching the processName
        PROCESSENTRY32 processEntry;
        processEntry.dwSize = sizeof(processEntry);
        if (!Process32First(snap.Handle, &processEntry))
        {
            // Unable to get any processes
            DWORD errorId = GetLastError();
            std::string errorMessage = "SysError " + std::to_string(errorId) + ": " + getSysErrorMessage(errorId);
            errorId != 0 ? PU::SetLastError(3, errorMessage.c_str()) : PU::SetLastError(3);
            return 0;
        }
 
        do
        {
            if (wcscmp(processEntry.szExeFile, processName) == 0)
            {
                return processEntry.th32ProcessID;
            }

        } while (Process32Next(snap.Handle, &processEntry));

        return 0; 
    }
    catch (const std::exception& ex)
    {
        PU::SetLastError(1, ex.what());
        return 0;
    }
}

/// <summary>
/// Finds an open file opened by a processId that matches a substring.
/// </summary>
/// <param name="processId">The id of process to check</param>
/// <param name="fileNameSubstring"> The </param>
/// <param name="fileDirBuffer">A buffer to store the output fileDirectory</param>
/// <param name="bufferLen">The maximum length of the buffer</param>
/// <returns>True if we found the open file matching the substring. False if not.</returns>
BOOL FindOpenFile(DWORD processId, const WCHAR* fileNameSubstring, WCHAR* fileDirBuffer, int bufferLength)
{
    try
    {
        HMODULE ntdll = GetModuleHandleA("ntdll.dll");
        HANDLE currentProcess = GetCurrentProcess();
        if (ntdll == NULL)
        {
            PU::SetLastError(20);
            return FALSE;
        }

        NtQuerySystemInformation ntQuerySystemInformation = (NtQuerySystemInformation)GetProcAddress(ntdll, LPCSTR("NtQuerySystemInformation"));
        NtDuplicateObject ntDuplicateObject = (NtDuplicateObject)GetProcAddress(ntdll, LPCSTR("NtDuplicateObject"));
        NtQueryObject ntQueryObject = (NtQueryObject)GetProcAddress(ntdll, LPCSTR("NtQueryObject"));

        SafeHandle processDup = SafeHandle(OpenProcess(PROCESS_DUP_HANDLE, FALSE, processId));
        if (processDup.IsNullOrInvalid())
        {
            PU::SetLastError(21);
            return FALSE;
        }

        // NTQuerySystemInformation. Updating buffer size as needed.
        ULONG handleInfoSize = 0x300000;
        std::unique_ptr<SYSTEM_HANDLE_INFORMATION, free_d> handleInfo((PSYSTEM_HANDLE_INFORMATION)malloc(handleInfoSize));
        NTSTATUS status; ULONG returnLength;
        while (isBufferSizeError(status = ntQuerySystemInformation(SystemHandleInformation, handleInfo.get(), handleInfoSize, &returnLength)))
        {
            handleInfoSize = returnLength + 0x100000;
            void* tmp = realloc(handleInfo.get(), handleInfoSize);
            if (tmp == NULL)
            {
                PU::SetLastError(22);
                return FALSE;
            }

            handleInfo.release();
            handleInfo.reset((PSYSTEM_HANDLE_INFORMATION)tmp);
        }

        if (!NT_SUCCESS(status) || handleInfo == nullptr)
        {
            std::string ntStatus = "NTStatus code: " + std::to_string(status) + ".";
            PU::SetLastError(23, ntStatus.c_str());
            return FALSE;
        }

        // Look at each handle owned by the process to find the in-use (locked) file handle
        for (ULONG i = 0; i < handleInfo->NumberOfHandles; i++)
        {
            SYSTEM_HANDLE sysHandle = handleInfo->Handles[i];
            if (sysHandle.ProcessId != processId)
            {
                // PID does not match. Ignore.
                continue;
            }

            if (sysHandle.ObjectTypeIndex != 37 && sysHandle.ObjectTypeIndex != 25 &&
                sysHandle.ObjectTypeIndex != 26 && sysHandle.ObjectTypeIndex != 28)
            {
                // Not a file handle. Ignore. 
                continue;
            }

            // Duplicate the file handle so we can query it
            SafeHandle fileHandleDup = SafeHandle(HANDLE());
            if (!NT_SUCCESS(ntDuplicateObject(processDup.Handle, (HANDLE)sysHandle.HandleValue, currentProcess, &fileHandleDup.Handle, NULL, FALSE, 0)))
            {
                continue;
            }

            // Call NTQueryObject. Updating buffer size as needed.
            ULONG objectNameInfoSize = 0x10000;
            std::unique_ptr<OBJECT_NAME_INFORMATION, free_d> objectNameInfo ((POBJECT_NAME_INFORMATION) malloc(objectNameInfoSize));
            while (isBufferSizeError(status = ntQueryObject(fileHandleDup.Handle, ObjectNameInformation, objectNameInfo.get(), objectNameInfoSize, &returnLength)))
            {
                objectNameInfoSize = returnLength + 0x10000;
                void* tmp = realloc(objectNameInfo.get(), objectNameInfoSize);
                if (tmp == NULL)
                {
                    PU::SetLastError(22);
                    return FALSE;
                }

                objectNameInfo.release();
                objectNameInfo.reset((POBJECT_NAME_INFORMATION)tmp);
            }

            if (!NT_SUCCESS(status) || objectNameInfo == nullptr)
            {
                continue;
            }

            // Check if found process matches the one we are looking for
            UNICODE_STRING objectName = objectNameInfo->Name;
            if (objectName.Length > 0)
            {
                if (wcsstr(objectName.Buffer, fileNameSubstring) != nullptr)
                {
                    wcsncpy_s(fileDirBuffer, bufferLength, objectName.Buffer, objectName.Length);
                    return TRUE;
                }
            }
        }

        return FALSE;
    }
    catch (const std::exception& ex)
    {
        PU::SetLastError(1, ex.what());
        return FALSE;
    }
}
