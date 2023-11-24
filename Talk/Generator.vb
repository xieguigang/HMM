Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports rnd = Microsoft.VisualBasic.Math.RandomExtensions

Public Class Generator

    Dim previous, curWord As String

    ReadOnly len As IntRange
    ReadOnly corpora As Corpora

    Public Sub New(len As IntRange, corpora As Corpora)
        Me.len = len
        Me.corpora = corpora
    End Sub

    Public Function Generate() As String
        Dim generatedText = ""
        Dim numWords As Integer = randomizeNumWords()

        For i As Integer = 0 To numWords - 1
            If generatedText.StringEmpty Then
                Dim keys As New List(Of String)(corpora.Keys)

                curWord = keys(rnd.Next(keys.Count))
                generatedText += curWord
            Else
                Dim words = pairs(curWord)
                Dim values = New Double(words.Count - 1) {}

                generatedText += " "

                For j As Integer = 0 To values.Length - 1
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
End Class
