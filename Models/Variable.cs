namespace BookMaintain.Models
{
    public static class Variable
    {
        /// <summary>
        /// A-可以借閱
        /// </summary>
        public static string Borrowable => "A";

        /// <summary>
        /// U-不可借閱 沒有借閱人
        /// </summary>
        public static string CannotBorrow => "U";

        /// <summary>
        /// B-已借閱
        /// </summary>
        public static string Borrowed => "B";

        /// <summary>
        /// C-已借未領 有借閱人
        /// </summary>
        public static string BorrowedNotReceived => "C";
    }
}