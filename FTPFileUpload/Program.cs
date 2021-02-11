﻿using System;
using System.IO;
using CoreFtp;

namespace FTPFileUpload
{
	class Program
	{
		static async System.Threading.Tasks.Task Main(string[] args)
		{
			Arguments oArguments = new Arguments(args);

			if (oArguments.IsEmpty)
			{
				Console.WriteLine("-------------------------------------------------");
				Console.WriteLine("- COMMAND LINE HELP (* for mandatory arguments) -");
				Console.WriteLine("-------------------------------------------------");
				Console.WriteLine();
				Console.WriteLine("-Username=<string>        *");
				Console.WriteLine("-Password=<string>        *");
				Console.WriteLine("-Host=<string>            *");
				Console.WriteLine("-File=<string>            *");
				Console.WriteLine("-RemoteDirectory=<string> *");

				return;
			}

			string sUsername = oArguments.GetParameter("Username", string.Empty);
			string sPassword = oArguments.GetParameter("Password", string.Empty);
			string sHost = oArguments.GetParameter("Host", string.Empty);
			string sFileName = oArguments.GetParameter("FileName", string.Empty);
			string sBaseDirectory = oArguments.GetParameter("BaseDirectory", string.Empty);

			using (var ftpClient = new FtpClient(new FtpClientConfiguration
			{
				Host = sHost,
				Username = sUsername,
				Password = sPassword,
				BaseDirectory = sBaseDirectory
			}))
			{
				await ftpClient.LoginAsync();

				FileInfo oFile = new FileInfo(sFileName);
				using (var ftpReadStream = await ftpClient.OpenFileReadStreamAsync(sFileName))
				{
					using (var fileWriteStream = oFile.OpenWrite())
					{
						await ftpReadStream.CopyToAsync(fileWriteStream);
					}
				}
			}
		}
	}
}