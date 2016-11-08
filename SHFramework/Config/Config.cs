using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace SHFramework.Config
{
	class Config
	{
		private readonly string[] FrameworkDefaultConfigTable =
		{
			"Maximun_Modules", "16",

			"", ""
		};

		private const string SettingConfigPath = @"\Config\FramworkSetting.cfg";
		XmlDocument xmlConfig = new XmlDocument();

		public Config()
		{
			if (File.Exists(SettingConfigPath))
				LoadConfigData();
			else
				CreativeConfig();
		}

		private void LoadConfigData()
		{

		}

		private void CreativeConfig()
		{
			xmlConfig.AppendChild(xmlConfig.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
			XmlNode rootConfigNode = xmlConfig.CreateElement("", "FrameworkSetting", "");

			rootConfigNode.AppendChild(CreativeXmlNode(xmlConfig, "", ""));

			xmlConfig.Save(SettingConfigPath);
		}

		private XmlNode CreativeXmlNode(XmlDocument parentDoc, string name, string text)
		{
			XmlNode node = parentDoc.CreateElement(string.Empty, name, string.Empty);
			node.InnerText = text;

			return node;
		}
	}
}
