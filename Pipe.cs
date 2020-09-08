using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using OxygenU_API;
using System.Windows.Forms;

namespace OxygenU_API
{
    class Pipe
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        public static bool NamedPipeExist(string pipeName)
        {
            bool result;
            try
            {
                int timeout = 0;
                if (!Pipe.WaitNamedPipe(Path.GetFullPath(string.Format("\\\\.\\pipe\\{0}", pipeName)), timeout))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 0)
                    {
                        return false;
                    }
                    if (lastWin32Error == 2)
                    {
                        return false;
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static void MainPipeClient(string pipe, string input)
        {
            if (Pipe.NamedPipeExist(pipe))
            {
                try
                {
                    using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", pipe, PipeDirection.Out))
                    {
                        namedPipeClientStream.Connect();
                        using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream))
                        {
                            streamWriter.Write(input);
                            streamWriter.Dispose();
                        }
                        namedPipeClientStream.Dispose();
                    }
                    return;
                }
                catch (IOException)
                {
                    System.Windows.Forms.MessageBox.Show("Pipe Is Uncorrect!", "Connection Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                   System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
                    return;
                }
            }
           System.Windows.Forms.MessageBox.Show("Error occured, Please Disable Your Antivirus!", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public class BasicInject
        {
            [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
            internal static extern IntPtr LoadLibraryA(string lpFileName);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            internal static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll")]
            internal static extern IntPtr OpenProcess(Pipe.BasicInject.ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

            [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll")]
            internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

            public bool InjectDLL()
            {
                if (Process.GetProcessesByName("RobloxPlayerBeta").Length == 0)
                {
                    return false;
                }
                Process process = Process.GetProcessesByName("RobloxPlayerBeta")[0];
                byte[] bytes = new ASCIIEncoding().GetBytes(AppDomain.CurrentDomain.BaseDirectory + Client.name);
                IntPtr hModule = Pipe.BasicInject.LoadLibraryA("kernel32.dll");
                UIntPtr procAddress = Pipe.BasicInject.GetProcAddress(hModule, "LoadLibraryA");
                Pipe.BasicInject.FreeLibrary(hModule);
                if (procAddress == UIntPtr.Zero)
                {
                    return false;
                }
                IntPtr intPtr = Pipe.BasicInject.OpenProcess(Pipe.BasicInject.ProcessAccess.AllAccess, false, process.Id);
                if (intPtr == IntPtr.Zero)
                {
                    return false;
                }
                IntPtr intPtr2 = Pipe.BasicInject.VirtualAllocEx(intPtr, (IntPtr)0, (uint)bytes.Length, 12288u, 4u);
                UIntPtr uintPtr;
                IntPtr intPtr3;
                return !(intPtr2 == IntPtr.Zero) && Pipe.BasicInject.WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, out uintPtr) && !(Pipe.BasicInject.CreateRemoteThread(intPtr, (IntPtr)0, 0u, procAddress, intPtr2, 0u, out intPtr3) == IntPtr.Zero);
            }

            [Flags]
            public enum ProcessAccess
            {
                AllAccess = 1050235,
                CreateThread = 2,
                DuplicateHandle = 64,
                QueryInformation = 1024,
                SetInformation = 512,
                Terminate = 1,
                VMOperation = 8,
                VMRead = 16,
                VMWrite = 32,
                Synchronize = 1048576
            }
        }
    }
}
