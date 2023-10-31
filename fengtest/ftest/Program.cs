
using System;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Hello World!");

		//string path = @"C:\Users\Administrator\Desktop\temp\cmdtest\genmd5\store_game_mini.zip";
		if(args.Length < 1) {
			Console.WriteLine("Usage: app.exe path2zip");
			System.Environment.Exit(2);
		}
		string path = args[0];

		if(File.Exists(path) == false) {
			Console.WriteLine("ZipNotFound: Zip[{0}]", path);
			System.Environment.Exit(3);
		}

		ZipFile zf = new ZipFile(path);

		long cnt = zf.Count;
		Console.WriteLine("ZipFile.Count=" + cnt);

		FileStream fsSrc = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

		string parentDir = "zipfolder";
		if(Directory.Exists(parentDir)) {
			Directory.Delete(parentDir, true);
		}

		for(int i=0;i<cnt;i++) {
			ZipEntry ze = zf[i];
			
			long realOffset = zf.LocateEntry(ze);

			if(ze.IsDirectory) {
				string dir = parentDir + "/" + ze.Name;
				if(Directory.Exists(dir) == false) {
					Directory.CreateDirectory(dir);
				}
			} else {
				ExtractFilePart(fsSrc, realOffset, ze.CompressedSize, parentDir + "/" + ze.Name);
			}

			Console.WriteLine("[{0}]: cmethod[{1}], file[{2}] size[{3}], off[{4}], realOff[{5}], name[{6}]",
							  ze.ZipFileIndex, ze.CompressionMethod, ze.IsFile, ze.Size, ze.Offset, realOffset, ze.Name);

		}

		fsSrc.Close();
		fsSrc = null;
	}
	public static long ExtractFilePart(FileStream fsSrc, long off, long len, string dst)
	{
		fsSrc.Seek(off, SeekOrigin.Begin);

		string dir = Path.GetDirectoryName(dst);
		if(Directory.Exists(dir) == false) {
			Directory.CreateDirectory(dir);
		}

		FileStream fsDst = new FileStream(dst, FileMode.Create, FileAccess.Write, FileShare.Read);

		byte [] buf = new byte[4096];

		long left = len;
		while(left > 0) {
			int toread = (int)Math.Min(buf.Length, left);
			int cnt = fsSrc.Read(buf, 0, toread);

			fsDst.Write(buf, 0, cnt);

			left = left - cnt;
		}

		fsDst.Close();

		return len;
	}
}

