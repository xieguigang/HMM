Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Module HMMTextGenerator

    ' 状态类
    Private Class State

        Public TransitionProbabilities As New Dictionary(Of State, Double)
        Public EmissionProbabilities As New Dictionary(Of String, Double)
        Public EmissionTransition As New Dictionary(Of String, Dictionary(Of State, Double))

        Public Name As String

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Function SumNormalize() As State
            Dim transition As Double = TransitionProbabilities.Values.Sum
            Dim transitionMatrix = TransitionProbabilities.OrderByDescending(Function(a) a.Value).ToDictionary(Function(a) a.Key, Function(a) a.Value / transition)
            Dim emission As Double = EmissionProbabilities.Values.Sum
            Dim emissionMatrix = EmissionProbabilities.OrderByDescending(Function(a) a.Value).ToDictionary(Function(a) a.Key, Function(a) a.Value / emission)
            Dim transition2 As New Dictionary(Of String, Dictionary(Of State, Double))

            For Each key As String In emissionMatrix.Keys
                Dim sum = EmissionTransition(key).Values.Sum
                EmissionTransition(key) = EmissionTransition(key).OrderByDescending(Function(a) a.Value).ToDictionary(Function(a) a.Key, Function(a) a.Value / sum)
            Next

            TransitionProbabilities = transitionMatrix
            EmissionProbabilities = emissionMatrix

            Return Me
        End Function

        Public Shared Sub Push(ByRef state As State, transitNext As String, pool As Dictionary(Of String, State), obs As String)
            state.TransitionProbabilities(pool(transitNext)) += 1

            If Not state.EmissionProbabilities.ContainsKey(obs) Then
                state.EmissionProbabilities(obs) = 1
                state.EmissionTransition.Add(obs, New Dictionary(Of State, Double) From {{pool(transitNext), 1}})
            Else
                state.EmissionProbabilities(obs) += 1

                Dim [set] = state.EmissionTransition(obs)

                If Not [set].ContainsKey(pool(transitNext)) Then
                    [set](pool(transitNext)) = 1
                Else
                    [set](pool(transitNext)) += 1
                End If
            End If
        End Sub
    End Class

    ' 隐马尔可夫模型类
    Private Class HiddenMarkovModel
        Public States As List(Of State)
        Public InitialState As State

        Public Sub New(states As IEnumerable(Of State))
            Me.States = New List(Of State)(From s In states Select s.SumNormalize)
            Me.InitialState = Me.States(0)
        End Sub

        '' 训练模型（这里只是示例，没有实际训练过程）
        'Public Sub Train()
        '    ' 假设我们有三个状态
        '    Dim state1 As New State()
        '    Dim state2 As New State()
        '    Dim state3 As New State()

        '    ' 设置初始状态
        '    InitialState = state1

        '    ' 设置转移概率
        '    state1.TransitionProbabilities.Add(state2, 0.5)
        '    state1.TransitionProbabilities.Add(state3, 0.5)
        '    state2.TransitionProbabilities.Add(state1, 0.4)
        '    state2.TransitionProbabilities.Add(state3, 0.6)
        '    state3.TransitionProbabilities.Add(state1, 0.3)
        '    state3.TransitionProbabilities.Add(state2, 0.7)

        '    ' 设置发射概率
        '    state1.EmissionProbabilities.Add("a", 0.7)
        '    state1.EmissionProbabilities.Add("b", 0.3)
        '    state2.EmissionProbabilities.Add("a", 0.4)
        '    state2.EmissionProbabilities.Add("b", 0.6)
        '    state3.EmissionProbabilities.Add("a", 0.5)
        '    state3.EmissionProbabilities.Add("b", 0.5)

        '    ' 添加状态到模型
        '    States.Add(state1)
        '    States.Add(state2)
        '    States.Add(state3)
        'End Sub

        ' 生成文本
        Public Iterator Function GenerateText(length As Integer) As IEnumerable(Of String)
            Dim currentState As State = InitialState

            For i As Integer = 1 To length
                ' 选择发射
                Dim observation As String = ChooseObservation(currentState.EmissionProbabilities)

                Yield observation

                Dim trans = currentState.EmissionTransition(observation)

                ' 选择下一个状态
                currentState = ChooseNextState(trans)
            Next
        End Function

        ' 根据概率选择观测
        Private Function ChooseObservation(probabilities As Dictionary(Of String, Double)) As String
            Dim randomValue As Double = randf.NextDouble()
            Dim cumulativeProbability As Double = 0.0

            For Each pair In probabilities
                cumulativeProbability += pair.Value
                If randomValue < cumulativeProbability Then
                    Return pair.Key
                End If
            Next

            Return ""
        End Function

        ' 根据概率选择下一个状态
        Private Function ChooseNextState(probabilities As Dictionary(Of State, Double)) As State
            Dim randomValue As Double = randf.NextDouble()
            Dim cumulativeProbability As Double = 0.0

            For Each pair In probabilities
                cumulativeProbability += pair.Value
                If randomValue < cumulativeProbability Then
                    Return pair.Key
                End If
            Next

            Return Nothing
        End Function
    End Class

    Sub Main33()
        Dim observations As New List(Of String)
        Dim transitionMatrix As New Dictionary(Of String, State)
        Dim stateNames As New List(Of String) From {
            "Begin",
            "StatementBegining", "StatementFirstHalf", "StatementSecondHalf", "StatementEnd", "StatementScalar", ' .
            "QuestionsBegining", "QuestionsFirstHalf", "QuestionsSecondHalf", "QuestionsEnd", "QuestionsScalar", ' ?
            "ExclamationBegining", "ExclamationFirstHalf", "ExclamationSecondHalf", "ExclamationEnd", "ExclamationScalar", ' !
            "PauseBegining", "PauseFirstHalf", "PauseSecondHalf", "PauseEnd", "PauseScalar" ' ,
        }

        For Each name As String In stateNames
            transitionMatrix(name) = New State With {.Name = name}
        Next

        For Each name As String In stateNames
            Dim current = transitionMatrix(name)

            For Each item In transitionMatrix.Values
                ' If item IsNot current Then
                current.TransitionProbabilities.Add(item, 0)
                ' End If
            Next
        Next

        For Each par As String In "G:\HMM\test\abstract.txt".ReadAllLines
            If par.StringEmpty(, True) Then
                Continue For
            End If

            If Not par.Last.ToString.IsPattern("[,.?!]") Then
                par = par & "."
            End If

            Dim parts = par.Matches(".*?[,.!?]").ToArray
            Dim stat As String = "Begin"
            Dim last As String

            For Each line As String In parts
                Dim tokens As String() = line.Trim(" "c, "."c, ","c, "!"c, "?"c).ToLower.Split(" "c, "("c, ")"c, "["c, "]"c)
                Dim type As String

                tokens = tokens.Where(Function(s) Not s.StringEmpty).ToArray

                If tokens.Length = 0 Then
                    Continue For
                Else
                    observations.AddRange(tokens)
                End If

                Select Case line.Last
                    Case "." : type = "Statement"
                    Case "?" : type = "Questions"
                    Case "!" : type = "Exclamation"
                    Case "," : type = "Pause"
                    Case Else
                        Throw New InvalidOperationException
                End Select

                If tokens.Length = 1 Then
                    last = type & "Scalar"
                    State.Push(transitionMatrix(stat), last, transitionMatrix, tokens(0))
                    stat = last
                Else
                    For i As Integer = 0 To tokens.Length - 1
                        If i = 0 Then
                            last = type & "Begining"
                        ElseIf i = tokens.Length - 1 Then
                            last = type & "End"
                        ElseIf i < tokens.Length \ 2 Then
                            last = type & "FirstHalf"
                        Else
                            last = type & "SecondHalf"
                        End If

                        State.Push(transitionMatrix(stat), last, transitionMatrix, tokens(i))
                        stat = last
                        ' emissionMatrix.Plus($"{stat} -> {tokens(i)}")
                    Next
                End If
            Next
        Next

        Dim hmm As New HiddenMarkovModel(transitionMatrix.Values)
        ' hmm.Train()

        For i As Integer = 0 To 100
            Dim generatedText As String = hmm.GenerateText(10).JoinBy(" ")
            Console.WriteLine("Generated Text: " & generatedText)
        Next

        Pause()
    End Sub

End Module
