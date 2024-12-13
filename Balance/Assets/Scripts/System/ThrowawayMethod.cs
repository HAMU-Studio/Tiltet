namespace System
{
    public class ThrowawayMethod
    {
        // 真偽値の初期値は偽
        bool flag;

        /// <summary>
        /// 一度だけ処理を行う
        /// </summary>
        /// <param name="action">処理内容</param>
        public void RunOnce(System.Action action)
        {
            // 真偽値が偽である場合
            if (!flag)
            {
                // 処理を行う
                action();
                // 真偽値を真に切り替える
                flag = true;
            }
        }
    }
}