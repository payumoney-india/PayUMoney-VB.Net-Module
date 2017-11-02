Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Security.Cryptography
Imports System.Text

Public Class ResponseHandling
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Dim merc_hash_vars_seq As String()
            Dim merc_hash_string As String = String.Empty
            Dim merc_hash As String = String.Empty
            Dim order_id As String = String.Empty
            Dim hash_seq As String = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10"

            If Request.Form("status") = "success" Then

                merc_hash_vars_seq = hash_seq.Split("|"c)
                Array.Reverse(merc_hash_vars_seq)
                merc_hash_string = ConfigurationManager.AppSettings("SALT") + "|" & Request.Form("status")


                For Each merc_hash_var As String In merc_hash_vars_seq
                    merc_hash_string += "|"

                    merc_hash_string = merc_hash_string & (If(Request.Form(merc_hash_var) IsNot Nothing, Request.Form(merc_hash_var), ""))
                Next
                Response.Write(merc_hash_string)
                [exit]()
                merc_hash = Generatehash512(merc_hash_string).ToLower()



                If merc_hash <> Request.Form("hash") Then

                    Response.Write("Hash value did not matched")
                Else
                    order_id = Request.Form("txnid")


                    'Hash value did not matched
                    Response.Write("value matched")

                End If
            Else


                ' osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                Response.Write("Hash value did not matched")
            End If

        Catch ex As Exception

            Response.Write("<span style='color:red'>" & ex.Message & "</span>")
        End Try
    End Sub

    Public Function Generatehash512(ByVal text As String) As String

        Dim message As Byte() = Encoding.UTF8.GetBytes(text)

        Dim UE As New UnicodeEncoding()
        Dim hashValue As Byte()
        Dim hashString As New SHA512Managed()
        Dim hex As String = ""
        hashValue = hashString.ComputeHash(message)
        For Each x As Byte In hashValue
            hex += [String].Format("{0:x2}", x)
        Next
        Return hex

    End Function






End Class