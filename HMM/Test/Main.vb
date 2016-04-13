Imports HMM
Imports Microsoft.VisualBasic

Public Module Main
    Public Sub Main()
        Dim jp As Util.Parser.JsonParser = Util.Parser.JsonParser.LoadJson("G:\Github Repositories\Hidden-Markov-Model\Resources\test_HMM.json")
        Dim name As String = DataDecoding.getModelName(jp.modelInfo.name)
        Dim states As List(Of String) = DataDecoding.getStates(jp.modelData.states)
        Dim observations As List(Of String) = DataDecoding.getObservations(jp.modelData.observations)
        Dim initialProbabilities As Dictionary(Of String, Double) = DataDecoding.getInitialProbabilities(jp.modelData.initial_prop)
        Dim transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double) = DataDecoding.getTransitionMatrix(jp.modelData.transition_matrix)
        Dim emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double) = DataDecoding.getEmissionMatrix(jp.modelData.emission_matrix)

        Dim hmm As New HiddenMarkovModel(name, states, observations, initialProbabilities, transitionMatrix, emissionMatrix)
        Dim sampleStates = New List(Of String)
        sampleStates.Add("R")
        sampleStates.Add("S")
        sampleStates.Add("R")
        sampleStates.Add("R")
        sampleStates.Add("S")
        sampleStates.Add("R")
        sampleStates.Add("R")
        sampleStates.Add("S")
        sampleStates.Add("R")
        sampleStates.Add("R")
        sampleStates.Add("S")
        sampleStates.Add("R")

        ' sampleStates.add("R");

        Dim sampleO As New List(Of String)
        sampleO.Add("U")
        sampleO.Add("D")
        sampleO.Add("U")
        sampleO.Add("U")
        sampleO.Add("D")
        sampleO.Add("U")
        sampleO.Add("U")
        sampleO.Add("D")
        sampleO.Add("U")
        sampleO.Add("U")
        sampleO.Add("D")
        sampleO.Add("U")
        'sampleO.add("U");

        'sampleO.add("U");
        'sampleO.add("D");
        '  sampleO.add("U");

        Console.WriteLine(hmm.evaluateUsingBruteForce(sampleStates, sampleO))
        Console.WriteLine(hmm.evaluateUsingForwardAlgorithm(sampleStates, sampleO))
        Console.WriteLine(hmm.getOptimalStateSequenceUsingViterbiAlgorithm(states, sampleO))
        hmm.estimateParametersUsingBaumWelchAlgorithm(states, sampleO, False)
        Console.WriteLine(hmm.InitialProbabilities)
        Console.WriteLine(hmm.TransitionMatrix)
        Console.WriteLine(hmm.EmissionMatrix)
        Console.WriteLine(hmm.evaluateUsingBruteForce(sampleStates, sampleO))
        Console.WriteLine(hmm.evaluateUsingForwardAlgorithm(sampleStates, sampleO))

    End Sub
End Module