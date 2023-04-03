using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BookMaintain.Models
{
    public class BookService
    {
        //取得連線字串
        private string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        }

        //依搜尋條件取得所需的書籍資料
        public List<Book> GetBookDataByCondition(Book book)
        {
            DataTable dt = new DataTable();
            string sql = @"
                            SELECT data.BOOK_ID AS BookId,
                                data.BOOK_NAME AS BookName,
                                class.BOOK_CLASS_NAME AS BookClassName,
                                CONVERT(VARCHAR(10),data.BOOK_BOUGHT_DATE,111) AS BookBoughtDate,
                                data.BOOK_STATUS AS BookStatusId,
                                code.CODE_NAME AS BookStatusName,
                                member.USER_ENAME AS BookKeeperEName
                            FROM BOOK_DATA data
                            INNER JOIN BOOK_CLASS class
                                ON data.BOOK_CLASS_ID = class.BOOK_CLASS_ID
                            INNER JOIN BOOK_CODE code
                                ON data.BOOK_STATUS = code.CODE_ID
                                    AND code.CODE_TYPE='BOOK_STATUS'
                            LEFT JOIN MEMBER_M member
                                ON data.BOOK_KEEPER = member.USER_ID
                            WHERE (UPPER(data.BOOK_NAME) LIKE UPPER ('%' + @BookName + '%') OR @BookName = '')
                                AND (data.BOOK_CLASS_ID = @BookClassId OR @BookClassId = '')
                                AND (data.BOOK_KEEPER = @BookKeeperId OR @BookKeeperId = '')
                                AND (data.BOOK_STATUS = @BookStatusId OR @BookStatusId = '')
                            ORDER BY BookBoughtDate DESC, BookName";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", book.BookName ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", book.BookClassId ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@BookKeeperId", book.BookKeeperId ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", book.BookStatusId ?? string.Empty));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookDataToList(dt);
        }

        //把資料map到list中
        private List<Book> MapBookDataToList(DataTable book)
        {
            List<Book> result = new List<Book>();
            foreach (DataRow row in book.Rows)
            {
                result.Add(new Book()
                {
                    BookId = (int)row["BookId"],
                    BookName = row["BookName"].ToString(),
                    BookClassName = row["BookClassName"].ToString(),
                    BookBoughtDate = row["BookBoughtDate"].ToString(),
                    BookStatusId = row["BookStatusId"].ToString(),
                    BookStatusName = row["BookStatusName"].ToString(),
                    BookKeeperEName = row["BookKeeperEName"].ToString()
                });
            }
            return result;
        }

        //新增書籍
        public void InsertBook(Book book)
        {
            string sql = @"
                            INSERT INTO BOOK_DATA
                            ( BOOK_NAME,    BOOK_CLASS_ID,    BOOK_AUTHOR,    BOOK_BOUGHT_DATE,    BOOK_PUBLISHER,
                              BOOK_NOTE,    BOOK_STATUS,    BOOK_KEEPER,    CREATE_DATE,    CREATE_USER,
                              MODIFY_DATE,    MODIFY_USER )
                            VALUES
                            ( @BookName,    @BookClassId,    @BookAuthor,    @BookBoughtDate,    @BookPublisher,
                              @BookNote,    @BookStatusId,    @BookKeeperId,    GETDATE(),    '3139',
                              GETDATE(),    '3139' )";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", Uri.UnescapeDataString(book.BookName)));//特殊符號解碼
                cmd.Parameters.Add(new SqlParameter("@BookClassId", book.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@BookAuthor", Uri.UnescapeDataString(book.BookAuthor)));
                cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", book.BookBoughtDate));
                cmd.Parameters.Add(new SqlParameter("@BookPublisher", Uri.UnescapeDataString(book.BookPublisher)));
                cmd.Parameters.Add(new SqlParameter("@BookNote", Uri.UnescapeDataString(book.BookNote)));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", Variable.Borrowable));
                cmd.Parameters.Add(new SqlParameter("@BookKeeperId", string.Empty));
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        //取得書籍的詳細資料
        public Book GetBookDetail(string bookId)
        {
            DataTable dt = new DataTable();
            string sql = @"
                            SELECT data.BOOK_ID AS BookId,
                                data.BOOK_NAME AS BookName,
                                data.BOOK_AUTHOR AS BookAuthor,
                                data.BOOK_PUBLISHER AS BookPublisher,
                                data.BOOK_NOTE AS BookNote,
                                CONVERT(VARCHAR(10),data.BOOK_BOUGHT_DATE,111) AS BookBoughtDate,
                                data.BOOK_CLASS_ID AS BookClassId,
                                class.BOOK_CLASS_NAME AS BookClassName,
                                data.BOOK_STATUS AS BookStatusId,
                                code.CODE_NAME AS BookStatusName,
                                data.BOOK_KEEPER AS BookKeeperId,
                                member.USER_ENAME AS BookKeeperEName,
                                CONVERT(VARCHAR(10),data.CREATE_DATE,111) AS CreateDate,
                                CONVERT(VARCHAR(10),data.MODIFY_DATE,111) AS ModifyDate
                           FROM BOOK_DATA data
                            INNER JOIN BOOK_CLASS class
                                ON data.BOOK_CLASS_ID = class.BOOK_CLASS_ID
                            INNER JOIN BOOK_CODE code
                                ON data.BOOK_STATUS = code.CODE_ID
                                    AND code.CODE_TYPE='BOOK_STATUS'
                            LEFT JOIN MEMBER_M member
                                ON data.BOOK_KEEPER = member.USER_ID
                           WHERE data.BOOK_ID = @BookId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", bookId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookDetailToList(dt);
        }

        //把Detail的資料map到list中
        private Book MapBookDetailToList(DataTable book)
        {
            Book result = new Book();
            foreach (DataRow row in book.Rows)
            {

                result.BookId = (int)row["BookId"];
                result.BookName = row["BookName"].ToString();
                result.BookAuthor = row["BookAuthor"].ToString();
                result.BookPublisher = row["BookPublisher"].ToString();
                result.BookNote = row["BookNote"].ToString();
                result.BookBoughtDate = row["BookBoughtDate"].ToString();
                result.BookClassId = row["BookClassId"].ToString();
                result.BookClassName = row["BookClassName"].ToString();
                result.BookStatusId = row["BookStatusId"].ToString();
                result.BookStatusName = row["BookStatusName"].ToString();
                result.BookKeeperId = row["BookKeeperId"].ToString();
                result.BookKeeperEName = row["BookKeeperEName"].ToString();
                result.CreateDate = row["CreateDate"].ToString();
                result.ModifyDate = row["ModifyDate"].ToString();

            }
            return result;
        }

        public string GetBookStatusId(string bookId)
        {
            string sql = @"SELECT BOOK_STATUS 
                           FROM BOOK_DATA 
                           WHERE BOOK_ID = @BookId";
            string bookStatusId;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", bookId));
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                try
                {
                    bookStatusId = (string)cmd.ExecuteScalar();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
            return bookStatusId;
        }

        //刪除書籍
        public bool DeleteBook(string bookId)
        {
            bool deleteSuccess = false;
            string bookStatus = GetBookStatusId(bookId);
            if (bookStatus == Variable.Borrowable || bookStatus == Variable.CannotBorrow)
            {
                string sql = @"DELETE FROM BOOK_DATA WHERE BOOK_ID = @BookId";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@BookId", bookId));
                    SqlTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tran.Commit();
                        deleteSuccess = true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return deleteSuccess;
        }

        //更新書籍
        public void UpdateBook(Book book)
        {
            string sql = @"UPDATE BOOK_DATA
                           SET BOOK_NAME = @BookName,
                               BOOK_AUTHOR = @BookAuthor,
                               BOOK_PUBLISHER = @BookPublisher,
                               BOOK_NOTE = @BookNote,
                               BOOK_BOUGHT_DATE = @BookBoughtDate,
                               BOOK_CLASS_ID = @BookClassId,
                               BOOK_STATUS = @BookStatusId,
                               BOOK_KEEPER = @BookKeeperId,
                               MODIFY_DATE = GETDATE()
                           WHERE BOOK_ID = @BookId";

            if (book.BookStatusId.Equals(Variable.Borrowed))
            {
                sql += @" INSERT INTO BOOK_LEND_RECORD 
                        (BOOK_ID, KEEPER_ID, LEND_DATE, CRE_DATE, CRE_USR, MOD_DATE, MOD_USR )
                        VALUES
                        (@BookId, @BookKeeperId, GETDATE(), GETDATE(), '3139', GETDATE(), '3139')";
            }
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", Uri.UnescapeDataString(book.BookName)));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", book.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@BookAuthor", Uri.UnescapeDataString(book.BookAuthor)));
                cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", book.BookBoughtDate));
                cmd.Parameters.Add(new SqlParameter("@BookPublisher", Uri.UnescapeDataString(book.BookPublisher)));
                cmd.Parameters.Add(new SqlParameter("@BookNote", Uri.UnescapeDataString(book.BookNote)));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", book.BookStatusId));
                cmd.Parameters.Add(new SqlParameter("@BookKeeperId", book.BookKeeperId ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@BookId", book.BookId));
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        //為了自動完成書名 取得所有書籍名稱
        public List<string> GetBookName()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT DISTINCT BOOK_NAME FROM BOOK_DATA";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sql, conn);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            List<string> result = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(row["BOOK_NAME"].ToString());
            }
            return result;
        }

        //取得書籍的借閱紀錄
        public List<Book> LendRecord(string bookId)
        {
            DataTable dt = new DataTable();
            string sql = @"
                            SELECT CONVERT(VARCHAR(10),LEND_DATE,111) AS LendDate,
                                LEND_DATE,
                                KEEPER_ID AS BookKeeperId,
                                member.USER_CNAME AS BookKeeperCName,
                                member.USER_ENAME AS BookKeeperEName
                            FROM BOOK_LEND_RECORD lend
                                INNER JOIN BOOK_DATA data
                                    ON lend.BOOK_ID = data.BOOK_ID                         
                                INNER JOIN MEMBER_M member
                                    ON lend.KEEPER_ID = member.USER_ID
                            WHERE data.BOOK_ID = @BookId
                            ORDER BY LEND_DATE DESC";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", bookId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapLendRecordToList(dt);
        }

        //把LendRecord的資料map到list中
        private List<Book> MapLendRecordToList(DataTable book)
        {
            List<Book> result = new List<Book>();
            foreach (DataRow row in book.Rows)
            {
                result.Add(new Book()
                {
                    LendDate = row["LendDate"].ToString(),
                    BookKeeperId = row["BookKeeperId"].ToString(),
                    BookKeeperEName = row["BookKeeperEName"].ToString(),
                    BookKeeperCName = row["BookKeeperCName"].ToString()
                });
            }
            return result;
        }

    }
}