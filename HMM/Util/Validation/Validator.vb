Imports System.Collections.Generic

Namespace Util.Validation



    ''' <summary>
    ''' Created by Ahmed Hani Ibrahim on 12/19/2015.
    ''' </summary>
    Public Module Validator

        Public Function summationIsOne(ByVal list As Dictionary(Of String, Double)) As Boolean
            Dim sum As Double = 0.0

            For Each item As Double In list.Values
                sum += item
            Next

            Return sum = 1.0
        End Function

        Public Function isValidInitialProbabilities(ByVal states As List(Of String), ByVal initialProbabilities As Dictionary(Of String, Double)) As Boolean
            If states.Count <> initialProbabilities.Count Then Return False

            For i As Integer = 0 To states.Count - 1
                Dim found As Boolean = False
                For Each state As String In initialProbabilities.Keys
                    If state.Equals(states(i)) Then
                        found = True
                        Exit For
                    End If
                Next

                If Not found Then Return False
            Next

            Return True
        End Function

        Public Function isValidTransitionMatrix(ByVal transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), ByVal states As List(Of String)) As Boolean
            If transitionMatrix.Count <> states.Count * states.Count Then Return False

            Dim frequency As New Dictionary(Of KeyValuePair(Of String, String), Boolean?)

            For Each item As KeyValuePair(Of String, String) In transitionMatrix.Keys
                If frequency.ContainsKey(item) Then Return False
                frequency(item) = True
            Next

            Dim visited As New Dictionary(Of KeyValuePair(Of String, String), Boolean?)

            For Each first As KeyValuePair(Of String, String) In transitionMatrix.Keys
                Dim sum As Double = 0.0
                Dim entered As Integer = 0
                Dim state As String = first.Key
                For Each second As KeyValuePair(Of String, String) In transitionMatrix.Keys
                    If state.Equals(second.Key) AndAlso (Not visited.ContainsKey(second)) Then
                        sum += transitionMatrix(second)
                        entered += 1
                        visited(second) = True
                    End If
                Next

                If sum <> 1.0 AndAlso entered > 0 Then Return False
            Next

            Return True
        End Function

        Public Function isValidEmissionMatrix(ByVal emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), ByVal states As List(Of String), ByVal observations As List(Of String)) As Boolean
            If emissionMatrix.Count <> observations.Count * states.Count Then Return False

            For Each item As KeyValuePair(Of String, String) In emissionMatrix.Keys
                Dim found As Boolean = False
                Dim sum As Double = 0.0
                Dim count As Integer = 0
                For i As Integer = 0 To states.Count - 1
                    For j As Integer = 0 To observations.Count - 1
                        If item.Key.Equals(states(i)) AndAlso item.Value.Equals(observations(j)) Then
                            found = True
                            Exit For
                        End If
                    Next

                    If found Then Exit For
                Next

                If Not found Then Return False

                For Each item2 As KeyValuePair(Of String, String) In emissionMatrix.Keys
                    If item.Key.Equals(item2.Key) Then
                        sum += emissionMatrix(item2)
                        count += 1
                    End If
                Next

                If sum <> 1.0 AndAlso count > 0 Then Return False
            Next

            Return True
        End Function
    End Module

End Namespace