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

        Public Function GetMatrix(json As String()) As Dictionary(Of String, Double)
            Dim ___transitionMatrix As New Dictionary(Of String, Double)
            Dim state As String
            Dim prob As Double

            For Each expression As String In json
                Dim transitionExpression As String() = StringSplit(expression, INNER_SPLITTER, True)

                For i As Integer = 0 To transitionExpression.Length - 1 Step 3
                    state = $"{transitionExpression(i)} -> {transitionExpression(i + 1)}"
                    prob = Convert.ToDouble(transitionExpression(i + 2))
                    ___transitionMatrix(state) = prob
                Next
            Next

            Return ___transitionMatrix
        End Function
    End Module
End Namespace