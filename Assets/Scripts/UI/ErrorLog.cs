using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using System.Threading.Tasks;

using UnityEngine;
using TMPro;

public class ErrorLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private TMP_InputField userOpinion;

    private const string smtpServer = "smtp.gmail.com";
    private const string receieveMailAddress = "mashuustudio@gmail.com";
    private const string sendMailAddress = "mashuuerrorlog@gmail.com";
    private const string sendMailPassword = "rcmtnltotmiitiis";
    private const string sendMailUserName = "TETD ERROR LOG";

    private string logContent;

    public void Log(string log, string str)
    {
        logText.text = log;
        logContent = str;
        userOpinion.text = "";
        gameObject.SetActive(true);
    }

    public void SendLog()
    {
        mailSent = false;

        string mailSubject = "TETD ERROR LOG";
        string mailContent =
            "----------------------------------------------\n" +
            "LOG INFO \n" +
            logContent + "\n" +
            "----------------------------------------------\n" +
            "USER OPINION \n" +
            userOpinion.text + "\n" +
            "----------------------------------------------\n";

        var client = new SmtpClient(smtpServer)
        {
            Port = 587,
            Credentials = new NetworkCredential(sendMailAddress, sendMailPassword),
            EnableSsl = true
        };

        var from = new MailAddress(sendMailAddress, sendMailUserName, System.Text.Encoding.UTF8);
        var to = new MailAddress(receieveMailAddress);

        var mail = new MailMessage(from, to)
        {
            Body = mailContent,
            Subject = mailSubject
        };

        client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

        var userState = "TETD ERROR LOG";
        client.SendAsync(mail, userState);

        mail.Dispose();
        client.Dispose();

        gameObject.SetActive(false);
    }

    private static bool mailSent = false;
    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        mailSent = true;
    }
}
