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
            if ( ( args.Length > 1 ) &&
                 ( Directory.Exists( args[1] ) || File.Exists( args[1] ) ) )
            {
                var path = args[1];
                try
                {
                    WaveToMp3( path );                               // wav -> mp3
                    Mp3ToZip( path );                                // mp3 -> zip
                    DeleteSpecifiedTypeFiles( path, FILE_EXT_MP3 );  // 不要なmp3ファイルを削除する
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
        private static void WaveToMp3( string path )
        {
            // path = フォルダの場合
            if ( Directory.Exists( path ) )
            {
                var waveFiles = Directory.GetFiles( path )
                                         .Where( x => Path.GetExtension( x ).Equals( FILE_EXT_WAV ) );
                if ( waveFiles.Count() > 0 )
                {
                    Console.WriteLine( "[wav -> mp3]" );

                    // 変換に時間がかかるので並列処理する
                    var processed  = 0;
                    var lockObject = new object();
                    Parallel.ForEach( waveFiles, wave =>
                    {
                        (var input, var output) = CreateMediaFile( wave );
                        lock ( lockObject )
                        {
                            using ( var engine = new Engine() )
                                engine.Convert( input, output );

                            Console.WriteLine( $"{ Path.GetFileName( wave ) } を変換しました。" +
                                               $"({ ++processed }/{ waveFiles.Count() })" );
                        }
                    } );
                }
                else
                {
                    Console.WriteLine( "waveファイルがありません。" );
                    return;
                }
            }
            // path = ファイルの場合
            else if ( File.Exists( path ) )
            {
                Console.WriteLine( "[wav -> mp3]" );

                (var input, var output) = CreateMediaFile( path );
                using ( var engine = new Engine() )
                    engine.Convert( input, output );

                Console.WriteLine( $"{ Path.GetFileName( path ) } を変換しました。" );
            }
            // あり得ないパス
            else
            {
                return;
            }

            // 入出力用のオブジェクトを生成する
            (MediaFile input, MediaFile output) CreateMediaFile( string inPath )
                => (input: new MediaFile { Filename = inPath },
                    output: new MediaFile { Filename = inPath.Replace( FILE_EXT_WAV, FILE_EXT_MP3 ) });
        }

        /// <summary>
        /// MP3ファイルからZipファイルに変換する
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="mp3Files"></param>
        private static void Mp3ToZip( string path )
        {
            // フォルダ・ファイルの共通処理
            var isDirectory = Directory.Exists( path );
            var isFile      = File.Exists(path);
            var outputZip   = string.Empty;
            if ( isDirectory || isFile )
            {
                Console.WriteLine( "[mp3 -> zip]" );

                // 既にzipファイルが存在する場合は削除する(圧縮時エラーになるため)
                outputZip = $"{ path }{ FILE_EXT_ZIP }";
                if ( File.Exists( outputZip ) ) File.Delete( outputZip );
            }

            // path = フォルダの場合
            if ( isDirectory )
            {
                var mp3Files = Directory.GetFiles( path )
                                        .Where( x => Path.GetExtension( x ).Equals( FILE_EXT_MP3 ) );

                if ( mp3Files.Count() > 0 )
                {
                    using ( var zip = ZipFile.Open( outputZip, ZipArchiveMode.Update, Encoding.GetEncoding( "shift_jis" ) ) )
                    {
                        // 圧縮に時間がかかるので並列処理する
                        var processed  = 0;
                        var lockObject = new object();
                        Parallel.ForEach( mp3Files, mp3 =>
                        {
                            lock ( lockObject )
                            {
                                zip.CreateEntryFromFile( mp3, Path.GetFileName( mp3 ), CompressionLevel.Optimal );

                                Console.WriteLine( $"{ Path.GetFileName( mp3 ) } を圧縮しました。" +
                                                   $"({ ++processed }/{ mp3Files.Count() })" );
                            }
                        } );
                    }
                }
                else
                {
                    Console.WriteLine( "mp3ファイルがありません。" );
                }
            }
            // path = ファイルの場合
            if ( isFile )
            {
                using ( var zip = ZipFile.Open( outputZip, ZipArchiveMode.Update, Encoding.GetEncoding( "shift_jis" ) ) )
                    zip.CreateEntryFromFile( path, Path.GetFileName( path ), CompressionLevel.Optimal );

                Console.WriteLine( $"{ Path.GetFileName( path ) } を圧縮しました。" );
            }

            // フォルダ・ファイルの共通処理
            Console.WriteLine( $"圧縮ファイルを{ outputZip }に出力しました。" );
        }

        /// <summary>
        /// 指定されたディレクトリから任意の種類のファイルを全て削除する
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fileType"></param>
        private static void DeleteSpecifiedTypeFiles( string path, string fileType )
        {
            // path = フォルダの場合
            if ( Directory.Exists( path ) )
            {
                var files = Directory.GetFiles( path ).Where( x => Path.GetExtension( x ).Equals( fileType ) );
                foreach ( var file in files )
                {
                    if ( File.Exists( file ) ) File.Delete( file );
                }
            }
            // path = ファイルの場合
            else if ( File.Exists( path ) )
            {
                var file = $"{ path.Split('.').First() }{ fileType }";
                File.Delete( file );
            }
            // あり得ないパス
            else
            {
                return;
            }
        }
    }
}