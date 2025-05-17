Imports System.Runtime.CompilerServices

Namespace Model

    ''' <summary>
    ''' Created by Ahmed Hani Ibrahim on 12/19/2015.
    ''' </summary>
    Public Module Validator

        Public Function summationIsOne(list As Dictionary(Of String, Double)) As Boolean
            Dim sum As Double = 0.0

            For Each item As Double In list.Values
                sum += item
            Next

            Return sum = 1.0
        End Function

        Public Function isValidInitialProbabilities(states As List(Of String), initialProbabilities As Dictionary(Of String, Double)) As Boolean
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

        Public Function isValidTransitionMatrix(transitionMatrix As Dictionary(Of String, Double), states As List(Of String)) As Boolean
            If transitionMatrix.Count <> states.Count * states.Count Then Return False

            Dim frequency As New Dictionary(Of String, Boolean?)

            For Each item As String In transitionMatrix.Keys
                If frequency.ContainsKey(item) Then Return False
                frequency(item) = True
            Next

            Dim visited As New Dictionary(Of String, Boolean)

            For Each first As String In transitionMatrix.Keys
                Dim sum As Double = 0.0
                Dim entered As Integer = 0
                Dim state As String = first.key
                For Each second As String In transitionMatrix.Keys
                    If state.Equals(second.key) AndAlso (Not visited.ContainsKey(second)) Then
                        sum += transitionMatrix(second)
                        entered += 1
                        visited(second) = True
                    End If
                Next

                If sum <> 1.0 AndAlso entered > 0 Then Return False
            Next

            Return True
        End Function

        <Extension>
        Private Function key(state As String) As String
            Return Strings.Split(state, " -> ").First
        End Function

        Public Function isValidEmissionMatrix(emissionMatrix As Dictionary(Of String, Double), states As List(Of String), observations As List(Of String)) As Boolean
            If emissionMatrix.Count <> observations.Count * states.Count Then Return False

            Dim state As String

            For Each item As String In emissionMatrix.Keys
                Dim found As Boolean = False
                Dim sum As Double = 0.0
                Dim count As Integer = 0
                For i As Integer = 0 To states.Count - 1
                    For j As Integer = 0 To observations.Count - 1
                        state = $"{states(i)} -> {observations(j)}"

                        If String.Equals(item, state) Then
                            found = True
                            Exit For
                        End If
                    Next

                    If found Then Exit For
                Next

                If Not found Then Return False

                For Each item2 As String In emissionMatrix.Keys
                    If __keyEquals(item, item2) Then
                        sum += emissionMatrix(item2)
                        count += 1
                    End If
                Next

                If sum <> 1.0 AndAlso count > 0 Then Return False
            Next

            Return True
        End Function

        Private Function __keyEquals(k1 As String, k2 As String) As Boolean
            Return String.Equals(Strings.Split(k1, " -> "), Strings.Split(k2, " -> "))
        End Function
    End Module
End Namespace