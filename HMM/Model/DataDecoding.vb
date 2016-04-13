Imports System
Imports Microsoft.VisualBasic

Namespace Model

    ''' <summary>
    ''' 
    ''' </summary>
    Public Module DataDecoding

        Const OUTER_SPLITTER As String = ", "
        Const INNER_SPLITTER As String = "->"

        ''' <summary>
        ''' Get the transition matrix of the model </summary>
        ''' <param name="json"> A string that hold the json expression of the model transition matrix </param>
        ''' <returns> A Hasshtable that is the transition matrix of the model </returns>

        Public Function GetMatrix(json As String()) As Dictionary(Of KeyValuePair(Of String, String), Double)
            Dim ___transitionMatrix As New Dictionary(Of KeyValuePair(Of String, String), Double)

            For Each expression As String In json
                Dim transitionExpression As String() = StringSplit(expression, INNER_SPLITTER, True)

                For i As Integer = 0 To transitionExpression.Length - 1 Step 3
                    ___transitionMatrix(New KeyValuePair(Of String, String)(transitionExpression(i), transitionExpression(i + 1))) = Convert.ToDouble(transitionExpression(i + 2))
                Next
            Next

            Return ___transitionMatrix
        End Function
    End Module
End Namespace