Imports System
Imports Microsoft.VisualBasic

''' <summary>
''' 
''' </summary>
Public Module DataDecoding

    Const OUTER_SPLITTER As String = ", "
    Const INNER_SPLITTER As String = "->"

    ''' <summary>
    ''' Get the HMM model name that is written in the Json file </summary>
    ''' <param name="nameInJson"> A string that hold the json expression of the model name </param>
    ''' <returns> A String that is the name of the model </returns>

    Public Function getModelName(ByVal nameInJson As String) As String
        Return nameInJson
    End Function

    ''' <summary>
    ''' Get the HMM model creation date that is written in the Json file </summary>
    ''' <param name="dateInJson"> A string that hold the json expression of the model creation date </param>
    ''' <returns> A String that is the creation date of the model </returns>

    Public Function getModelCreationDate(ByVal dateInJson As String) As String
        Return dateInJson
    End Function

    ''' <summary>
    ''' Get the purpose of the model creation, such as "Weather prediction" </summary>
    ''' <param name="purposeInJson"> A string that hold the json expression of the model purpose </param>
    ''' <returns> A String that is the purpose of the model </returns>

    Public Function getModelCreationPurpose(ByVal purposeInJson As String) As String
        Return purposeInJson
    End Function

    ''' <summary>
    ''' Get the model states </summary>
    ''' <param name="statesInJson"> A string that hold the json expression of the model states </param>
    ''' <returns> A Vector that is the states of the model </returns>

    Public Function getStates(ByVal statesInJson As String) As List(Of String)
        Dim ___states As New List(Of String)
        Dim statesArray As String() = StringSplit(statesInJson, OUTER_SPLITTER, True)

        ___states.AddRange(statesArray)

        Return ___states
    End Function

    ''' <summary>
    ''' Get the initial probabilities of the states </summary>
    ''' <param name="initialProbabilitiesInJson"> A string that hold the json expression of the model initial probabilities </param>
    ''' <returns> A Hashtable that is the initial probabilities of the model states </returns>

    Public Function getInitialProbabilities(ByVal initialProbabilitiesInJson As String) As Dictionary(Of String, Double)
        Dim ___initialProbabilities As New Dictionary(Of String, Double)

        Dim initialProb As String() = StringSplit(initialProbabilitiesInJson, OUTER_SPLITTER, True)

        For Each expression As String In initialProb
            Dim tempExpression As String() = StringSplit(expression, INNER_SPLITTER, True)

            For i As Integer = 0 To tempExpression.Length - 1 Step 2
                ___initialProbabilities(tempExpression(i)) = Convert.ToDouble(tempExpression(i + 1))
            Next
        Next

        Return ___initialProbabilities
    End Function

    ''' <summary>
    ''' Get the observations of the model </summary>
    ''' <param name="observationsInJson"> A string that hold the json expression of the model observations </param>
    ''' <returns> A Hashtable that is the observations of the model </returns>

    Public Function getObservations(ByVal observationsInJson As String) As List(Of String)
        Dim ___observations As New List(Of String)
        Dim expressionArray As String() = StringSplit(observationsInJson, OUTER_SPLITTER, True)

        ___observations.AddRange(expressionArray)

        Return ___observations
    End Function

    ''' <summary>
    ''' Get the transition matrix of the model </summary>
    ''' <param name="transitionMatrixInJson"> A string that hold the json expression of the model transition matrix </param>
    ''' <returns> A Hasshtable that is the transition matrix of the model </returns>

    Public Function getTransitionMatrix(ByVal transitionMatrixInJson As String) As Dictionary(Of KeyValuePair(Of String, String), Double)
        Dim ___transitionMatrix As New Dictionary(Of KeyValuePair(Of String, String), Double)
        Dim tempExpressionArray As String() = StringSplit(transitionMatrixInJson, OUTER_SPLITTER, True)

        For Each expression As String In tempExpressionArray
            Dim transitionExpression As String() = StringSplit(expression, INNER_SPLITTER, True)

            For i As Integer = 0 To transitionExpression.Length - 1 Step 3
                ___transitionMatrix(New KeyValuePair(Of String, String)(transitionExpression(i), transitionExpression(i + 1))) = Convert.ToDouble(transitionExpression(i + 2))
            Next
        Next

        Return ___transitionMatrix
    End Function

    ''' <summary>
    ''' Get the emission matrix </summary>
    ''' <param name="emissionMatrixInJson"> A string that hold the json expression of the model emission matrix </param>
    ''' <returns> A Hasshtable that is the emission matrix of the model </returns>

    Public Function getEmissionMatrix(ByVal emissionMatrixInJson As String) As Dictionary(Of KeyValuePair(Of String, String), Double)
        Dim ___emissionMatrix As New Dictionary(Of KeyValuePair(Of String, String), Double)
        Dim tempExpressionArray As String() = StringSplit(emissionMatrixInJson, OUTER_SPLITTER, True)

        For Each expression As String In tempExpressionArray
            Dim emissionExpression As String() = StringSplit(expression, INNER_SPLITTER, True)

            For i As Integer = 0 To emissionExpression.Length - 1 Step 3
                ___emissionMatrix(New KeyValuePair(Of String, String)(emissionExpression(i), emissionExpression(i + 1))) = Convert.ToDouble(emissionExpression(i + 2))
            Next
        Next

        Return ___emissionMatrix
    End Function
End Module