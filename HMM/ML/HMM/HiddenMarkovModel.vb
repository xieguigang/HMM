Imports Microsoft.VisualBasic
Imports System

Namespace ML.HMM

    Public Class HiddenMarkovModel

        ''' <summary>
        ''' A constructor that initialize the class attributes </summary>
        ''' <param name="states"> A Vector that is the states of the model </param>
        ''' <param name="observations">  A Vector that is the observations of the model </param>
        ''' <param name="initialProbabilities"> A Hashtable that is the initial probability vector of the states </param>
        ''' <param name="transitionMatrix"> A Hashtable the transition matrix between the states </param>
        ''' <param name="emissionMatrix"> A Hashtable that is the emission matrix between the states and the observations </param>

        Public Sub New(ByVal name As String, ByVal states As List(Of String), ByVal observations As List(Of String), ByVal initialProbabilities As Dictionary(Of String, Double), ByVal transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), ByVal emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double))
            Me.Name = name
            Me.States = states
            Me.NumberOfStates = states.Count
            Me.Observations = observations
            Me.NumberOfObservations = observations.Count

            Me.InitialProbabilities = initialProbabilities
            If Not Me.validateInitialProbability(initialProbabilities) Then Throw New Exception("Initial Probabilities sum must be equal 1.0")
            If Not Me.validateInitialProbabilitiesAndStates(states, initialProbabilities) Then Throw New Exception("States size and Initial Probabilities size must be equal")

            Me.TransitionMatrix = transitionMatrix
            If Not Me.validateTransitionMatrix(transitionMatrix, states) Then Throw New Exception("Check the transition matrix elements")

            Me.EmissionMatrix = emissionMatrix
            If Not Me.validateEmissionMatrix(emissionMatrix, states, observations) Then Throw New Exception("Check the emission matrix elements")

            Me.Alpha = New List(Of Dictionary(Of String, Double))
            Me.Beta = New List(Of Dictionary(Of String, Double))
        End Sub

        ''' 
        ''' <param name="initialProbabilities"> A hashtable that is the initial probability vector of the states </param>
        ''' <returns> [True/False] which specifies if the vector elements are logically right or not </returns>

        Private Function validateInitialProbability(ByVal initialProbabilities As Dictionary(Of String, Double)) As Boolean
            Return Util.Validation.Validator.Instance.summationIsOne(initialProbabilities)
        End Function

        ''' 
        ''' <param name="states"> A Vector<String> that is the states of the model </param>
        ''' <param name="initialProbabilities"> A hashtable that is the initial probability vector of the states </param>
        ''' <returns> [True/False] which specifies if the sizes are matched or not </returns>

        Private Function validateInitialProbabilitiesAndStates(ByVal states As List(Of String), ByVal initialProbabilities As Dictionary(Of String, Double)) As Boolean
            Return Util.Validation.Validator.Instance.isValidInitialProbabilities(states, initialProbabilities)
        End Function

        ''' 
        ''' <param name="transitionMatrix"> A Hashtable that is the transition matrix between the states </param>
        ''' <param name="states"> A Vector that is the states of the model </param>
        ''' <returns> [True/False] which specifies if the matrix elements are logically right or not </returns>

        Private Function validateTransitionMatrix(ByVal transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), ByVal states As List(Of String)) As Boolean
            Return Util.Validation.Validator.Instance.isValidTransitionMatrix(transitionMatrix, states)
        End Function

        ''' 
        ''' <param name="emissionMatrix"> A Hashtable that is the emission matrix between the states and the observations </param>
        ''' <param name="states"> A Vector that is the states of the model </param>
        ''' <param name="observations"> A Vector that is the model observations </param>
        ''' <returns> [True/False] True/False which specifies if the matrix elements are logically right or not </returns>

        Private Function validateEmissionMatrix(ByVal emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), ByVal states As List(Of String), ByVal observations As List(Of String)) As Boolean
            Return Util.Validation.Validator.Instance.isValidEmissionMatrix(emissionMatrix, states, observations)
        End Function

        ''' <summary>
        ''' Get the model name </summary>
        ''' <returns> A String that is the model </returns>
        Public Overridable Property Name As String

        ''' <summary>
        ''' Get the number of states in the model </summary>
        ''' <returns> An integer that specifies the number of states in the model </returns>

        Public Overridable Property NumberOfStates As Integer

        ''' <summary>
        ''' Get the model states </summary>
        ''' <returns> A Vector which is the states of the model </returns>

        Public Overridable ReadOnly Property States As List(Of String)

        ''' <summary>
        ''' Get the number of observations in the model </summary>
        ''' <returns> An integer that specifies the number of observations in the model </returns>

        Public Overridable Property NumberOfObservations As Integer

        ''' <summary>
        ''' Get the model observations </summary>
        ''' <returns> A Vector which is the observations of the model </returns>
        Public Overridable ReadOnly Property Observations As List(Of String)

        ''' <summary>
        ''' Get the initial probability vector of the states </summary>
        ''' <returns> Hashtable that is the initial probability vector of the states </returns>

        Public Overridable Property InitialProbabilities As Dictionary(Of String, Double)

        ''' <summary>
        ''' Get the transition matrix between the states </summary>
        ''' <returns> Hashtable that is the transition matrix between the states </returns>

        Public Overridable Property TransitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double)

        ''' <summary>
        ''' Get the emission matrix between the states and the observations </summary>
        ''' <returns> Hashtable that is the emission matrix between the states and the observations </returns>

        Public Overridable Property EmissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double)

        ''' 
        ''' <param name="firstState"> A string that is a state in the model </param>
        ''' <param name="secondState"> A string that is a state in the model </param>
        ''' <returns> A Double that is the transition value between the 2 states </returns>

        Public Overridable Function getTransitionValue(ByVal firstState As String, ByVal secondState As String) As Double
            Return Me.TransitionMatrix(New KeyValuePair(Of String, String)(firstState, secondState))
        End Function

        ''' 
        ''' <param name="state"> A string that is a state in the model </param>
        ''' <param name="observation"> A string that is an observation in the model </param>
        ''' <returns> A Double that is the value of the emission between the state and the observation </returns>

        Public Overridable Function getEmissionValue(ByVal state As String, ByVal observation As String) As Double
            Return Me.EmissionMatrix(New KeyValuePair(Of String, String)(state, observation))
        End Function

        ''' 
        ''' <param name="state"> A string that is a state in the model </param>
        ''' <returns> A Double that is the initial probability value of the state </returns>

        Public Overridable Function getInitialProbability(ByVal state As String) As Double
            Return Me.InitialProbabilities(state)
        End Function

        ''' <summary>
        ''' Get the Alpha values which is obtained from the forward function </summary>
        ''' <returns> A Hashtable which represents the Alpha values </returns>

        Public Overridable ReadOnly Property Alpha As List(Of Dictionary(Of String, Double))

        ''' <summary>
        ''' Get the Beta values which is obtained from the backward function </summary>
        ''' <returns> A Hashtable which represents the Beta values </returns>

        Public Overridable ReadOnly Property Beta As List(Of Dictionary(Of String, Double))

        ''' <summary>
        ''' Calculate the probability to obtain this sequence of states and observations which is the Evaluation of the model </summary>
        ''' <param name="states"> A Vector which is the sequence of model states </param>
        ''' <param name="observations"> A Vector which is the sequence of the model observations </param>
        ''' <returns> A Double The probability to get this sequence of states and observations </returns>
        ''' <exception cref="Exception"> The sizes of states and observations sequences must be the same. </exception>

        Public Overridable Function evaluateUsingBruteForce(ByVal states As List(Of String), ByVal observations As List(Of String)) As Double
            If states.Count <> observations.Count Then Throw New Exception("States and Observations must be at a same size!")

            Dim previousState As String = ""
            Dim probability As Double = 0.0
            Dim result As Double = 0.0

            For i As Integer = 0 To states.Count - 1
                probability = Me.getInitialProbability(states(i))
                previousState = ""
                For j As Integer = 0 To observations.Count - 1
                    Dim ___emissionValue As Double = Me.getEmissionValue(states(j), observations(j))
                    Dim ___transitionValue As Double = 0.0
                    If j <> 0 Then
                        ___transitionValue += Me.getTransitionValue(previousState, states(j))
                        probability *= ___transitionValue * ___emissionValue
                    End If
                    previousState = states(j)
                Next
                result += probability
            Next

            Return result
        End Function

        Public Overridable Function evaluateUsingForwardAlgorithm(ByVal states As List(Of String), ByVal observations As List(Of String)) As Double
            Me._Alpha = Me.calculateForwardProbabilities(states, observations)
            Dim res As Double = 0.0

            For i As Integer = 0 To Me.Alpha(observations.Count - 1).Count - 1
                res += Me.Alpha(observations.Count - 1)(states(i))
            Next

            Return res
        End Function

        ''' <summary>
        ''' Calculate the probability to obtain this sequence of states and observations which is the Evaluation of the model </summary>
        ''' <param name="states"> A Vector which is the sequence of model states </param>
        ''' <param name="observations"> A Vector which is the sequence of the model observations </param>
        ''' <returns> A Double The probability to get this sequence of states and observations </returns>
        ''' <exception cref="Exception"> The sizes of states and observations sequences must be the same. </exception>

        Public Overridable Function evaluateUsingForward_Backward(ByVal states As List(Of String), ByVal observations As List(Of String)) As List(Of Double)
            Dim resultsVector As New List(Of Double)

            Me._Alpha = Me.calculateForwardProbabilities(states, observations)
            'alpha = StatisticalOperations.getInstance().normalize(alpha, states); // Normalization
            Me._Beta = Me.calculateBackwardProbabilities(states, observations)
            ' beta = StatisticalOperations.getInstance().normalize(beta, states); // Normalization

            For t As Integer = 0 To states.Count - 1
                Dim result As Double = 1.0
                For i As Integer = 0 To Me.Alpha.Count - 1
                    result += (Me.Alpha(t)(states(i)) * Me.Beta(t)(states(i)))
                Next
                resultsVector.Add(result)
            Next

            resultsVector = SA.Statistics.StatisticalOperations.Instance.normalize(resultsVector)

            Return resultsVector
        End Function

        ''' <summary>
        ''' Calculate the forward probabilities Alpha as a part of Forward-Backward algorithm https://en.wikipedia.org/wiki/Forward%E2%80%93backward_algorithm </summary>
        ''' <param name="states"> A Vector that is the model states </param>
        ''' <param name="observations"> A Vector that represents the observations sequence </param>
        ''' <returns> A Vector which contains the alpha values </returns>

        Public Overridable Function calculateForwardProbabilities(ByVal states As List(Of String), ByVal observations As List(Of String)) As List(Of Dictionary(Of String, Double))
            Me.Alpha.Add(New Dictionary(Of String, Double))
            For i As Integer = 0 To states.Count - 1
                Me.Alpha(0)(states(i)) = Me.getInitialProbability(states(i)) * Me.getEmissionValue(states(i), observations(0))
            Next

            For t As Integer = 1 To observations.Count - 1
                Me.Alpha.Add(New Dictionary(Of String, Double))
                For i As Integer = 0 To states.Count - 1
                    Dim probability As Double = 0.0
                    For j As Integer = 0 To states.Count - 1
                        probability += Me.Alpha(t - 1)(states(j)) * Me.getTransitionValue(states(j), states(i))
                    Next
                    Me.Alpha(t)(states(i)) = probability * Me.getEmissionValue(states(i), observations(t))
                Next
            Next

            Return Me.Alpha
        End Function

        ''' <summary>
        ''' Calculate the backward probabilities Beta as a part of Forward-Backward algorithm https://en.wikipedia.org/wiki/Forward%E2%80%93backward_algorithm </summary>
        ''' <param name="states"> A Vector that is the model states </param>
        ''' <param name="observations"> A Vector that represents the observations sequence </param>
        ''' <returns> A Vector which contains the Beta values </returns>

        Private Function calculateBackwardProbabilities(ByVal states As List(Of String), ByVal observations As List(Of String)) As List(Of Dictionary(Of String, Double))
            Me._Beta = New List(Of Dictionary(Of String, Double))
            Me.Beta.Add(New Dictionary(Of String, Double))

            For i As Integer = 0 To states.Count - 1
                Me.Beta(0)(states(i)) = 1.0
            Next

            For t As Integer = observations.Count - 2 To 0 Step -1
                Me.Beta.Insert(0, New Dictionary(Of String, Double))
                For i As Integer = 0 To states.Count - 1
                    Dim probability As Double = 0.0
                    For j As Integer = 0 To states.Count - 1
                        probability += Me.Beta(1)(states(j)) * Me.getTransitionValue(states(i), states(j)) * Me.getEmissionValue(states(j), observations(t))
                    Next
                    Me.Beta(0)(states(i)) = probability
                Next
            Next

            Return Me.Beta
        End Function

        ''' <summary>
        ''' Get the most optimal path for states to emit the given observations </summary>
        ''' <param name="states"> A Vector which is the model states </param>
        ''' <param name="observations"> A Vector which represents the observations </param>
        ''' <returns> A String which holds the optimal path and the total cost </returns>

        Public Overridable Function getOptimalStateSequenceUsingViterbiAlgorithm(ByVal states As List(Of String), ByVal observations As List(Of String)) As String
            Dim path As String = ""
            Dim dpTable As New List(Of Dictionary(Of String, Double))
            Dim statesProbabilities As New Dictionary(Of String, Double)
            Dim priorProbabilities As New Dictionary(Of String, Double)

            For i As Integer = 0 To observations.Count - 1
                If i = 0 Then
                    For Each state As String In states
                        Dim ___initialProbability As Double = Me.getInitialProbability(state)
                        Dim emissionProbability As Double = Me.getEmissionValue(state, observations(i))
                        statesProbabilities(state) = Math.Log(___initialProbability) + Math.Log(emissionProbability)
                    Next
                Else
                    For Each state As String In states
                        Dim emissionProbability As Double = Me.getEmissionValue(state, observations(i))
                        Dim bestProbability As Double = -100000

                        For Each prevState As String In priorProbabilities.Keys
                            Dim transitionProbability As Double = Me.getTransitionValue(prevState, state)
                            Dim accumulate As Double = priorProbabilities(prevState) + Math.Log(emissionProbability) + Math.Log(transitionProbability)

                            If accumulate > bestProbability Then bestProbability = accumulate
                        Next
                        statesProbabilities(state) = bestProbability
                    Next
                End If

                dpTable.Add(CType(statesProbabilities.Clone(), Dictionary(Of String, Double)))
                priorProbabilities = CType(statesProbabilities.Clone(), Dictionary(Of String, Double))
            Next

            Dim lastColumn As Dictionary(Of String, Double) = dpTable(dpTable.Count - 1)
            Dim totalCost As Double = -1000000

            For Each item As String In lastColumn.Keys
                If lastColumn(item) > totalCost Then totalCost = lastColumn(item)
            Next

            For Each column As Dictionary(Of String, Double) In dpTable
                Dim costPerColumn As Double = -1000000
                Dim targetState As String = ""
                For Each state As String In column.Keys
                    If column(state) > costPerColumn Then
                        costPerColumn = column(state)
                        targetState = state
                    End If
                Next
                path += targetState & " -> "
            Next

            path &= "END with total cost = " & totalCost

            Return path
        End Function

        ''' <summary>
        ''' Estimate the parameters of HMM which known as the learning approach for HMM </summary>
        ''' <param name="states"> A Vector which is the model states </param>
        ''' <param name="observations"> A Vector which is the sequence of observations </param>
        ''' <param name="additiveSmoothing"> A boolean which indicates if the function will use the smoothing value or not to avoid zero values. </param>

        Public Overridable Sub estimateParametersUsingBaumWelchAlgorithm(ByVal states As List(Of String), ByVal observations As List(Of String), ByVal additiveSmoothing As Boolean)
            Dim smoothing As Double = If(additiveSmoothing, 1.0, 0.0)
            Me._Alpha = Me.calculateForwardProbabilities(states, observations)
            Me._Beta = Me.calculateBackwardProbabilities(states, observations)
            Dim gamma As New List(Of Dictionary(Of String, Double))

            For i As Integer = 0 To observations.Count - 1
                gamma.Add(New Dictionary(Of String, Double))
                Dim probabilitySum As Double = 0.0
                For Each state As String In states
                    Dim product As Double = Me.Alpha(i)(state) * Me.Beta(i)(state)
                    gamma(i)(state) = product
                    probabilitySum += product
                Next
                If probabilitySum = 0 Then Continue For

                For Each state As String In states
                    gamma(i)(state) = gamma(i)(state) / probabilitySum
                Next
            Next

            Dim eps As New List(Of Dictionary(Of String, Dictionary(Of String, Double)))

            For i As Integer = 0 To observations.Count - 2
                Dim probabilitySum As Double = 0.0
                eps.Add(New Dictionary(Of String, Dictionary(Of String, Double)))
                For Each fromState As String In states
                    eps(i)(fromState) = New Dictionary(Of String, Double)
                    For Each toState As String In states
                        Dim tempProbability As Double = Me.Alpha(i)(fromState) * Me.Beta(i + 1)(toState) * Me.getTransitionValue(fromState, toState) * Me.getEmissionValue(toState, observations(i + 1))

                        eps(i)(fromState)(toState) = tempProbability
                        probabilitySum += tempProbability
                    Next
                Next

                If probabilitySum = 0 Then Continue For

                For Each [from] As String In states
                    For Each [to] As String In states
                        eps(i)(from)([to]) = eps(i)(from)([to]) / probabilitySum
                    Next
                Next
            Next

            For Each state As String In states
                Dim updated As Double = (gamma(0)(state) + smoothing) / (1 + (states.Count * smoothing))
                Me.InitialProbabilities(state) = updated

                Dim gammaProbabilitySum As Double = 0.0
                For i As Integer = 0 To observations.Count - 2
                    gammaProbabilitySum += gamma(i)(state)
                Next

                If gammaProbabilitySum > 0 Then
                    Dim denominator As Double = gammaProbabilitySum + smoothing * states.Count
                    For Each [to] As String In states
                        Dim epsSum As Double = 0.0
                        For i As Integer = 0 To observations.Count - 2
                            epsSum += eps(i)(state)([to])
                            Me.TransitionMatrix(New KeyValuePair(Of String, String)(state, [to])) = (smoothing + epsSum) / denominator
                        Next
                    Next
                Else
                    For Each [to] As String In states
                        Me.TransitionMatrix(New KeyValuePair(Of String, String)(state, [to])) = 0.0
                    Next
                End If

                gammaProbabilitySum = 0.0

                For i As Integer = 0 To observations.Count - 1
                    gammaProbabilitySum += gamma(i)(state)
                Next

                Dim emissionProbabilitySums As New Dictionary(Of String, Double)

                For Each observation As String In Me.Observations
                    emissionProbabilitySums(observation) = 0.0
                Next

                For i As Integer = 0 To observations.Count - 1
                    emissionProbabilitySums(observations(i)) = emissionProbabilitySums(observations(i)) + gamma(i)(state)
                Next

                If gammaProbabilitySum > 0 Then
                    Dim denominator As Double = gammaProbabilitySum + smoothing * observations.Count
                    For Each observation As String In Me.Observations
                        Me.EmissionMatrix(New KeyValuePair(Of String, String)(state, observation)) = (smoothing + emissionProbabilitySums(observation)) / denominator
                    Next
                Else
                    For Each observation As String In Me.Observations
                        Me.EmissionMatrix(New KeyValuePair(Of String, String)(state, observation)) = 0.0
                    Next
                End If
            Next
        End Sub
    End Class
End Namespace