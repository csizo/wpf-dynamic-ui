using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel
{
    public class FeedbackModel : NavigationModel
    {
        [Required]
        [StringLength(100)]
        [Display(AutoGenerateField = true, Description = "Email address", Prompt = "Enter email")]
        public string To { get; set; }

        [Required]
        [StringLength(100)]
        [Display(AutoGenerateField = true, Description = "Subject", Prompt = "Enter subject")]
        public string Subject { get; set; }

        [Required]
        [StringLength(4000)]
        [UIHint(UIHints.DisplayRichTextBox)]
        [Display(AutoGenerateField = true, Description = "Body")]
        public string Body { get; set; }

        [Display(AutoGenerateField = false)]
        public bool CanSendFeedback
        {
            get { return Subject != null && Body != null; }
        }

        [Display(AutoGenerateField = true, Name = "Send email", Description = "Send email using the local mail client")]
        public void SendFeedback()
        {
            string processName = string.Format("mailto:{0}?Subject={1}&Body={2}", To, UrlEncode(Subject),
                                               UrlEncode(Body));
            Process.Start(processName);

            BusinessApplication.Instance.ShowPopup(
                "Sending Email via the default email client. Thanks for the feedback!");
        }

        [Display(AutoGenerateField = true, Name = "Feedback", Description = "Opens feedback form")]
        public static void EmailToDeveloper()
        {
            BusinessApplication.Instance.OpenModel(() => new FeedbackModel());
        }

        private static string UrlEncode(string text)
        {
            return Uri.EscapeDataString(text); //.Replace("%20", "+");
        }
    }
}