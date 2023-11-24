Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports TalkGenerator

Module Module1

    Sub Main2()

        Dim corpus As New Corpora()
        Dim corpora As String() = {
            "G:\GCModeller\src\runtime\sciBASIC#\Data\TextRank\Rapunzel.txt",
            "G:\GCModeller\src\runtime\sciBASIC#\Data\TextRank\Beauty_and_the_Beast.txt",
            "G:\GCModeller\src\runtime\sciBASIC#\Data\TextRank\Cinderalla.txt",
            "G:\GCModeller\src\runtime\sciBASIC#\Data\Trinity\alice30.txt"
        }

        For Each file As String In corpora
            For Each line As String In file.ReadAllLines
                Call corpus.MakeCorpus(line)
            Next
        Next

        Dim talk As New Generator(New IntRange(3, 12), corpus)

        For i As Integer = 0 To 20
            Call Console.WriteLine(talk.Generate)
        Next

        Pause()
    End Sub
End Module
