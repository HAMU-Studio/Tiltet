namespace System
{
    public class ThrowawayMethod
    {
        // 初期値は偽
        bool flag;

        /// <summary>
        /// 一度だけ処理を行う
        /// </summary>
        /// <param name="action">処理内用 引数は渡せない</param>
        public void RunOnce(System.Action action)
        {
            if (!flag)
            {
                action();
                flag = true;
            }
        }
    }
}