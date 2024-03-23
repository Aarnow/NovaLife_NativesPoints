using Life;
using Life.CheckpointSystem;
using Life.DB;
using Life.Network;
using ModKit.Helper;
using ModKit.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NativePoints
{
    public class NativePoints : ModKit.ModKit
    {
        public static string ConfigDirectoryPath;
        public static string ConfigAmboiseFilePath;
        public static string ConfigSaintBranchFilePath;
        public List<Point> AmboisePoints = new List<Point>();
        public List<Point> SaintBranchPoints = new List<Point>();
        public bool isAmboise { get; set; }

        public NativePoints(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Aarnow");
            isAmboise = Nova.mapId == 0 ? true : false;

            /*new SChatCommand("/tpdebug",
                "tpdebug",
                "/tpdebug pos",
                ((player, arg) =>
                {
                    if (player.IsAdmin && arg[0] != null && arg[1] != null && arg[2] != null)
                    {
                        if(float.TryParse(arg[0], out float x))
                        {
                            if (float.TryParse(arg[1], out float y))
                            {
                                if (float.TryParse(arg[2], out float z))
                                {
                                    player.setup.TargetSetPosition(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                })).Register();*/
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            InitDirectoryAndFiles();
            AmboisePoints = LoadConfigFile(ConfigAmboiseFilePath);
            SaintBranchPoints = LoadConfigFile(ConfigSaintBranchFilePath);
            ModKit.Internal.Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }

        private void InitDirectoryAndFiles()
        {
            try
            {
                ConfigDirectoryPath = DirectoryPath + "/NativePoints";
                ConfigAmboiseFilePath = Path.Combine(ConfigDirectoryPath, "amboise.json");
                ConfigSaintBranchFilePath = Path.Combine(ConfigDirectoryPath, "saintbranch.json");

                if (!Directory.Exists(ConfigDirectoryPath)) Directory.CreateDirectory(ConfigDirectoryPath);
                if (!File.Exists(ConfigAmboiseFilePath)) InitAmboiseFile();
                if (!File.Exists(ConfigSaintBranchFilePath)) InitSaintBranchFile();
            }
            catch (IOException ex)
            {
                ModKit.Internal.Logger.LogError("InitDirectory", ex.Message);
            }
        }
        private void InitAmboiseFile()
        {
            List<Point> amboisePoints = new List<Point>
            {
                new Point("Commissariat", true, new Vector3(361.64f, 50.15f, 640.53f)),
                new Point("Hopital", true, new Vector3(939.22f, 53.12f, 546.67f)),
                new Point("Amboise materiaux - amboise", true, new Vector3(364.78f, 50.04f, 827.40f)),
                new Point("Amboise materiaux - reigneire", true, new Vector3(275.52f, 45.20f, -1263.51f)),
                new Point("Kioba - Achat de vêtements", true, new Vector3(403.51f, 44.99f, -1.17f)),
                new Point("Auto école", true, new Vector3(293.03f, 50.01f, 721.51f)),
                new Point("Pôle travail", true, new Vector3(509.51f, 51.06f, 363.83f)),
                new Point("Règle de Nova-Life", true, new Vector3(758.27f, 50.00f, 675.66f)),
                new Point("Garage virtuel", true, new Vector3(608.45f, 50.03f, 995.24f)),
                new Point("Spawn - loader", true, new Vector3(4343.92f, 104.77f, -2183.27f)),
                new Point("Spawn", true, new Vector3(751.50f, 50.02f, 694.92f)),
                new Point("Spawn - offmap", true, new Vector3(0.00f, -75.00f, 0.00f)),
                new Point("Concessionnaire", true, new Vector3(624.16f, 50.02f, 975.82f))
            };

            string amboiseJson = JsonConvert.SerializeObject(amboisePoints, Formatting.Indented);
            File.WriteAllText(ConfigAmboiseFilePath, amboiseJson);
        }
        private void InitSaintBranchFile()
        {
            List<Point> saintbranchPoints = new List<Point>
            {
                new Point("commissariat", true, new Vector3(77.64f, 45.10f, 405.79f)),
                new Point("hopital", true, new Vector3(279.34f, 45.10f, 519.10f)),
                new Point("amboise materiaux", true, new Vector3(692.29f, 50.03f, 329.83f)),
                new Point("amboise materiaux - offmap", true, new Vector3(0.00f, -50.00f, 0.00f)),
                new Point("Kioba - Achat de vêtements", true,new Vector3(471.80f, 45.10f, 262.60f)),
                new Point("Auto école", true, new Vector3(257.06f, 45.10f, 554.29f)),
                new Point("Pôle travail", true, new Vector3(256.00f, 45.10f, 679.55f)),
                new Point("Règle de Nova-Life", true, new Vector3(354.71f, 45.10f, 515.04f)),
                new Point("Garage virtuel", true, new Vector3(295.55f, 45.10f, 748.08f)),
                new Point("Spawn - loader", true, new Vector3(4343.92f, 104.77f, -2183.27f)),
                new Point("Spawn", true,new Vector3(349.41f, 45.02f, 524.83f)),
                new Point("Spawn - offmap", true,new Vector3(0.00f, -75.00f, 0.00f)),
                new Point("Concessionnaire", true,new Vector3(624.16f, 50.02f, 975.82f))
            };

            string saintbranchJson = JsonConvert.SerializeObject(saintbranchPoints, Formatting.Indented);
            File.WriteAllText(ConfigSaintBranchFilePath, saintbranchJson);
        }
        private List<Point> LoadConfigFile(string path)
        {
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);
                List<Point> points = JsonConvert.DeserializeObject<List<Point>>(jsonContent);

                return points;
            }
            else return null;
        }

        public override void OnPlayerSpawnCharacter(Player player, Mirror.NetworkConnection conn, Characters character)
        {
            base.OnPlayerSpawnCharacter(player, conn, character);
            InitNativePoints(player);
        }

        private void InitNativePoints(Player player)
        {
            Console.WriteLine("InitNativePoints !");
            Console.WriteLine($"{AmboisePoints.Count}");

            foreach (NCheckpoint cp in Nova.server.checkpoints)
            {
                foreach (var point in Nova.mapId == 0 ? AmboisePoints : SaintBranchPoints)
                {
                    if (isNativeCheckpoint(cp, point.VPosition, 0.01f))
                    {
                        player.setup.TargetDestroyCheckpoint(cp.checkpointId);
                    }
                }
            }
        }

        public bool isNativeCheckpoint(NCheckpoint cp, Vector3 nativePosition, float tolerance)
        {
            return (Math.Abs(cp.position.x - nativePosition.x) < tolerance
                        && Math.Abs(cp.position.y - nativePosition.y) < tolerance
                        && Math.Abs(cp.position.z - nativePosition.z) < tolerance);
        }
    }
}
