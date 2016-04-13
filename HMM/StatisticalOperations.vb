Imports Microsoft.VisualBasic

''' <summary>
''' Created by Ahmed Hani Ibrahim on 12/24/2015.
''' </summary>
Public Module StatisticalOperations

    ''' <summary>
    ''' Probabilities Normalization </summary>
    ''' <param name="probabilities"> A Hashtable which contains the probability values </param>
    ''' <param name="states"> A Vector which is the model states </param>
    ''' <returns> Normalized probabilities as a Hashtable </returns>

    Public Function normalize(probabilities As List(Of Dictionary(Of String, Double)), states As List(Of String)) As List(Of Dictionary(Of String, Double))
        Dim sum As Double = 0.0
        If states.Count = 1 Then Return probabilities

        For t As Integer = 0 To states.Count - 1
            For i As Integer = 0 To probabilities.Count - 1
                sum += (probabilities(t)(states(i)))
            Next
        Next

        For t As Integer = 0 To states.Count - 1
            For i As Integer = 0 To probabilities.Count - 1
                Dim current As Double = (probabilities(t)(states(i)))
                probabilities(t)(states(i)) = current / sum
            Next
        Next

        Return probabilities
    End Function

    Public Function normalize(data As List(Of Double)) As List(Of Double)
        Dim res As New List(Of Double)
        Dim sum As Double = 0.0

        For i As Integer = 0 To data.Count - 1
            sum += data(i)
        Next

        For i As Integer = 0 To data.Count - 1
            res.Add(data(i) / sum)
        Next

        Return res
    End Function
End Module