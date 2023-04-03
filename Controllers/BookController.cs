using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BookMaintain.Models;

namespace BookMaintain.Controllers
{
    public class BookController : Controller
    {
        private readonly CodeService _codeService = new CodeService();
        private readonly BookService _bookService = new BookService();

        //查詢書籍
        [HttpGet()]
        public ActionResult SearchBook()
        {
            return View();
        }

        //查詢書籍
        [HttpPost()]
        public JsonResult SearchBook(Book book)
        {
            List<Book> searchResult = _bookService.GetBookDataByCondition(book);
            return Json(searchResult);
        }

        //新增書籍
        [HttpGet()]
        public ActionResult InsertBook()
        {
            return View();
        }

        //新增書籍
        [HttpPost()]
        public JsonResult InsertBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return Json(false);
            }

            try
            {
                _bookService.InsertBook(book);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        //書籍詳細資料
        [HttpGet()]
        public ActionResult DetailBook()
        {
            return View();
        }

        //書籍詳細資料
        [HttpPost()] 
        public JsonResult DetailBook(string bookId)
        {
            Book bookDetail = _bookService.GetBookDetail(bookId);
            bookDetail.BookNote = bookDetail.BookNote?.Replace("<BR>", "\r\n");
            return Json(bookDetail);
        }

        //刪除書籍
        [HttpPost()]
        public JsonResult DeleteBook(string bookId)
        {
            try
            {
                return Json(_bookService.DeleteBook(bookId));
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        //確認書籍狀態
        [HttpPost()]
        public JsonResult CheckBookStatus(string bookId)
        {
            string bookStatusId = _bookService.GetBookStatusId(bookId);

            if (bookStatusId == Variable.Borrowable || bookStatusId == Variable.CannotBorrow)
            {
                return Json(true);
            }

            if (bookStatusId == Variable.Borrowed || bookStatusId == Variable.BorrowedNotReceived)
            {
                return Json(false);
            }

            return Json("此書已不存在");
        }

        //修改書籍
        [HttpGet()]
        public ActionResult UpdateBook()
        {
            return View();
        }

        //修改書籍
        [HttpPost()]
        public JsonResult UpdateBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return Json(false);
            }

            if (((book.BookStatusId == Variable.Borrowed || book.BookStatusId == Variable.BorrowedNotReceived) && (book.BookKeeperId != null)) ||
                ((book.BookStatusId == Variable.Borrowable || book.BookStatusId == Variable.CannotBorrow) && (book.BookKeeperId == null)))
            {
                _bookService.UpdateBook(book);
                return Json(true);
            }

            return Json(false);
        }

        //取得下拉式選單資料
        [HttpPost()]
        public JsonResult GetDropDownListData(string category)
        {
            List<SelectListItem> listResult = null;
            switch (category)
            {
                case "BookClassId":
                    listResult = _codeService.GetBookClass();
                    break;
                case "BookKeeperId":
                    listResult = _codeService.GetBookKeeper();
                    break;
                case "BookStatusId":
                    listResult = _codeService.GetBookStatus();
                    break;
            }
            return Json(listResult);
        }

        //自動完成書名
        [HttpPost()]
        public JsonResult AutoCompleteBookName()
        {
            return Json(_bookService.GetBookName());
        }

        //書籍借閱紀錄
        [HttpPost()]
        public JsonResult LendRecord(string bookId)
        {
            List<Book> lendRecord = _bookService.LendRecord(bookId);
            return Json(lendRecord);
        }
    }
}