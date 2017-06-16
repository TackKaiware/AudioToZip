using MediaToolkit.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AudioToZip
{
    /// <summary>
    /// ファイルパスユーティリティクラス
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// 入出力用のMediaFileオブジェクトを生成する
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <param name="sourceFileType"></param>
        /// <param name="destinationFileType"></param>
        /// <returns></returns>
        public static (MediaFile input, MediaFile output) CreateMediaFile( string sourceFile,
                                                                               string outputPath,
                                                                               FileTypeEnum sourceFileType,
                                                                               FileTypeEnum destinationFileType )
            => (input: new MediaFile { Filename = sourceFile },
                output: new MediaFile
                {
                    Filename = Path.Combine( outputPath,
                               Path.GetFileName( sourceFile.Replace( sourceFileType.GetExtention( true ),
                                                                     destinationFileType.GetExtention( true ) ) ) )
                });

        /// <summary>
        /// 指定された種類のファイルのみ取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSpecifiedTypeFiles( this string path, FileTypeEnum fileType )
        {
            if ( Directory.Exists( path ) )
            {
                return fileType.Equals( FileTypeEnum.All )
                        ? Directory.GetFiles( path )
                        : Directory.GetFiles( path ).Where( x => x.FileTypeEquals( fileType ) );
            }

            return new List<string>();
        }

        /// <summary>
        /// 指定したファイルタイプか調べる
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool FileTypeEquals( this string path, FileTypeEnum fileType )
            => Path.GetExtension( path ).Equals( fileType.GetExtention( true ) );

        /// <summary>
        /// pathがディレクトリの場合->そのままpathを返す
        /// pathがファイルの場合->ファイルが格納されているディレクトリのパスを返す
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryName( this string path )
        {
            if ( Directory.Exists( path ) )
                return path;

            if ( File.Exists( path ) )
                return Path.GetDirectoryName( path );

            return string.Empty;
        }

        /// <summary>
        /// ディレクトリまたはファイルをリネームする
        /// リネーム元のファイルが存在しない場合は例外(IOException)が発生する
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public static void Rename( this string oldName, string newName )
        {
            if ( Directory.Exists( oldName ) )
            {
                // リネーム先のフォルダが既に存在している場合は削除する。(例外発生の回避のため)
                if ( Directory.Exists( newName ) )
                    Directory.Delete( newName, true );

                Directory.Move( oldName, newName );
            }
            else if ( File.Exists( oldName ) )
            {
                // リネーム先のファイルが既に存在している場合は削除する。(例外発生の回避のため)
                if ( File.Exists( newName ) )
                    File.Delete( newName );

                File.Move( oldName, newName );
            }
            else
            {
                // リネーム元のディレクトまたはファイルが存在しない場合は例外発生
                throw new IOException();
            }
        }
    }
}