
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Security.Cryptography
Imports System.Text
Imports System.Net
Imports System.IO
Public Class _Default
    Inherits System.Web.UI.Page
    Public action1 As String = String.Empty
    Public hash1 As String = String.Empty
    Public txnid1 As String = String.Empty
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try



            key.Value = ConfigurationManager.AppSettings("MERCHANT_KEY")

            If Not IsPostBack Then
                ' error form
                frmError.Visible = False
                'frmError.Visible = true;
            Else
            End If
            If String.IsNullOrEmpty(Request.Form("hash")) Then
                submit.Visible = True
            Else
                submit.Visible = False

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





    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles submit.Click
        Try

            Dim hashVarsSeq As String()
            Dim hash_string As String = String.Empty


            If String.IsNullOrEmpty(Request.Form("txnid")) Then
                ' generating txnid
                Dim rnd As New Random()
                Dim strHash As String = Generatehash512(rnd.ToString() & DateTime.Now)

                txnid1 = strHash.ToString().Substring(0, 20)
            Else
                txnid1 = Request.Form("txnid")
            End If
            If String.IsNullOrEmpty(Request.Form("hash")) Then
                ' generating hash value
                If String.IsNullOrEmpty(ConfigurationManager.AppSettings("MERCHANT_KEY")) OrElse String.IsNullOrEmpty(txnid1) OrElse String.IsNullOrEmpty(Request.Form("amount")) OrElse String.IsNullOrEmpty(Request.Form("firstname")) OrElse String.IsNullOrEmpty(Request.Form("email")) OrElse String.IsNullOrEmpty(Request.Form("phone")) OrElse String.IsNullOrEmpty(Request.Form("productinfo")) OrElse String.IsNullOrEmpty(Request.Form("surl")) OrElse String.IsNullOrEmpty(Request.Form("furl")) OrElse String.IsNullOrEmpty(Request.Form("service_provider")) Then
                    'error

                    frmError.Visible = True
                    Return
                Else

                    frmError.Visible = False
                    hashVarsSeq = ConfigurationManager.AppSettings("hashSequence").Split("|"c)
                    ' spliting hash sequence from config
                    hash_string = ""
                    For Each hash_var As String In hashVarsSeq
                        If hash_var = "key" Then
                            hash_string = hash_string + ConfigurationManager.AppSettings("MERCHANT_KEY")
                            hash_string = hash_string & "|"c
                        ElseIf hash_var = "txnid" Then
                            hash_string = hash_string & txnid1
                            hash_string = hash_string & "|"c
                        ElseIf hash_var = "amount" Then
                            hash_string = hash_string & Convert.ToDecimal(Request.Form(hash_var)).ToString("g29")
                            hash_string = hash_string & "|"c
                        Else

                            hash_string = hash_string & (If(Request.Form(hash_var) IsNot Nothing, Request.Form(hash_var), ""))
                            ' isset if else
                            hash_string = hash_string & "|"c
                        End If
                    Next

                    hash_string += ConfigurationManager.AppSettings("SALT")
                    ' appending SALT
                    hash1 = Generatehash512(hash_string).ToLower()
                    'generating hash
                    ' setting URL
                    action1 = ConfigurationManager.AppSettings("PAYU_BASE_URL") + "/_payment"


                End If

            ElseIf Not String.IsNullOrEmpty(Request.Form("hash")) Then
                hash1 = Request.Form("hash")

                action1 = ConfigurationManager.AppSettings("PAYU_BASE_URL") + "/_payment"
            End If




            If Not String.IsNullOrEmpty(hash1) Then
                hash.Value = hash1
                txnid.Value = txnid1

                Dim data As New System.Collections.Hashtable()
                ' adding values in gash table for data post
                data.Add("hash", hash.Value)
                data.Add("txnid", txnid.Value)
                data.Add("key", key.Value)
                Dim AmountForm As String = Convert.ToDecimal(amount.Text.Trim()).ToString("g29")
                ' eliminating trailing zeros
                amount.Text = AmountForm
                data.Add("amount", AmountForm)
                data.Add("firstname", firstname.Text.Trim())
                data.Add("email", email.Text.Trim())
                data.Add("phone", phone.Text.Trim())
                data.Add("productinfo", productinfo.Text.Trim())
                data.Add("surl", surl.Text.Trim())
                data.Add("furl", furl.Text.Trim())
                data.Add("lastname", lastname.Text.Trim())
                data.Add("curl", curl.Text.Trim())
                data.Add("address1", address1.Text.Trim())
                data.Add("address2", address2.Text.Trim())
                data.Add("city", city.Text.Trim())
                data.Add("state", state.Text.Trim())
                data.Add("country", country.Text.Trim())
                data.Add("zipcode", zipcode.Text.Trim())
                data.Add("udf1", udf1.Text.Trim())
                data.Add("udf2", udf2.Text.Trim())
                data.Add("udf3", udf3.Text.Trim())
                data.Add("udf4", udf4.Text.Trim())
                data.Add("udf5", udf5.Text.Trim())
                data.Add("pg", pg.Text.Trim())
                data.Add("service_provider", service_provider.Text.Trim())


                Dim strForm As String = PreparePOSTForm(action1, data)

                Page.Controls.Add(New LiteralControl(strForm))
            Else
                'no hash



            End If

        Catch ex As Exception


            Response.Write("<span style='color:red'>" & ex.Message & "</span>")
        End Try

    End Sub



    Private Function PreparePOSTForm(ByVal url As String, ByVal data As System.Collections.Hashtable) As String
        ' post form
        'Set a name for the form
        Dim formID As String = "PostForm"
        'Build the form using the specified data to be posted.
        Dim strForm As New StringBuilder()
        strForm.Append("<form id=""" & formID & """ name=""" & formID & """ action=""" & url & """ method=""POST"">")

        For Each key As System.Collections.DictionaryEntry In data

            strForm.Append("<input type=""hidden"" name=""" & Convert.ToString(key.Key) & """ value=""" & Convert.ToString(key.Value) & """>")
        Next


        strForm.Append("</form>")
        'Build the JavaScript which will do the Posting operation.
        Dim strScript As New StringBuilder()
        strScript.Append("<script language='javascript'>")
        strScript.Append("var v" & formID & " = document." & formID & ";")
        strScript.Append("v" & formID & ".submit();")
        strScript.Append("</script>")
        'Return the form and the script concatenated.
        '(The order is important, Form then JavaScript)
        Return strForm.ToString() & strScript.ToString()
    End Function





End Class