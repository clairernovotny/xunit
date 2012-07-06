using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Xunit.Utility
{
    class AppContainerUtilities
    {
        private static readonly bool isCurrentProcessInAppContainer;


        static AppContainerUtilities()
        {
            isCurrentProcessInAppContainer = NativeMethods.RunningInAppContainer(NativeMethods.GetProcessToken(Process.GetCurrentProcess().Handle));

        }

        public static bool IsCurrentProcessInAppContainer
        {
            get
            {
                return isCurrentProcessInAppContainer;
            }
        }

        private class NativeMethods
        {

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("KERNEL32.DLL", SetLastError = true)]
            internal static extern bool CloseHandle(IntPtr handle);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("ADVAPI32.DLL", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            private static extern bool GetTokenInformation(SafeTokenHandle tokenHandle, TOKEN_INFORMATION_CLASS tokenInformationClass, out uint tokenInformation, uint tokenInformationLength, out uint returnLength);
         
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("ADVAPI32.DLL", EntryPoint = "OpenProcessToken", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            private static extern bool GetCurrentProcessToken(IntPtr ProcessHandle, TokenAccessLevels DesiredAccess, out SafeTokenHandle TokenHandle);

            [SecurityCritical]
            internal static SafeTokenHandle GetProcessToken(IntPtr processHandle)
            {
                SafeTokenHandle handle;
                if (!GetCurrentProcessToken(processHandle, TokenAccessLevels.Query, out handle))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return handle;
            }

            internal static bool RunningInAppContainer(SafeTokenHandle tokenHandle)
            {
                uint num;
                uint num2;
                Version version = new Version(6, 2);
                if (Environment.OSVersion.Version < version)
                {
                    return false;
                }
                if (!GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenIsAppContainer, out num, 4, out num2))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return (num == 1);
            }

            internal sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
            {
                // Methods
                public SafeTokenHandle()
                    : base(true)
                {
                }

                public SafeTokenHandle(IntPtr handle)
                    : base(true)
                {
                    base.SetHandle(handle);
                }

                protected override bool ReleaseHandle()
                {
                    return AppContainerUtilities.NativeMethods.CloseHandle(base.handle);
                }
            }

 


            internal enum TOKEN_INFORMATION_CLASS
            {
                MaxTokenInfoClass = 30,
                TokenAccessInformation = 0x16,
                TokenAuditPolicy = 0x10,
                TokenDefaultDacl = 6,
                TokenElevation = 20,
                TokenElevationType = 0x12,
                TokenGroups = 2,
                TokenGroupsAndPrivileges = 13,
                TokenHasRestrictions = 0x15,
                TokenImpersonationLevel = 9,
                TokenIntegrityLevel = 0x19,
                TokenIsAppContainer = 0x1d,
                TokenLinkedToken = 0x13,
                TokenLogonSid = 0x1c,
                TokenMandatoryPolicy = 0x1b,
                TokenOrigin = 0x11,
                TokenOwner = 4,
                TokenPrimaryGroup = 5,
                TokenPrivileges = 3,
                TokenRestrictedSids = 11,
                TokenSandBoxInert = 15,
                TokenSessionId = 12,
                TokenSessionReference = 14,
                TokenSource = 7,
                TokenStatistics = 10,
                TokenType = 8,
                TokenUIAccess = 0x1a,
                TokenUser = 1,
                TokenVirtualizationAllowed = 0x17,
                TokenVirtualizationEnabled = 0x18
            }

 

 

        }
    }
}
