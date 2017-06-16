using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AudioToZip.Tests
{
    [TestClass()]
    public class PathUtilityTests
    {
        [TestMethod()]
        public void CreateMediaFileTest()
        {
            var sourceFile = @"D:\test1\sample1.wav";
            var outputPath = @"D:\test1\";
            var destFile = @"D:\test1\sample1.mp3";

            (var input, var output) = PathUtility.CreateMediaFile( sourceFile, outputPath, FileTypeEnum.Wave, FileTypeEnum.Mp3 );
            input.Filename.Is( sourceFile );
            output.Filename.Is( destFile );
        }

        [TestMethod()]
        public void GetSpecifiedTypeFilesTest()
        {
            var dir = @"D:\test1";

            var waves = dir.GetSpecifiedTypeFiles( FileTypeEnum.Wave );
            waves.Is( new[] { $@"{ dir }\sample1.wav", $@"{ dir }\sample2.wav" } );

            var mp3s = dir.GetSpecifiedTypeFiles( FileTypeEnum.Mp3 );
            mp3s.Is( new[] { $@"{ dir }\sample1.mp3", $@"{ dir }\sample2.mp3", $@"{ dir }\sample3.mp3" } );

            var zips = dir.GetSpecifiedTypeFiles( FileTypeEnum.Zip );
            zips.Is( new[] { $@"{ dir }\sample1.zip", $@"{ dir }\sample2.zip", $@"{ dir }\sample3.zip", $@"{ dir }\sample4.zip" } );

            var all1 = dir.GetSpecifiedTypeFiles( FileTypeEnum.All );
            var all2 = waves.Union( mp3s ).Union( zips ).OrderBy( _ => _ );
            all1.Is( all2 );

            var file = $@"{ dir }\sample1.wav".GetSpecifiedTypeFiles( FileTypeEnum.All );
            file.Is( new List<string>() );
        }

        [TestMethod()]
        public void FileTypeEqualsTest()
        {
            var dir = @"D:\test1";
            var wave = $@"{ dir }\sample1.wav";

            wave.FileTypeEquals( FileTypeEnum.Wave ).Is( true );
            wave.FileTypeEquals( FileTypeEnum.Mp3 ).Is( false );
        }

        [TestMethod()]
        public void GetDirectoryNameTest()
        {
            var dir = @"D:\test1";
            dir.GetDirectoryName().Is( dir );

            var file = $@"{dir}\sample1.wav";
            file.GetDirectoryName().Is( dir );

            "xxx".GetDirectoryName().Is( string.Empty );
        }

        [TestMethod()]
        public void RenameTest_リネーム先のディレクトリが存在しない場合()
        {
            // テストの前準備
            var dirA = @"D:\testA";
            var dirB = @"D:\testB";

            if ( !Directory.Exists( dirA ) )
                Directory.CreateDirectory( dirA );

            if ( Directory.Exists( dirB ) )
                Directory.Delete( dirB, true );

            // テスト
            dirA.Rename( dirB );

            // 結果チェック
            Directory.Exists( dirA ).Is( false );
            Directory.Exists( dirB ).Is( true );
        }

        [TestMethod()]
        public void RenameTest_リネーム先のディレクトリが存在する場合()
        {
            // テストの前準備
            var dirA = @"D:\testA";
            var dirB = @"D:\testB";

            if ( !Directory.Exists( dirA ) )
                Directory.CreateDirectory( dirA );

            if ( !Directory.Exists( dirB ) )
                Directory.CreateDirectory( dirB );

            // テスト
            dirA.Rename( dirB );

            // 結果チェック
            Directory.Exists( dirA ).Is( false );
            Directory.Exists( dirB ).Is( true );
        }

        [TestMethod()]
        public void RenameTest_リネーム元のディレクトリが存在しない場合()
        {
            // テストの前準備
            var dirC = @"D:\testC";
            var dirD = @"D:\testD";

            if ( Directory.Exists( dirC ) )
                Directory.Delete( dirC );

            if ( !Directory.Exists( dirD ) )
                Directory.CreateDirectory( dirD );

            // テスト
            AssertEx.Throws<IOException>( () => dirC.Rename( dirD ) );
        }

        [TestMethod()]
        public void RenameTest_リネーム先のファイルが存在しない場合()
        {
            // テストの前準備
            var fileA = @"D:\test\sampleA.txt";
            var fileB = @"D:\test\sampleB.txt";

            if ( !File.Exists( fileA ) )
                File.Create( fileA ).Close();

            if ( File.Exists( fileB ) )
                File.Delete( fileB );

            // テスト
            fileA.Rename( fileB );

            // 結果チェック
            File.Exists( fileA ).Is( false );
            File.Exists( fileB ).Is( true );
        }

        [TestMethod()]
        public void RenameTest_リネーム先のファイルが存在する場合()
        {
            // テストの前準備
            var fileA = @"D:\test\sampleA.txt";
            var fileB = @"D:\test\sampleB.txt";

            if ( !File.Exists( fileA ) )
                File.Create( fileA ).Close();

            if ( !File.Exists( fileB ) )
                File.Create( fileB ).Close();

            // テスト
            fileA.Rename( fileB );

            // 結果チェック
            File.Exists( fileA ).Is( false );
            File.Exists( fileB ).Is( true );
        }

        [TestMethod()]
        public void RenameTest_リネーム元のファイルが存在しない場合()
        {
            // テストの前準備
            var fileC = @"D:\test\sampleC.txt";
            var fileD = @"D:\test\sampleD.txt";

            if ( File.Exists( fileC ) )
                File.Delete( fileC );

            if ( !File.Exists( fileD ) )
                File.Create( fileD ).Close();

            // テスト
            AssertEx.Throws<IOException>( () => fileC.Rename( fileD ) );
        }
    }
}