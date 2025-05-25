namespace System.FadeSystem
{
    /// <summary>
    /// インターフェースは契約みたいなもの。"フェードアウトの開始処理"と"完了を確認するフラグ"を作ることを確約させることで
    /// それがある前提の処理が継承先で書ける。
    /// 差し替えや機能拡張が楽になる
    /// </summary>
    public interface IFadeHandler
    {
        void StartFadeOut();        // フェードアウトを開始する
        bool IsFadeOutComplete();   // フェードアウトが完了したかを確認する
    }
}