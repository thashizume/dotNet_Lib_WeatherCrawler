Public Class PrefectureBlock


    Private _prefectureBlockName As String
    Private _prefectureBlockNumber As String

    Private _dt As DataTable
    Private _fileName As String = "weather_prefectire_brock.txt"
    Private Const _dataTableName = "WEATHER_PREFECTURE_BLOCK"

    Public Property FileName As String
        Get
            Return _fileName
        End Get
        Set(value As String)
            _fileName = value
        End Set

    End Property

    Public ReadOnly Property PrefectureBlock As System.Data.DataTable
        Get
            Return _dt
        End Get
    End Property

    Public Sub New()


        If New System.IO.FileInfo(Me.FileName).Exists Then
            Me.LoadFile()
        Else
            _dt = New DataTable(_dataTableName)
            _dt.Columns.Add("PREFECTURE_NAME", GetType(String))
            _dt.Columns.Add("PREFECTURE_BLOCK_NUMBER", GetType(String))
            _dt.Columns.Add("PREFECTURE_BLOCK_NAME", GetType(String))
            _dt.Columns.Add("PREFECTURE_NUMBER", GetType(String))


        End If

    End Sub

    Public Sub FlashFile()
        Dim tsv As New jp.polestar.io.Datatable2TSV
        tsv.dt2tsv(_dt, _fileName)

    End Sub

    Public Sub LoadFile()
        Dim tsv As New jp.polestar.io.Datatable2TSV
        _dt = tsv.tsv2dt(_fileName, _dataTableName)
    End Sub


    Public Function Add(prefecture_name As String, block_number As String, block_name As String) As Integer

        Dim r As DataRow = _dt.NewRow
        r(0) = prefecture_name
        r(1) = block_number
        r(2) = block_name
        _dt.Rows.Add(r)

        Me.FlashFile()
        Me.LoadFile()

        Return 0

    End Function


    Public Function getBlockName(prefectureNumber As String, blockNumber As String) As String

        For Each v As System.Data.DataRow In _dt.Rows
            If v.Item("PREFECTURE_NUMBER") = prefectureNumber And v.Item("PREFECTURE_BLOCK_NUMBER") = blockNumber Then Return v.Item("PREFECTURE_BLOCK_NAME")
        Next

        Return String.Empty
    End Function
End Class
