using System;
//using System.Web.UI.WebControls;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.IO;
using System.Collections.Generic;
using InoMessagerie.Models;
using System.Net;

namespace InoMessagerie
{
    public class Messagerie
    {
        System.Net.Mail.SmtpClient client;
        string messageFromMail;
        public Messagerie(ServerConfigurationModel serverInfos)
        {
            //Server Configuration
            client = new System.Net.Mail.SmtpClient(serverInfos.server, serverInfos.port);
            client.EnableSsl = serverInfos.useSsl != 0 ? true : false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(serverInfos.username, serverInfos.password);
            messageFromMail = serverInfos.username;
        }


        public bool SendMail(SendMailParamsModel Params)
        {

            try
            {
                MailMessage msgObj = new MailMessage();

                msgObj.From = new MailAddress(messageFromMail);

                //add receivers
                foreach (var mail in Params.messageTo)
                {
                    msgObj.To.Add(mail);
                }

                //Subject
                msgObj.Subject = Params.Subject;

                //Body
                msgObj.Body = Params.content;

                //add attachments file
                foreach (Attachments FileBase64 in Params.attachments)
                {
                    Byte[] bitmapData = Convert.FromBase64String(FixBase64ForPdf(FileBase64.file));
                    System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);

                    var mail = new MailMessage();
                    var imageToInline = new LinkedResource(streamBitmap, System.Net.Mime.MediaTypeNames.Application.Pdf);
                    imageToInline.ContentId = FileBase64.name;

                    Attachment attachment = new Attachment(imageToInline.ContentStream, imageToInline.ContentType);
                    attachment.Name = FileBase64.name;
                    attachment.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    msgObj.Attachments.Add(attachment);
                }

                // add cc
                foreach (var mail in Params.Cc)
                {
                    msgObj.CC.Add(mail);
                }

                //add bcc
                foreach (var mail in Params.Bcc)
                {
                    msgObj.Bcc.Add(mail);
                }


                client.Send(msgObj);

                return true;
            }

            catch (SmtpException err)
            {
                throw new Exception("302");
            }
            catch (ArgumentNullException err)
            {
                throw new Exception("303");
            }
        }
        public static string FixBase64ForPdf(string pdf)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(pdf, pdf.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }
    }
}