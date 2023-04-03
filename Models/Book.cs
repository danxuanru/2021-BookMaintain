using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookMaintain.Models
{
    public class Book
    {
        //書的流水號
        public int BookId { get; set; }

        //書名
        [DisplayName("書名")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookName { get; set; }

        //作者
        [DisplayName("作者")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookAuthor { get; set; }

        //購書日期
        [DisplayName("購書日期")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookBoughtDate { get; set; }

        //出版商
        [DisplayName("出版商")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookPublisher { get; set; }

        //內容簡介
        [DisplayName("內容簡介")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookNote { get; set; }

        //圖書類別代號
        [DisplayName("圖書類別")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookClassId { get; set; }

        //圖書類別名
        [DisplayName("圖書類別")]
        public string BookClassName { get; set; }

        //借閱人代號
        [DisplayName("借閱人")]
        public string BookKeeperId { get; set; }

        //中文姓名
        [DisplayName("借閱人")]
        public string BookKeeperCName { get; set; }

        //英文姓名
        [DisplayName("借閱人")]
        public string BookKeeperEName { get; set; }

        //全名
        [DisplayName("借閱人")]
        public string BookKeeperName { get; set; }

        //借閱狀態代號
        [DisplayName("借閱狀態")]
        public string BookStatusId { get; set; }

        //借閱狀態名
        [DisplayName("借閱狀態")]
        public string BookStatusName { get; set; }

        //借閱日期
        [DisplayName("借閱日期")]
        public string LendDate { get; set; }

        //新增日期
        [DisplayName("新增日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public string CreateDate { get; set; }

        //新增人員
        [DisplayName("新增人員")]
        public string CreateUser { get; set; }

        //最後修改日期
        [DisplayName("最後修改日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public string ModifyDate { get; set; }

        //最後修改人員
        [DisplayName("最後修改人員")]
        public string ModifyUser { get; set; }
    }
}