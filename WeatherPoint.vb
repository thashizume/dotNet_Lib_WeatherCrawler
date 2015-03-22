Public Class WeatherPoint


    Private _dt As System.Data.DataTable = Nothing
    Private _fileName As String = "weather_point.txt"
    Private Const _dataTableName = "WEATHER_POINT"

    Public Sub New()

        Dim d2v As New jp.polestar.io.Datatable2TSV
        _dt = d2v.tsv2dt(_fileName, _dataTableName)

        'PREFECTURE_ID
        'PREFECTURE_NAME
        'POINT_NAME
        'POINT_SHORT_NAME
        'BLOCK_ID
        'BLOCK_NAME
        'BLOCK_NAME_KANA
        'BLOCK_TYPE
        'LATITUDE
        'LONGITUDE
        'ALTITUDE


    End Sub

    Public ReadOnly Property WeatherPoint As DataTable
        Get
            Return _dt
        End Get
    End Property

    Public Function getPrefectureList() As List(Of String)

        Dim result As New List(Of String)

        For Each row As System.Data.DataRow In _dt.Rows

            'Console.WriteLine(result.IndexOf(row("PREFECTURE_NAME")))


            If result.IndexOf(row("PREFECTURE_NAME")) < 0 Then result.Add(row("PREFECTURE_NAME"))


        Next
        Return result

    End Function

    Public Function getPointList(prefectureName As String) As Dictionary(Of String, String)

        Dim result As New Dictionary(Of String, String)

        For Each row As System.Data.DataRow In _dt.Rows

            If row("PREFECTURE_NAME") = prefectureName Then
                result.Add(row("PREFECTURE_ID"), row("POINT_SHORT_NAME"))
            End If


            'If Not result.ContainsKey(row("PREFECTURE_ID")) Then result.Add(row("PREFECTURE_ID"), row("POINT_SHORT_NAME"))
        Next
        Return result

    End Function

    Public Sub CrawlBlock(prefectureNumber As String, blockNumber As String, beginYear As Integer, Optional years As Integer = 0)

        For i As Integer = beginYear To beginYear + IIf(years = 0, years, years - 1)
            For m As Integer = 1 To 12
                Dim c As New Crawl
                c.getWeather(prefectureNumber, blockNumber, i, m)

            Next
        Next

    End Sub


End Class
