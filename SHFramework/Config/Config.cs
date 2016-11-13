using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace SHFramework.Config
{
	class Config
	{
		private const string SettingConfigPath = @"\Config\FramworkSetting.cfg";
		private const string ChecksumPath = @"\Config\FrameworkConfigChecksum.chksm";
		private const string KeyPath = @"\Config\FrameworkConfigEncryptionKey.enckey";

		private const string RootNodeName = "GlobalFrameworkSetting";
			private const string ModuleNodeName = "ModuleSetting";
			private const string KernelNodeName = "KernelSetting";


		// Type Define.
		// B : Boolean
		// S : String
		// I : Int
		// Example -> "B:IsModule" -> 'B', 'IsModule'. (Boolean Type "IsModule" Data)
		private readonly string[,] DefaultKernelSetting =
		{
			{ "B:Encryption_Config", "false" },
			{ "S:Encryption_Config_Checksum", "" },
		};

		private readonly string[,] DefaultModuleSetting =
		{
			{ "I:Maximun_Modules", "16" },
			{ "S:Prefix_Module_Path", "" },

			// 1: None | 2: Uri | 3: IPAddress | 4: Custom(object[]) | 5: All //
			{ "I:Using DoWork Method", "5" }
		};

		XmlDocument xmlConfig = new XmlDocument();

		public Config()
		{
			if (!File.Exists(SettingConfigPath))
				CreateConfig();
		}

		private void LoadConfigData()
		{

		}

		private void CreateConfig()
		{
			xmlConfig.AppendChild(xmlConfig.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
			XmlNode rootConfigNode = xmlConfig.CreateElement(string.Empty, RootNodeName, string.Empty);


			// Kernel Setting //
			XmlNode kernelConfigNode = CreateXmlNode(xmlConfig, KernelNodeName, string.Empty);

			for (int i = 0; i < DefaultKernelSetting.Length; i++)
				kernelConfigNode.AppendChild(CreateXmlNode(xmlConfig, DefaultKernelSetting[i, 0], DefaultKernelSetting[i, 1]));

			rootConfigNode.AppendChild(kernelConfigNode);
			// END //


			// Module Config //
			XmlNode moduleConfigNode = CreateXmlNode(xmlConfig, ModuleNodeName, string.Empty);

			for (int i = 0; i < DefaultModuleSetting.Length; i++)
				moduleConfigNode.AppendChild(CreateXmlNode(xmlConfig, DefaultModuleSetting[i, 0], DefaultModuleSetting[i, 1]));

			rootConfigNode.AppendChild(moduleConfigNode);
			// END //

			xmlConfig.Save(SettingConfigPath);

			byte[] Key = new byte[256];

			using (StreamWriter sw = new StreamWriter(KeyPath))
			{
				Random rd = new Random(Environment.CurrentManagedThreadId);
				rd.NextBytes(Key);
				sw.Write(rd.Next());
				sw.Flush();
			}



			CreateEncryptionChecksum();
		}

		// Encryption //
		private static void CreateEncryptionChecksum()
		{
			SHA256 enc = new SHA256CryptoServiceProvider();
			Stream st;

			if (File.Exists(ChecksumPath))
				st = File.Open(ChecksumPath, FileMode.Open);
			else
				st = File.Open(ChecksumPath, FileMode.Create);

			using (StreamWriter sw = new StreamWriter(st))
			{
				sw.Write(Convert.ToString(enc.ComputeHash(File.Open(SettingConfigPath, FileMode.Open))));
				st.Dispose();
			}
			
			return;
		}

		private XmlNode CreateXmlNode(XmlDocument parentDoc, string name, string text)
		{
			XmlNode node = parentDoc.CreateElement(string.Empty, name, string.Empty);
			node.InnerText = text;

			return node;
		}
	}
}
