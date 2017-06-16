using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AudioToZip.FileConverterStatusEnum;

namespace AudioToZip
{
    /// <summary>
    /// ファイル変換系クラス(抽象)
    /// (Observerパターンの監視される側)
    /// </summary>
    public abstract class FileConverter : IObservable
    {
        #region フィールド

        /// <summary>
        /// このクラスの状態変化を監視するObserverのリスト
        /// </summary>
        private readonly List<FileConverterObserver> _observers = new List<FileConverterObserver>();

        #endregion フィールド

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sourceFileType"></param>
        /// <param name="destinationFileType"></param
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        public FileConverter( FileTypeEnum sourceFileType, FileTypeEnum destinationFileType, string inputPath, string outputPath = null )
        {
            // 引数チェック
            if ( sourceFileType.Equals( destinationFileType ) )
                throw new ArgumentException( $"{ nameof( sourceFileType ) }と{ nameof( destinationFileType ) }が同じです。" );

            if ( inputPath.IndexOfAny( Path.GetInvalidPathChars() ) > 0 )
                throw new ArgumentException( "存在しないファイルパスです。", $"{ nameof( inputPath ) } = { inputPath }" );

            if ( ( outputPath != null ) && ( outputPath.IndexOfAny( Path.GetInvalidPathChars() ) > 0 ) )
                throw new ArgumentException( "存在しないファイルパスです。", $"{ nameof( outputPath ) } = { outputPath }" );

            // プロパティの設定
            SourceFileType = sourceFileType;
            DestinationFileType = destinationFileType;
            InputPath = inputPath;
            OutputPath = outputPath ?? inputPath.GetDirectoryName();

            if ( Directory.Exists( inputPath ) )
            {
                var files = Directory.GetFiles( InputPath );
                if ( sourceFileType.Equals( FileTypeEnum.All ) )
                    TotalCount = files.Count();
                else
                    TotalCount = files.Where( x => x.FileTypeEquals( sourceFileType ) ).Count();
            }
            else
            {
                TotalCount = inputPath.FileTypeEquals( sourceFileType ) ? 1 : 0;
            }
        }

        #endregion コンストラクタ

        #region 公開プロパティ

        private FileConverterStatusEnum _status = Wait;

        /// <summary>
        /// 変換処理の状態(設定時、ProcessedCountを更新時し、NotifyObserversを呼び出す)
        /// </summary>
        public FileConverterStatusEnum Status
        {
            get => _status;
            set
            {
                _status = value;
                if ( _status.Equals( Start ) ) ProcessedCount = 0;
                if ( _status.Equals( Running ) ) ProcessedCount++;
                NotifyObservers();
            }
        }

        /// <summary>
        /// 変換元ファイルの種類
        /// </summary>
        public FileTypeEnum SourceFileType { get; }

        /// <summary>
        /// 変換先ファイルの種類
        /// </summary>
        public FileTypeEnum DestinationFileType { get; }

        /// <summary>
        /// 入力元パス
        /// </summary>
        public string InputPath { get; }

        /// <summary>
        /// 出力先パス
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// 変換対象のファイルパス
        /// </summary>
        public string TargetFilePath { get; protected set; }

        /// <summary>
        /// 処理済みファイルの数
        /// </summary>
        public int ProcessedCount { get; private set; } = 0;

        /// <summary>
        /// 変換対象ファイルの総数
        /// </summary>
        public int TotalCount { get; }

        #endregion 公開プロパティ

        #region TemplateMethodパターン

        /// <summary>
        /// ファイルを変換する
        /// </summary>
        /// <param name="fileName"></param>
        public void Convert()
        {
            try
            {
                if ( TotalCount > 0 )
                {
                    Status = Start;

                    // 変換処理の前準備
                    PreparationConvert();

                    if ( Directory.Exists( InputPath ) )
                        // 入力元パスがディレクトリの場合 -> 複数のファイルを変換する
                        ConvertMultiFile();
                    else
                        // 入力元パスがファイルの場合 -> 1つのファイルを変換する
                        ConvertSingleFile();
                }
                else
                {
                    Status = Wait;
                }
            }
            catch ( InvalidOperationException )
            {
                Status = Failed;
                Environment.Exit( -1 );
            }
        }

        // 具体的な処理は派生先で定義する

        /// <summary>
        /// 変換前の前準備
        /// </summary>
        /// <param name="outputPath"></param>
        protected abstract void PreparationConvert();

        /// <summary>
        /// 1つのファイルに対する変換処理
        /// </summary>
        /// <param name="filePath"></param>
        protected abstract void ConvertSingleFile();

        /// <summary>
        /// 複数のファイルに対する変換処理
        /// </summary>
        /// <param name="filePathes"></param>
        protected abstract void ConvertMultiFile();

        #endregion TemplateMethodパターン

        #region IObservableの実装

        /// <summary>
        /// 監視クラスを追加する
        /// </summary>
        /// <param name="observer"></param>
        public void AddObserver( FileConverterObserver observer ) => _observers.Add( observer );

        /// <summary>
        /// 監視クラスを削除する
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver( FileConverterObserver observer ) => _observers.Remove( observer );

        /// <summary>
        /// 監視クラスに状態変化を通知する
        /// </summary>
        /// <param name="status"></param>
        public void NotifyObservers()
        {
            foreach ( var observer in _observers )
            {
                observer.Update( this );
            }
        }

        #endregion IObservableの実装
    }
}