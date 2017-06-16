using System;
using System.IO;
using System.Reflection;
using static AudioToZip.FileTypeEnum;

namespace AudioToZip
{
    /// <summary>
    /// wav->mp3->zipに変換するプログラム
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// プログラムのエントリポイント
        /// </summary>
        private static void Main()
        {
            Console.WriteLine( $"#### {nameof( AudioToZip )} " +
                               $"Ver.{ Assembly.GetExecutingAssembly().GetName().Version } ####" );

            var args = Environment.GetCommandLineArgs();
            if ( args.Length < 2 )
            {
                Console.WriteLine( "ディレクトリを指定して下さい。" );
                goto Exit;
            }

            // WAVEファイルが格納されたディレクトリ/WAVEファイル
            var inputPath = args[1];

            // MP3を格納する一時フォルダ/MP3ファイル
            var tempDirectory = inputPath.GetDirectoryName() + $"_{ DateTime.Now.ToString( "yyyyMMdd_hhmmss_fff" ) }";

            // WAVE -> MP3変換
            var converter = new AudioConverter( Wave, Mp3, inputPath, tempDirectory );
            converter.AddObserver( new AudioConverterObserver() );
            converter.Convert();

            // MP3 -> ZIP圧縮
            var compressor = new ZipCompressor( Mp3, tempDirectory );
            compressor.AddObserver( new ZipCompressorObserver() );
            compressor.Convert();

            // 処理したファイルが存在した場合
            if ( ( converter.ProcessedCount > 0 ) && ( compressor.ProcessedCount > 0 ) )
            {
                // ZIPファイルが一時フォルダの名前になっているのでリネームする
                var oldName = tempDirectory + Zip.GetExtention( true );
                var newName = inputPath + Zip.GetExtention( true );
                oldName.Rename( newName );

                // 入力元のディレクトに生成した不要な一時フォルダ/ファイルを削除
                Directory.Delete( tempDirectory, true );

                // 最終出力結果を表示
                Console.WriteLine( $"\n>> 最終生成圧縮ファイル: \n>> { newName }\n" );
            }

            // 終了メッセージを表示
            Exit:
            Console.WriteLine( "処理が終了しました。何かキーを押して下さい..." );
            Console.ReadLine();
        }
    }
}