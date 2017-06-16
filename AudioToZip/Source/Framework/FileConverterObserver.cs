using System;
using System.Collections.Generic;
using static AudioToZip.FileConverterStatusEnum;

namespace AudioToZip
{
    /// <summary>
    /// ファイル変換系クラスの監視クラス(抽象)
    /// (Observerパターンの監視する側)
    /// </summary>
    public abstract class FileConverterObserver : Observer
    {
        #region フィールド

        /// <summary>
        /// コンソールに表示するメッセージのテーブル
        /// </summary>
        private readonly Dictionary<FileConverterStatusEnum, Func<FileConverter, string>> _messageTable;

        #endregion フィールド

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected FileConverterObserver()
        {
            _messageTable = new Dictionary<FileConverterStatusEnum, Func<FileConverter, string>>
            {
                // FileConverterの状態   // 表示メッセージ取得関数
                { Wait,                 GetWaitMessage      },
                { Start,                GetStartMessage     },
                { Running,              GetRunningMessage   },
                { Complete,             GetCompleteMessage  },
                { Failed,               GetFailedMessage    },
            };
        }

        #endregion コンストラクタ

        #region Observerの実装

        /// <summary>
        /// 監視対象オブジェクトの状態変化時に呼び出され、そのオブジェクトの状態を表示する
        /// </summary>
        /// <param name="converter"></param>
        public override void Update( IObservable subject )
        {
            var converter = subject as FileConverter ?? throw new InvalidProgramException();

            var getMessgeFunc = _messageTable[converter.Status] ?? throw new NotImplementedException();
            Console.WriteLine( getMessgeFunc( converter ) );
        }

        #endregion Observerの実装

        #region メッセージ取得系関数

        // 具体的な表示メッセージは派生先で定義する

        /// <summary>
        /// 監視対象クラスが待機している場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected abstract string GetWaitMessage( FileConverter converter );

        /// <summary>
        /// 監視対象クラスが処理を開始した場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected abstract string GetStartMessage( FileConverter converter );

        /// <summary>
        /// 監視対象クラスが処理中の場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected abstract string GetRunningMessage( FileConverter converter );

        /// <summary>
        /// 監視対象クラスが処理完了した場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected abstract string GetCompleteMessage( FileConverter converter );

        /// <summary>
        /// 監視対象クラスが処理失敗した場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected abstract string GetFailedMessage( FileConverter converter );

        #endregion メッセージ取得系関数
    }
}