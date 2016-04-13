Imports System
Imports System.Text
Imports Microsoft.VisualBasic.Serialization

Namespace Util.Parser

    Public Structure modelInfo
        Public Property name As String
        Public Property [date] As String
        Public Property title As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Structure modelData
        Public Property states As String()
        Public Property init_props As Dictionary(Of String, Double)
        Public Property observations As String()
        Public Property transitions As String()
        Public Property emissions As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    ''' <summary>
    ''' Created by Ahmed Hani Ibrahim on 12/19/2015.
    ''' </summary>
    Public Class JsonParser
        Public Property modelInfo As modelInfo
        Public Property modelData As modelData

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function LoadJson(path As String) As JsonParser
            Return JsonContract.LoadJsonFile(Of JsonParser)(path)
        End Function
    End Class
End Namespace