Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Trinity.NLP
Imports Microsoft.VisualBasic.Linq
Imports rnd = Microsoft.VisualBasic.Math.RandomExtensions

Public Class Generator

    Dim previous, curWord As String

    ReadOnly len As IntRange
    ReadOnly pairs As New Dictionary(Of String, List(Of Word))()

    Public Sub New(len As IntRange)
        Me.len = len
    End Sub

    Public Function generate([string] As String) As String
        Dim generatedText = ""
        createTable([string])
        Dim numWords As Integer = randomizeNumWords()
        For i = 0 To numWords - 1
            If generatedText.Equals("") Then
                Dim keys As New List(Of String)(pairs.Keys)

                curWord = keys(rnd.Next(keys.Count))
                generatedText += curWord
            Else
                generatedText += " "
                Dim words = pairs(curWord)
                Dim values = New Double(words.Count - 1) {}
                For j = 0 To values.Length - 1
                    values(j) = words(j).num
                Next
                If values.Length <> 0 Then
                    curWord = words(rouletteSelect(values)).str
                Else
                    curWord = getRandomWord()
                End If
                generatedText += curWord
            End If
        Next
        Return generatedText
    End Function

    Private Function getRandomWord() As String
        Dim keys = pairs.Keys.ToArray
        Dim rndkey = keys(rnd.Next(keys.Length))
        Dim words = pairs(rndkey)
        Return words(rnd.Next(words.Count))
    End Function

    Public Function rouletteSelect(weight As Double()) As Integer
        Dim weight_sum As Double = 0
        For i = 0 To weight.Length - 1
            weight_sum += weight(i)
        Next
        Dim value As Double = rnd.NextDouble() * weight_sum
        For i = 0 To weight.Length - 1
            value -= weight(i)
            If value <= 0 Then
                Return i
            End If
        Next
        Return weight.Length - 1
    End Function

    Public Function randomizeNumWords() As Integer
        Return rnd.Next(len.Max - len.Min + 1) + len.Min
    End Function

    Public Sub createTable([string] As String)
        Dim words = [string].Split(" "c)
        For i = 0 To words.Length - 1
            If Not pairs.ContainsKey(words(i)) Then
                pairs(words(i)) = New List(Of Word)()
            End If
            Dim w As New List(Of Word)()
            If pairs.ContainsKey(previous) Then
                w = pairs(previous)
                Dim found = False
                For j = 0 To w.Count - 1
                    If w(j).str = words(i) Then
                        w(j).num += 1
                        found = True
                    End If
                Next
                If Not found Then
                    w.Add(New Word(words(i)))
                End If
            End If

            pairs(previous) = w
            previous = words(i)
        Next
    End Sub
End Class
