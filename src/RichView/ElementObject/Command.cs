namespace RichView.ElementObject
{
    /// <summary>
    /// 命令接口
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// 恢复
        /// </summary>
        void Redo();
        /// <summary>
        /// 撤销
        /// </summary>
        void Undo();
    }

    /// <summary>
    /// 恢复/撤销  的命令信息
    /// </summary>
    internal class ViewCommand : ICommand
    {
        private ViewInfo Old_View;
        private ViewInfo New_View;
       
        /// <summary>
        /// RichView
        /// </summary>
        private RichView ControlView;
        /// <summary>
        /// 原鼠标所在位置的Index
        /// </summary>
        private long Old_CursorIndex;
        /// <summary>
        /// 新的鼠标所在位置Index
        /// </summary>
        private long New_CursorIndex;

        /// <summary>
        /// 命令信息
        /// </summary>
        /// <param name="view"></param>
        /// <param name="oldView"></param>
        /// <param name="newView"></param>
        /// <param name="oldCurIndex"></param>
        /// <param name="newCurIndex"></param>
        public ViewCommand(RichView view, ViewInfo oldView, ViewInfo newView, long oldCurIndex, long newCurIndex)
        {
            ControlView = view;
            Old_View = oldView;
            New_View = newView;
            Old_CursorIndex = oldCurIndex;
            New_CursorIndex = newCurIndex;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Redo()
        {
            ControlView.ExecuteCommand(New_View, New_CursorIndex);
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            ControlView.ExecuteCommand(Old_View, Old_CursorIndex);
        }

    }
}
