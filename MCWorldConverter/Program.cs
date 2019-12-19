using System;
using System.Collections.Generic;
using System.Linq;
using MCWorldConverter.Data;
using NBTExplorer.Model;

namespace MCWorldConverter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("- MCWorldConverter by xParadoxical_ -");
            Console.WriteLine("Note: *You can ONLY downgrade worlds from 1.14 to 1.8 using this tool!");
            Console.WriteLine("      *This tool edits only blocks, tile entities and paintings!");
            Console.WriteLine();

            var worldDir = GetWorldDir();
            var worldManager = WorldManager.FromWorldDir(worldDir);
            Console.WriteLine($"World contains {worldManager.RegionFiles.Count} region files.");
            Console.WriteLine($"There are {worldManager.ChunkManagers.Count} chunks.");

            Console.WriteLine();
            Console.WriteLine("Conversion details:");
            (ChunkInfo n, ChunkInfo p) = GetConversionRange();
            Console.WriteLine();

            chunks = chunks.Where(pair => pair.Key.IsIn(n, p)).ToDictionary(pair => pair.Key, pair => pair.Value);
            Console.WriteLine($"{chunks.Count} chunk(s) will be converted.");



            Exit();
        }

        private static void ExitIfDirEmpty(DirectoryDataNode dir)
        {
            if (!dir.IsExpanded) dir.Expand();
            if (dir.Nodes.Count == 0) Exit("Directory " + dir.NodeDirPath + " is empty.");
        }

        private static DirectoryDataNode GetWorldDir()
        {
            bool isWin = (int)Environment.OSVersion.Platform < 4;
            Console.Write("Path to the world directory" + (isWin ? " (you can use environment variables)" : "") + ": ");
#if DEBUG
            string path = @"%appdata%\.minecraft\saves\New World-";
            Console.WriteLine(path);
#else
            string path = Console.ReadLine();
#endif
            if (isWin) path = Environment.ExpandEnvironmentVariables(path);

            var dir = new DirectoryDataNode(path);
            ExitIfDirEmpty(dir);

            return dir;
        }

        private static DirectoryDataNode GetRegionDir(DirectoryDataNode worldDir)
        {
            var regionDir = worldDir.Nodes.FirstOrDefault(node => (node as DirectoryDataNode)?.NodePathName == "region") as DirectoryDataNode;
            if (regionDir is null)
                Exit("Directory 'region' doesn't exist.");
            else
                Console.WriteLine("Found 'region' directory.");

            ExitIfDirEmpty(regionDir);

            return regionDir;
        }

        private static (ChunkInfo n, ChunkInfo p) GetConversionRange()
        {
            Console.Write("X of the north-west (X-, Z-) corner: ");
            var ax = int.Parse(Console.ReadLine());
            Console.Write("Z of the north-west (X-, Z-) corner: ");
            var az = int.Parse(Console.ReadLine());
            Console.Write("X of the south-east (X+, Z+) corner: ");
            var bx = int.Parse(Console.ReadLine());
            Console.Write("Z of the south-east (X+, Z+) corner: ");
            var bz = int.Parse(Console.ReadLine());

            (Point n, Point p) = (new Point(ax, az), new Point(bx, bz));
            if (!(n < p)) Exit("Invalid range.");

            return (ChunkInfo.FromBlockInWorld(n.x, n.y), ChunkInfo.FromBlockInWorld(p.x, p.y));
        }

        private static void Exit(string msg = null)
        {
            if (msg != null)
            {
                var fg = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = fg;
            }
            Console.ReadKey(true);
            Environment.Exit(0);
        }
    }
}
