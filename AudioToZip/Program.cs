using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AudioToZip
{
    /// <summary>
    /// wav->mp3->zipに変換するプログラム
    /// </summary>
    internal class Program
    {
        private const string FILE_EXT_WAV = ".wav";
        private const string FILE_EXT_MP3 = ".mp3";
        private const string FILE_EXT_ZIP = ".zip";

        /// <summary>
        /// プログラムのエントリポイント
        /// </summary>
        /// <param name="args"></param>
        private static void Main()
        {
            Console.WriteLine( $"#### {nameof( AudioToZip )} " +
                               $"Ver.{ Assembly.GetExecutingAssembly().GetName().Version } ####" );

            var args = Environment.GetCommandLineArgs();
            if ( ( args.Length > 1 ) && ( Directory.Exists( args[1] ) ) )
            {
                var dir = args[1];
                try
                {
                    WaveToMp3( dir );   // wav -> mp3
                    Mp3ToZip( dir );    // mp3 -> zip
                    DeleteSpecifiedTypeFiles( dir, FILE_EXT_MP3 );  // 不要なmp3ファイルを削除する
                }
                catch ( Exception )
                {
                    Console.WriteLine( "ファイルの変換に失敗しました。" );
                }
            }
            else
            {
                Console.WriteLine( "ディレクトリを指定して下さい。" );
            }
            Console.WriteLine( "処理が終了しました。何かキーを押して下さい..." );
            Console.ReadLine();
        }

        /// <summary>
        /// WaveファイルからMP3ファイルに変換する
        /// </summary>
        /// <param name="waveFiles"></param>
        private static void WaveToMp3( string dir )
        {
            var waveFiles = Directory.GetFiles( dir ).
                            Where( x => Path.GetExtension( x ).Equals( FILE_EXT_WAV ) );
            if ( waveFiles.Count() > 0 )
            {
                Console.WriteLine( "[wav -> mp3]" );

                // 変換に時間がかかるので並列処理する
                var processed  = 0;
                var lockObject = new object();
                Parallel.ForEach( waveFiles, wave =>
                {
                    var inputFile  = new MediaFile { Filename = wave };
                    var outputFile = new MediaFile { Filename = wave.Replace( FILE_EXT_WAV, FILE_EXT_MP3 ) };

                    using ( var engine = new Engine() )
                    {
                        engine.Convert( inputFile, outputFile );
                    }
                    lock ( lockObject )
                    {
                        Console.WriteLine( $"{ Path.GetFileName( wave ) } を変換しました。" +
                                           $"({ ++processed }/{ waveFiles.Count() })" );
                    }
                } );
            }
            else
            {
                Console.WriteLine( "waveファイルがありません。" );
            }
        }

        /// <summary>
        /// MP3ファイルからZipファイルに変換する
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="mp3Files"></param>
        private static void Mp3ToZip( string dir )
        {
            var mp3Files = Directory.GetFiles( dir ).
                              Where( x => Path.GetExtension( x ).Equals( FILE_EXT_MP3 ) );
            if ( mp3Files.Count() > 0 )
            {
                Console.WriteLine( "[mp3 -> zip]" );
                var outputZip = $"{ dir }{ FILE_EXT_ZIP }";

                // 既にzipファイルが存在する場合は削除する(圧縮時エラーになるため)
                if ( File.Exists( outputZip ) ) File.Delete( outputZip );

                using ( var zip = ZipFile.Open( outputZip, ZipArchiveMode.Update, Encoding.GetEncoding( "shift_jis" ) ) )
                {
                    // 圧縮に時間がかかるので並列処理する
                    var processed = 0;
                    var lockObject   = new object();
                    Parallel.ForEach( mp3Files, mp3 =>
                    {
                        var inputFile  = mp3;
                        var outputFile = mp3.Replace( FILE_EXT_MP3, FILE_EXT_ZIP );

                        zip.CreateEntryFromFile( inputFile, Path.GetFileName( inputFile ), CompressionLevel.Optimal );
                        lock ( lockObject )
                        {
                            Console.WriteLine( $"{ Path.GetFileName( mp3 ) } を圧縮しました。" +
                                               $"({ ++processed }/{ mp3Files.Count() })" );
                        }
                    } );
                    Console.WriteLine( $"圧縮ファイルを{ outputZip }に出力しました。" );
                }
            }
            else
            {
                Console.WriteLine( "mp3ファイルがありません。" );
            }
        }

        /// <summary>
        /// 指定されたディレクトリから任意の種類のファイルを全て削除する
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fileType"></param>
        private static void DeleteSpecifiedTypeFiles( string dir, string fileType )
        {
            var files = Directory.GetFiles( dir ).Where( x => Path.GetExtension( x ).Equals( fileType ) );
            foreach ( var file in files )
            {
                File.Delete( file );
            }
        }
    }
}