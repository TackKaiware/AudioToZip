using MediaToolkit.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AudioToZip
{
    /// <summary>
    /// ファイルパスユーティリティクラス
    /// </summary>
    internal static class PathUtility
    {
        /// <summary>
        /// 入出力用のMediaFileオブジェクトを生成する
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <param name="sourceFileType"></param>
        /// <param name="destinationFileType"></param>
        /// <returns></returns>
        public static (MediaFile input, MediaFile output) CreateMediaFile( string inputPath,
                                                                               string outputPath,
                                                                               FileTypeEnum sourceFileType,
                                                                               FileTypeEnum destinationFileType )
            => (input: new MediaFile { Filename = inputPath },
                output: new MediaFile
                {
                    Filename = Path.Combine( outputPath,
                               Path.GetFileName( inputPath.Replace( sourceFileType.GetExtention( true ),
                                                                    destinationFileType.GetExtention( true ) ) ) )
                });

        /// <summary>
        /// 指定された種類のファイルのみ取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSpecifiedTypeFiles( string path, FileTypeEnum fileType )
        {
            if ( Directory.Exists( path ) )
            {
                var allFiles = Directory.GetFiles( path );

                if ( fileType.Equals( FileTypeEnum.All ) )
                    return allFiles;
                else
                    return allFiles.Where( x => Path.GetExtension( x ).Equals( fileType.GetExtention( true ) ) );
            }
            else
            {
                if ( Path.GetExtension( path ).Equals( fileType.GetExtention( true ) ) )
                    return new List<string>() { path };
                else
                    return new List<string>();
            }
        }

        /// <summary>
        /// pathがディレクトリの場合->そのままpathを返す
        /// pathがファイルの場合->ファイルが格納されているディレクトリのパスを返す
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryName( string path )
        {
            if ( Directory.Exists( path ) ) return path;
            else /* if ( File.Exists( path ) ) */ return Path.GetDirectoryName( path );
        }

        /// <summary>
        /// ディレクトリまたはファイルをリネームする
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public static void Rename( string oldName, string newName )
        {
            if ( Directory.Exists( oldName ) )
            {
                // リネーム先のフォルダが既に存在している場合は削除する。(例外発生の回避のため)
                if ( Directory.Exists( newName ) )
                {
                    Directory.Delete( newName, true );
                }

                Directory.CreateDirectory( newName );
                Directory.Move( oldName, newName );
                Directory.Delete( oldName, true );
            }
            else
            {
                // TODO:未テストのパス
                if ( File.Exists( newName ) )
                {
                    File.Delete( newName );
                }
                File.Move( oldName, newName );
                File.Delete( oldName );
            }
        }
    }
}