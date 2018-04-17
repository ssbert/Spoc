namespace SPOC.ExamPaper.Dto
{
    public class PaperPreviewOutputDto
    {
        public PaperPreviewOutputDto()
        {
            code = "";
            title = "";
            subTitle = "";
        }
        public string viewHtml { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string paperId { get; set; }
        public bool isPolicy { get; set; }
    }
}