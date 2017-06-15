using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using static AudioToZip.FileConverterStatusEnum;

namespace AudioToZip
{
    /// <summary>
    /// ZIPファイル変換クラス
    /// </summary>
    internal class ZipCompressor : FileConverter
    {
        #region 公開クラス

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sourceFileType"></param>
        /// <param name="inputPath"></param>
        public ZipCompressor( FileTypeEnum sourceFileType, string inputPath )
            : base( sourceFileType, FileTypeEnum.Zip, inputPath, ( inputPath + FileTypeEnum.Zip.GetExtention( true ) ) ) { }

        #endregion 公開クラス

        #region 派生元のConvert()の各処理

        /// <summary>
        /// 圧縮前の前準備
        /// </summary>
        protected override void PreparationConvert()
        {
            // 既にzipファイルが存在する場合、そのまま圧縮処理を行うと
            // zipファイルの中に同じファイルが重複して追加されるため削除する
            if ( File.Exists( OutputPath ) ) File.Delete( OutputPath );
        }

        /// <summary>
        /// ZIPファイルに変換する(1つ)
        /// </summary>
        /// <param name="filePath"></param>
        protected override void ConvertSingleFile()
        {
            using ( var zip = ZipFile.Open( OutputPath, ZipArchiveMode.Update, Encoding.GetEncoding( "shift_jis" ) ) )
            {
                zip.CreateEntryFromFile( InputPath, Path.GetFileName( InputPath ), CompressionLevel.Optimal );
            }
            TargetFilePath = InputPath;
            Status = Running;
            Status = Complete;
        }

        /// <summary>
        /// ZIPファイルに変換する(複数)
        /// </summary>
        /// <param name="filePathes"></param>
        protected override void ConvertMultiFile()
        {
            using ( var zip = ZipFile.Open( OutputPath, ZipArchiveMode.Update, Encoding.GetEncoding( "shift_jis" ) ) )
            {
                var filePathes = PathUtility.GetSpecifiedTypeFiles( InputPath, SourceFileType );

                // 圧縮に時間がかかるので並列処理する
                var lockObject = new object();
                Parallel.ForEach( filePathes, filePath =>
                {
                    lock ( lockObject )
                    {
                        zip.CreateEntryFromFile( filePath, Path.GetFileName( filePath ), CompressionLevel.Optimal );
                        TargetFilePath = filePath;
                        Status = Running;
                    }
                } );
                Status = Complete;
            }
        }

        #endregion 派生元のConvert()の各処理
    }
}