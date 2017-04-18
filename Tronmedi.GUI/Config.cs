using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tronmedi.GUI
{
	class Config
	{
		public struct StillImageInfo
		{
			public int Width { get; set; }
			public int Height{ get; set; }
			public short ColorDepth { get; set; }
		}

		public const string Storage = "tronmedi.config.json";

		public static readonly Config Instance;

		static Config()
		{
			if (File.Exists(Storage))
			{
				try
				{
					var content = File.ReadAllText(Storage);
					Instance = JsonConvert.DeserializeObject<Config>(content);
					return;
				}
				catch
				{
					// fall through
				}
			}

			Instance = new Config();
			File.WriteAllText(Storage, JsonConvert.SerializeObject(Instance));
		}

		public string CameraVid { get; set; }
		public string CameraPid { get; set; }
		public StillImageInfo StillImage { get; set; }
	}
}
