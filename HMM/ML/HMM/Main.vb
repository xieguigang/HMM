Imports Microsoft.VisualBasic

Namespace ML.HMM

    Public Class Main
        Public Sub Test()
            Dim jp As Util.Parser.JsonParser = Util.Parser.JsonParser.LoadJson("G:\Github Repositories\Hidden-Markov-Model\Resources\test_HMM.json")
            Dim name As String = DA.Processing.DataDecoding.Instance.getModelName(jp.modelInfo.name)
            Dim states As List(Of String) = DA.Processing.DataDecoding.Instance.getStates(jp.modelData.states)
            Dim observations As List(Of String) = DA.Processing.DataDecoding.Instance.getObservations(jp.modelData.observations)
            Dim initialProbabilities As Dictionary(Of String, Double) = DA.Processing.DataDecoding.Instance.getInitialProbabilities(jp.modelData.initial_prop)
            Dim transitionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double) = DA.Processing.DataDecoding.Instance.getTransitionMatrix(jp.modelData.transition_matrix)
            Dim emissionMatrix As Dictionary(Of KeyValuePair(Of String, String), Double) = DA.Processing.DataDecoding.Instance.getEmissionMatrix(jp.modelData.emission_matrix)

            Dim hmm As New HiddenMarkovModel(name, states, observations, initialProbabilities, transitionMatrix, emissionMatrix)
            Dim sampleStates = New List(Of String)
            sampleStates.add("R")
            sampleStates.add("S")
            sampleStates.add("R")
            sampleStates.add("R")
            sampleStates.add("S")
            sampleStates.add("R")
            sampleStates.add("R")
            sampleStates.add("S")
            sampleStates.add("R")
            sampleStates.add("R")
            sampleStates.add("S")
            sampleStates.add("R")

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

    End Class

End Namespace