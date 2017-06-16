using MediaToolkit;
using System.IO;
using System.Threading.Tasks;
using static AudioToZip.FileConverterStatusEnum;

namespace AudioToZip
{
    /// <summary>
    /// オーディオファイル変換クラス
    /// </summary>
    public class AudioConverter : FileConverter
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sourceFileType"></param>
        /// <param name="destinationFileType"></param>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        public AudioConverter( FileTypeEnum sourceFileType, FileTypeEnum destinationFileType, string inputPath, string outputPath = null )
            : base( sourceFileType, destinationFileType, inputPath, outputPath ) { }

        #endregion コンストラクタ

        #region 派生元のConvert()の各処理

        /// <summary>
        /// 変化前の前準備
        /// </summary>
        protected override void PreparationConvert()
        {
            // 出力先のディレクトリが存在しない場合は作成する
            if ( !Directory.Exists( OutputPath ) ) { Directory.CreateDirectory( OutputPath ); }
        }

        /// <summary>
        /// オーディオファイルを変換する(1つ)
        /// </summary>
        protected override void ConvertSingleFile()
        {
            (var input, var output) = PathUtility.CreateMediaFile( InputPath, OutputPath, SourceFileType, DestinationFileType );
            using ( var engine = new Engine() ) { engine.Convert( input, output ); }
            TargetFilePath = InputPath;
            Status = Running;
            Status = Complete;
        }

        /// <summary>
        /// オーディオファイルを変換する(複数)
        /// </summary>
        protected override void ConvertMultiFile()
        {
            var filePathes = InputPath.GetSpecifiedTypeFiles( SourceFileType );

            // 圧縮に時間がかかるので並列処理する
            var lockObject = new object();
            Parallel.ForEach( filePathes, filePath =>
            {
                (var input, var output) = PathUtility.CreateMediaFile( filePath, OutputPath, SourceFileType, DestinationFileType );
                lock ( lockObject )
                {
                    using ( var engine = new Engine() ) { engine.Convert( input, output ); }
                    TargetFilePath = filePath;
                    Status = Running;
                }
            } );
            Status = Complete;
        }

        #endregion 派生元のConvert()の各処理
    }
}