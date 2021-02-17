using System;
using System.IO;
using System.Reflection;
using CoreFtp;

namespace FTPFileUpload
{
	class Program
	{
		static async System.Threading.Tasks.Task Main(string[] args)
		{
			Arguments oArguments = new Arguments(args);

			// https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create
			if (oArguments.IsEmpty)
			{
				var versionString = Assembly.GetEntryAssembly()
																.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
																.InformationalVersion
																.ToString();

				Console.WriteLine($"FTPFileUpload v{versionString}");
				Console.WriteLine("-------------------------------------------------");
				Console.WriteLine("- COMMAND LINE HELP (* for mandatory arguments) -");
				Console.WriteLine("-------------------------------------------------");
				Console.WriteLine();
				Console.WriteLine("-Username=<string>        *");
				Console.WriteLine("-Password=<string>        *");
				Console.WriteLine("-Host=<string>            *");
				Console.WriteLine("-FileName=<string>            *");
				Console.WriteLine("-BaseDirectory=<string> *");

				return;
			}

			string sUsername = oArguments.GetParameter("Username", string.Empty);
			string sPassword = oArguments.GetParameter("Password", string.Empty);
			string sHost = oArguments.GetParameter("Host", string.Empty);
			string sFileName = oArguments.GetParameter("FileName", string.Empty);
			string sBaseDirectory = oArguments.GetParameter("BaseDirectory", string.Empty);

			using (var oClient = new FtpClient(new FtpClientConfiguration
			{
				Host = sHost,
				Username = sUsername,
				Password = sPassword,
				BaseDirectory = sBaseDirectory
			}))
			{
				await oClient.LoginAsync();

				using (var oWriteStream = await oClient.OpenFileWriteStreamAsync(sFileName))
				{
					FileInfo oFile = new FileInfo(sFileName);
					using (var oReader = oFile.OpenRead())
					{
						await oReader.CopyToAsync(oWriteStream);
					}
				}
			}
		}
	}
}
