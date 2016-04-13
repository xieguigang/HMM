Imports System
Imports System.Collections.Generic

Public Class HMM_For_Testing
    Private name As String

    ''' <summary>
    ''' A constructor that initialize the class attributes </summary>
    ''' <param name="states"> A Vector that is the states of the model </param>
    ''' <param name="observations">  A Vector that is the observations of the model </param>
    ''' <param name="initialProbabilities"> A Hashtable that is the initial probability vector of the states </param>
    ''' <param name="transitionMatrix"> A Hashtable the transition matrix between the states </param>
    ''' <param name="emissionMatrix"> A Hashtable that is the emission matrix between the states and the observations </param>

    Public Sub New(name As String, states As List(Of String), observations As List(Of String), initialProbabilities As Dictionary(Of String, Double), transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double))
        Me.name = name
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
    End Sub

    Public Sub New(filepath As String)

    End Sub

    ''' 
    ''' <param name="initialProbabilities"> A hashtable that is the initial probability vector of the states </param>
    ''' <returns> [True/False] which specifies if the vector elements are logically right or not </returns>

    Private Function validateInitialProbability(initialProbabilities As Dictionary(Of String, Double)) As Boolean
        Return Util.Validation.Validator.summationIsOne(initialProbabilities)
    End Function

    ''' 
    ''' <param name="states"> A Vector of <see cref="String"/> that is the states of the model </param>
    ''' <param name="initialProbabilities"> A hashtable that is the initial probability vector of the states </param>
    ''' <returns> [True/False] which specifies if the sizes are matched or not </returns>

    Private Function validateInitialProbabilitiesAndStates(states As List(Of String), initialProbabilities As Dictionary(Of String, Double)) As Boolean
        Return Util.Validation.Validator.isValidInitialProbabilities(states, initialProbabilities)
    End Function

    ''' 
    ''' <param name="transitionMatrix"> A Hashtable that is the transition matrix between the states </param>
    ''' <param name="states"> A Vector that is the states of the model </param>
    ''' <returns> [True/False] which specifies if the matrix elements are logically right or not </returns>

    Private Function validateTransitionMatrix(transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), states As List(Of String)) As Boolean
        Return Util.Validation.Validator.isValidTransitionMatrix(transitionMatrix, states)
    End Function

    ''' 
    ''' <param name="emissionMatrix"> A Hashtable that is the emission matrix between the states and the observations </param>
    ''' <param name="states"> A Vector that is the states of the model </param>
    ''' <param name="observations"> A Vector that is the model observations </param>
    ''' <returns> [True/False] True/False which specifies if the matrix elements are logically right or not </returns>

    Private Function validateEmissionMatrix(emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double), states As List(Of String), observations As List(Of String)) As Boolean
        Return Util.Validation.Validator.isValidEmissionMatrix(emissionMatrix, states, observations)
    End Function

    ''' <summary>
    ''' Get the number of states in the model </summary>
    ''' <returns> An integer that specifies the number of states in the model </returns>

    Public Property NumberOfStates As Integer

    ''' <summary>
    ''' Get the model states </summary>
    ''' <returns> A Vector which is the states of the model </returns>

    Public ReadOnly Property States As List(Of String)

    ''' <summary>
    ''' Get the number of observations in the model </summary>
    ''' <returns> An integer that specifies the number of observations in the model </returns>

    Public Property NumberOfObservations As Integer

    ''' <summary>
    ''' Get the model observations </summary>
    ''' <returns> A Vector which is the observations of the model </returns>
    Public ReadOnly Property Observations As List(Of String)

    ''' <summary>
    ''' Get the initial probability vector of the states </summary>
    ''' <returns> Hashtable that is the initial probability vector of the states </returns>

    Public Property InitialProbabilities As Dictionary(Of String, Double)

    ''' <summary>
    ''' Get the transition matrix between the states </summary>
    ''' <returns> Hashtable that is the transition matrix between the states </returns>

    Public Property TransitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double)

    ''' <summary>
    ''' Get the emission matrix between the states and the observations </summary>
    ''' <returns> Hashtable that is the emission matrix between the states and the observations </returns>

    Public Property EmissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double)

    ''' 
    ''' <param name="firstState"> A string that is a state in the model </param>
    ''' <param name="secondState"> A string that is a state in the model </param>
    ''' <returns> A Double that is the transition value between the 2 states </returns>

    Public Function getTransitionValue(firstState As String, secondState As String) As Double
        Return Me.TransitionMatrix(New KeyValuePair(Of String, String)(firstState, secondState))
    End Function

    ''' 
    ''' <param name="state"> A string that is a state in the model </param>
    ''' <param name="observation"> A string that is an observation in the model </param>
    ''' <returns> A Double that is the value of the emission between the state and the observation </returns>

    Public Function getEmissionValue(state As String, observation As String) As Double
        Return Me.EmissionMatrix(New KeyValuePair(Of String, String)(state, observation))
    End Function

    ''' 
    ''' <param name="state"> A string that is a state in the model </param>
    ''' <returns> A Double that is the initial probability value of the state </returns>

    Public Function getInitialProbability(state As String) As Double
        Return Me.InitialProbabilities(state)
    End Function

    ''' <summary>
    ''' Calculate the probability to obtain this sequence of states and observations which is the Evaluation of the model </summary>
    ''' <param name="states"> A Vector which is the sequence of model states </param>
    ''' <param name="observations"> A Vector which is the sequence of the model observations </param>
    ''' <returns> A Double The probability to get this sequence of states and observations </returns>
    ''' <exception cref="Exception"> The sizes of states and observations sequences must be the same. </exception>

    Public Function evaluateUsingBruteForce(states As List(Of String), observations As List(Of String)) As Double
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

    ''' <summary>
    ''' Calculate the probability to obtain this sequence of states and observations which is the Evaluation of the model </summary>
    ''' <param name="states"> A Vector which is the sequence of model states </param>
    ''' <param name="observations"> A Vector which is the sequence of the model observations </param>
    ''' <returns> A Double The probability to get this sequence of states and observations </returns>
    ''' <exception cref="Exception"> The sizes of states and observations sequences must be the same. </exception>

    Public Function evaluateUsingForward_Backward(states As List(Of String), observations As List(Of String)) As Double
        If observations.Count <> states.Count Then Throw New Exception("States and Observations must be at a same size")

        Dim result As Double = 0.0

        Dim alpha As List(Of Dictionary(Of String, Double)) = Me.calculateForwardProbabilities(states, observations)
        ' alpha = StatisticalOperations.getInstance().normalize(alpha, states);
        Dim beta As List(Of Dictionary(Of String, Double)) = Me.calculateBackwardProbabilities(states, observations)
        'beta = StatisticalOperations.getInstance().normalize(beta, states);

        For t As Integer = 0 To states.Count - 1
            For i As Integer = 0 To alpha.Count - 1
                result += (alpha(t)(states(i)) * beta(t)(states(i)))
            Next
        Next

        Return result
    End Function

    ''' <summary>
    ''' Calculate the forward probabilities Alpha as a part of Forward-Backward algorithm https://en.wikipedia.org/wiki/Forward%E2%80%93backward_algorithm </summary>
    ''' <param name="states"> A Vector that is the model states </param>
    ''' <param name="observations"> A Vector that is the model observations </param>
    ''' <returns> A Vector which contains the alpha values </returns>

    Public Function calculateForwardProbabilities(states As List(Of String), observations As List(Of String)) As List(Of Dictionary(Of String, Double))
        Dim alpha As New List(Of Dictionary(Of String, Double))
        alpha.Add(New Dictionary(Of String, Double))
        Dim sum1 As Double = 0.0
        For i As Integer = 0 To states.Count - 1
            alpha(0)(states(i)) = Me.getInitialProbability(states(i)) * Me.getEmissionValue(states(i), observations(0))
            sum1 += Me.getInitialProbability(states(i)) * Me.getEmissionValue(states(i), observations(0))
        Next

        For i As Integer = 0 To states.Count - 1
            alpha(0)(states(i)) = alpha(0)(states(i)) * (1 / sum1)
        Next

        sum1 = 0.0
        For t As Integer = 1 To states.Count - 1
            alpha.Add(New Dictionary(Of String, Double))
            For i As Integer = 0 To states.Count - 1
                Dim probability As Double = 0.0
                For j As Integer = 0 To states.Count - 1
                    probability += alpha(t - 1)(states(j)) * Me.getTransitionValue(states(j), states(i))
                Next
                alpha(t)(states(i)) = probability * Me.getEmissionValue(states(i), observations(t))
                sum1 += probability * Me.getEmissionValue(states(i), observations(t))
            Next
        Next

        For t As Integer = 1 To states.Count - 1
            For i As Integer = 0 To states.Count - 1
                alpha(t)(states(i)) = alpha(t)(states(i)) * (1 / sum1)
            Next
        Next

        Return alpha
    End Function

    ''' <summary>
    ''' Calculate the backward probabilities Beta as a part of Forward-Backward algorithm https://en.wikipedia.org/wiki/Forward%E2%80%93backward_algorithm </summary>
    ''' <param name="states"> A Vector that is the model states </param>
    ''' <param name="observations"> A Vector that is the model observations </param>
    ''' <returns> A Vector which contains the Beta values </returns>

    Public Function calculateBackwardProbabilities(states As List(Of String), observations As List(Of String)) As List(Of Dictionary(Of String, Double))
        Dim beta As New List(Of Dictionary(Of String, Double))
        beta.Add(New Dictionary(Of String, Double))
        Dim sum1 As Double = 0.0

        For i As Integer = 0 To states.Count - 1
            beta(0)(states(i)) = 1.0
            sum1 += 1.0
        Next

        For i As Integer = 0 To states.Count - 1
            beta(0)(states(i)) = beta(0)(states(i)) * (1 / sum1)
        Next

        sum1 = 0.0

        For t As Integer = states.Count - 2 To 0 Step -1
            beta.Insert(0, New Dictionary(Of String, Double))
            For i As Integer = 0 To states.Count - 1
                Dim probability As Double = 0.0
                For j As Integer = 0 To states.Count - 1
                    probability += beta(1)(states(j)) * Me.getEmissionValue(states(j), observations(t)) * Me.getTransitionValue(states(i), states(j))
                Next
                beta(0)(states(i)) = probability
                sum1 += probability
            Next
        Next

        For t As Integer = states.Count - 2 To 0 Step -1
            For i As Integer = 0 To states.Count - 1
                beta(0)(states(i)) = beta(0)(states(i)) * (1 / sum1)
            Next
        Next

        Return beta
    End Function
End Class