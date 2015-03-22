Public Class Crawl

    Private Const baseURL1 = "http://www.data.jma.go.jp/obd/stats/etrn/view/daily_a1.php?"
    Private Const baseURL2 = "http://www.data.jma.go.jp/obd/stats/etrn/view/daily_s1.php?"

    Private baseURls As Dictionary(Of Integer, String)

    Private _prefecture_number As String = Nothing
    Private _block_number As String = Nothing
    Private _year As String = Nothing
    Private _month As String = Nothing

    Private _directory As String = "output"
    Private _dt As System.Data.DataTable = Nothing


    Public Property OutputDirectory As String
        Get
            Return _directory
        End Get
        Set(value As String)
            _directory = value
        End Set
    End Property

    Public Property PrefectureNumber As String
        Get
            Return _prefecture_number
        End Get
        Set(value As String)
            _prefecture_number = value
        End Set

    End Property

    Public ReadOnly Property WeatherData As DataTable
        Get
            Return _dt
        End Get
    End Property

    Public ReadOnly Property PrefectureName As String
        Get
            Return (New Prefecture).getPrefectureName(Me._prefecture_number)
        End Get
    End Property

    Public Property BlockNumber As String
        Get
            Return _block_number
        End Get
        Set(value As String)
            _block_number = value
        End Set
    End Property

    Public ReadOnly Property BlockName As String
        Get
            Return (New PrefectureBlock).getBlockName(Me._prefecture_number, Me._block_number)
        End Get
    End Property

    Public ReadOnly Property Year As String
        Get
            Return Me._year
        End Get
    End Property

    Public ReadOnly Property Month As String
        Get
            Return Me._month
        End Get
    End Property

    Public Sub New()
        _dt = New System.Data.DataTable
        _dt.Columns.Add("都道府県番号", GetType(String))
        _dt.Columns.Add("地域番号", GetType(String))
        _dt.Columns.Add("年月日", GetType(String))
        _dt.Columns.Add("降水量合計", GetType(String))
        _dt.Columns.Add("降水量時間", GetType(String))
        _dt.Columns.Add("降水量分", GetType(String))
        _dt.Columns.Add("気温平均", GetType(String))
        _dt.Columns.Add("気温最高", GetType(String))
        _dt.Columns.Add("気温最低", GetType(String))
        _dt.Columns.Add("平均風速", GetType(String))
        _dt.Columns.Add("最大風速", GetType(String))
        _dt.Columns.Add("最大風向", GetType(String))
        _dt.Columns.Add("瞬間最大風速", GetType(String))
        _dt.Columns.Add("瞬間最大風向", GetType(String))
        _dt.Columns.Add("日照時間", GetType(String))
        _dt.Columns.Add("降雪合計", GetType(String))
        _dt.Columns.Add("最深積雪", GetType(String))

        baseURls = New Dictionary(Of Integer, String)
        baseURls.Add(4, baseURL1)
        baseURls.Add(5, baseURL2)

    End Sub

    ''' <summary>
    ''' 気象庁のHPにアクセスし、過去の気象情報を取得する。取得した結果はDatatableとして出力する
    ''' </summary>
    ''' <param name="prefecture_number"></param>
    ''' <param name="block_number"></param>
    ''' <param name="year"></param>
    ''' <param name="month"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWeather( _
        prefecture_number As String, block_number As String, year As String, month As String) _
            As System.Data.DataTable

        Me._prefecture_number = prefecture_number
        Me._block_number = block_number
        Me._year = year
        Me._month = month

        Dim urlString = baseURls(block_number.Length)
        urlString += "prec_no=" & prefecture_number
        urlString += "&block_no=" & block_number
        urlString += "&year=" & year
        urlString += "&month=" & month
        urlString += "&day="
        urlString += "&view="

        Dim s As String = String.Empty
        Dim client As New System.Net.WebClient()
        Dim data As System.IO.Stream = Nothing
        Dim reader As System.IO.StreamReader = Nothing
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("utf-8")

        Try
            data = client.OpenRead(urlString)
            reader = New System.IO.StreamReader(data, enc)
            s = reader.ReadToEnd()

            Console.WriteLine(urlString)

            If block_number.Length = 4 Then

                If s.IndexOf("<!-- contents -->") < 0 Then Throw New Exception("begin message can not found")
                If s.IndexOf("<!-- //contents -->") < 0 Then Throw New Exception("end message can not found")

                s = s.Substring(s.IndexOf("<!-- contents -->"), (s.IndexOf("<!-- //contents -->") - s.IndexOf("<!-- contents -->")))

                Dim doc As HtmlAgilityPack.HtmlDocument = New HtmlAgilityPack.HtmlDocument
                doc.LoadHtml(s)

                Dim xpath As String
                xpath = "/table[1]"
                Dim _nodes As HtmlAgilityPack.HtmlNodeCollection = doc.DocumentNode.SelectNodes(xpath)

                For Each value As HtmlAgilityPack.HtmlNode In _nodes.Nodes

                    If value.HasChildNodes = True And value.ChildNodes.Count = 16 Then
                        Dim row As DataRow = _dt.NewRow
                        row.Item(0) = Me.PrefectureNumber
                        row.Item(1) = Me.BlockNumber
                        row.Item(2) = Me.Year & "-" & Me.Month & "-" & value.ChildNodes(0).InnerText
                        row.Item(3) = unkonData(value.ChildNodes(1).InnerText)
                        row.Item(4) = unkonData(value.ChildNodes(2).InnerText)
                        row.Item(5) = unkonData(value.ChildNodes(3).InnerText)
                        row.Item(6) = unkonData(value.ChildNodes(4).InnerText)
                        row.Item(7) = unkonData(value.ChildNodes(5).InnerText)
                        row.Item(8) = unkonData(value.ChildNodes(6).InnerText)
                        row.Item(9) = unkonData(value.ChildNodes(7).InnerText)
                        row.Item(10) = unkonData(value.ChildNodes(8).InnerText)
                        row.Item(11) = unkonData(value.ChildNodes(9).InnerText)
                        row.Item(12) = unkonData(value.ChildNodes(10).InnerText)
                        row.Item(13) = unkonData(value.ChildNodes(11).InnerText)
                        row.Item(14) = unkonData(value.ChildNodes(13).InnerText)
                        row.Item(15) = unkonData(value.ChildNodes(14).InnerText)
                        row.Item(16) = unkonData(value.ChildNodes(15).InnerText)

                        _dt.Rows.Add(row)

                    End If
                Next

            ElseIf block_number.Length = 5 Then

                If s.IndexOf("<!-- contents -->") < 0 Then Throw New Exception("begin message can not found")
                'Console.WriteLine(s.IndexOf("<div class=" & Chr(34) & "print" & Chr(34) & ">"), s.IndexOf("<!-- contents -->"))
                If s.IndexOf("<p class=" & Chr(34) & "totop" & Chr(34) & ">") < 0 Then Throw New Exception("end message can not found")

                s = s.Substring(s.IndexOf("<!-- contents -->"), s.IndexOf("<p class=" & Chr(34) & "totop" & Chr(34) & ">") - s.IndexOf("<!-- contents -->"))

                Dim doc As HtmlAgilityPack.HtmlDocument = New HtmlAgilityPack.HtmlDocument
                doc.LoadHtml(s)

                Dim xpath As String
                xpath = "/table[1]"
                xpath = "/table[1]/tr[5]"
                Dim _nodes As HtmlAgilityPack.HtmlNodeCollection = doc.DocumentNode.SelectNodes(xpath)
                Dim row As DataRow

                Do Until _nodes(0).ChildNodes.Count = 21
                    row = _dt.NewRow
                    row.Item(0) = Me.PrefectureNumber
                    row.Item(1) = Me.BlockNumber
                    row.Item(2) = Me.Year & "-" & Me.Month & "-" & _nodes(0).ChildNodes(0).InnerText
                    row.Item(3) = unkonData(_nodes(0).ChildNodes(3).InnerText)
                    row.Item(4) = unkonData(_nodes(0).ChildNodes(4).InnerText)
                    row.Item(5) = unkonData(_nodes(0).ChildNodes(5).InnerText)
                    row.Item(6) = unkonData(_nodes(0).ChildNodes(6).InnerText)
                    row.Item(7) = unkonData(_nodes(0).ChildNodes(7).InnerText)
                    row.Item(8) = unkonData(_nodes(0).ChildNodes(8).InnerText)
                    row.Item(9) = unkonData(_nodes(0).ChildNodes(11).InnerText)
                    row.Item(10) = unkonData(_nodes(0).ChildNodes(12).InnerText)
                    row.Item(11) = unkonData(_nodes(0).ChildNodes(13).InnerText)
                    row.Item(12) = unkonData(_nodes(0).ChildNodes(14).InnerText)
                    row.Item(13) = unkonData(_nodes(0).ChildNodes(15).InnerText)
                    row.Item(14) = unkonData(_nodes(0).ChildNodes(16).InnerText)
                    row.Item(15) = unkonData(_nodes(0).ChildNodes(17).InnerText)
                    row.Item(16) = unkonData(_nodes(0).ChildNodes(18).InnerText)

                    _dt.Rows.Add(row)

                    doc = New HtmlAgilityPack.HtmlDocument
                    doc.LoadHtml(_nodes(0).InnerHtml)
                    xpath = "/tr[1]"
                    _nodes = doc.DocumentNode.SelectNodes(xpath)


                Loop
                
                row = _dt.NewRow
                row.Item(0) = Me.PrefectureNumber
                row.Item(1) = Me.BlockNumber
                row.Item(2) = Me.Year & "-" & Me.Month & "-" & _nodes(0).ChildNodes(0).InnerText
                row.Item(3) = unkonData(_nodes(0).ChildNodes(3).InnerText)
                row.Item(4) = unkonData(_nodes(0).ChildNodes(4).InnerText)
                row.Item(5) = unkonData(_nodes(0).ChildNodes(5).InnerText)
                row.Item(6) = unkonData(_nodes(0).ChildNodes(6).InnerText)
                row.Item(7) = unkonData(_nodes(0).ChildNodes(7).InnerText)
                row.Item(8) = unkonData(_nodes(0).ChildNodes(8).InnerText)
                row.Item(9) = unkonData(_nodes(0).ChildNodes(11).InnerText)
                row.Item(10) = unkonData(_nodes(0).ChildNodes(12).InnerText)
                row.Item(11) = unkonData(_nodes(0).ChildNodes(13).InnerText)
                row.Item(12) = unkonData(_nodes(0).ChildNodes(14).InnerText)
                row.Item(13) = unkonData(_nodes(0).ChildNodes(15).InnerText)
                row.Item(14) = unkonData(_nodes(0).ChildNodes(16).InnerText)
                row.Item(15) = unkonData(_nodes(0).ChildNodes(17).InnerText)
                row.Item(16) = unkonData(_nodes(0).ChildNodes(18).InnerText)

                _dt.Rows.Add(row)


            End If


        Catch ex As Exception
            Console.WriteLine(ex.Message)

        Finally
            If Not data Is Nothing Then data.Close()
            If Not reader Is Nothing Then reader.Close()
            If Not client Is Nothing Then client.Dispose()

        End Try

        If _dt.Rows.Count > 0 Then
            Dim d2t As New jp.polestar.io.Datatable2TSV
            d2t.dt2tsv(_dt, PreparationOutputEnv)
        End If

        Return _dt

    End Function

    Private Function unkonData(value As String) As String

        Dim result As String
        If value = "///" Then
            result = String.Empty
        ElseIf value = "--" Then
            result = String.Empty
        Else
            result = value
        End If
        Return result
    End Function

    ''' <summary>
    ''' 出力するファイルシステムの準備
    ''' ディレクトリーがなければディレクトリを作成する
    ''' </summary>
    ''' <returns>出力するファイル名</returns>
    ''' <remarks></remarks>
    Private Function PreparationOutputEnv() As String

        Dim d As System.IO.DirectoryInfo
        Dim result As String = String.Empty

        d = New System.IO.DirectoryInfo(OutputDirectory)
        If Not d.Exists Then d.Create()

        d = New System.IO.DirectoryInfo(OutputDirectory & "\" & PrefectureNumber)
        If Not d.Exists Then d.Create()

        d = New System.IO.DirectoryInfo(OutputDirectory & "\" & PrefectureNumber & "\" & BlockNumber)
        If Not d.Exists Then d.Create()

        d = New System.IO.DirectoryInfo(OutputDirectory & "\" & PrefectureNumber & "\" & BlockNumber & "\" & Year)
        If Not d.Exists Then d.Create()

        result = OutputDirectory & "\" & PrefectureNumber & "\" & BlockNumber & "\" & Year & "\" & PrefectureNumber & "_" & BlockNumber & "_" & Year & "_" & Month & ".txt"

        Return result

    End Function

    Public Sub margeFile()

        Const exportFileName As String = "weather.txt"
        Dim files As String() = System.IO.Directory.GetFiles(_directory, "*.txt", System.IO.SearchOption.AllDirectories)
        Dim dt As System.Data.DataTable = _dt
        Dim d2t1 As jp.polestar.io.Datatable2TSV

        d2t1 = New jp.polestar.io.Datatable2TSV
        d2t1.dt2tsv(dt, exportFileName, True, io.FileAction.DeleteCreate)

        For Each fileName As String In files
            Dim d2t As New jp.polestar.io.Datatable2TSV
            Dim d As System.Data.DataTable
            d = d2t.tsv2dt(fileName, "weather_point", True)

            For Each row As System.Data.DataRow In d.Rows
                Dim _row As System.Data.DataRow
                _row = dt.NewRow

                For i As Integer = 0 To 16
                    _row(i) = row(i)
                Next
                dt.Rows.Add(_row)

            Next

            d2t1 = New jp.polestar.io.Datatable2TSV
            d2t1.dt2tsv(dt, exportFileName, False, io.FileAction.Overwrite)
            dt.Rows.Clear()

        Next

        
    End Sub

End Class
