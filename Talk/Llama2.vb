Imports System.Diagnostics.CodeAnalysis
Imports System.IO
Imports System.IO.MemoryMappedFiles
Imports System.Runtime.InteropServices
Imports System.Text

'MIT License
'
'Copyright(c) 2023 Rahul TR
'
'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.
'
'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.

' Usage:   run <checkpoint> [options]
' Example: run model.bin -n 256 -i "Once upon a time"
' Options:
'   -t <float>  temperature, default 1.0
'   -p <float>  p value in top-p (nucleus) sampling. default 0.9, 0 = off
'   -s <int>    random seed, default time(NULL)
'   -n <int>    number of steps to run for, default 256. 0 = max_seq_len
'   -i <string> input prompt

''' <summary>
''' 
''' </summary>
Public Module Llama2

    Dim _rngSeed As Long

    Sub New()
        Call SetSeed(CUInt(Date.UtcNow.Ticks))
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="temperature">0.0 = greedy deterministic. 1.0 = original. don't set higher; -t &lt;float>  temperature, default 1.0</param>
    ''' <param name="topp">top-p in nucleus sampling; -p &lt;float>  p value in top-p (nucleus) sampling. default 0.9, 0 = off</param>
    ''' <param name="steps">number of steps to run for; -n &lt;int>    number of steps to run for, default 256. 0 = max_seq_len</param>
    ''' <param name="prompt">prompt string; -i &lt;string> input prompt</param>
    ''' <param name="tokenizer">"tokenizer.bin"</param>
    ''' <param name="seed">-s &lt;int>    random seed, default time(NULL)</param>
    Public Sub Run(checkpoint As String,
                   Optional prompt As String = Nothing,
                   Optional steps As Integer = 256,
                   Optional temperature As Single = 1.0F,
                   Optional topp As Single = 0.9F,
                   Optional seed As Integer? = Nothing,
                   Optional tokenizer As String = Nothing)

        If Not seed Is Nothing Then
            _rngSeed = seed
        End If

        If _rngSeed = 0 Then
            Console.WriteLine("Cannot use seed=0 because of the rng alg used" & vbLf)
            Return
        End If

        ' read in the model.bin file
        Dim config As Config
        Dim weights As TransformerWeights

        Try
            Dim fileStream As FileStream = New FileStream(checkpoint, FileMode.Open, FileAccess.Read)
            ' Read in the config header
            Dim configBytes = New Byte(Marshal.SizeOf(GetType(Config)) - 1) {}
            If fileStream.Read(configBytes, 0, configBytes.Length) <> configBytes.Length Then Environment.Exit(1)

            Dim handle = GCHandle.Alloc(configBytes, GCHandleType.Pinned)
            Try
                Dim pointer As IntPtr = handle.AddrOfPinnedObject()
                config = CType(Marshal.PtrToStructure(pointer, GetType(Config)), Config)
            Finally
                handle.Free()
            End Try

            ' Negative vocab size is a hacky way of signaling unshared weights. Bit yikes.
            Dim sharedWeights = config.vocab_size > 0
            config.vocab_size = Math.Abs(config.vocab_size)

            ' Figure out the file size
            Dim fileSize = fileStream.Length ' size of the checkpoint file in bytes

            Dim memoryMappedFile = MemoryMappedFiles.MemoryMappedFile.CreateFromFile(fileStream, Nothing, fileSize, MemoryMappedFileAccess.Read, HandleInheritability.None, False)
            Dim configSizeInBytes As Long = Marshal.SizeOf(GetType(Config))
            Dim accessor = memoryMappedFile.CreateViewAccessor(configSizeInBytes, fileSize - configSizeInBytes, MemoryMappedFileAccess.Read)
            weights = New TransformerWeights()

            CheckpointInitWeights(weights, config, accessor, sharedWeights)
        Catch __unusedFileNotFoundException1__ As FileNotFoundException
            Console.Error.WriteLine($"Couldn't open file {checkpoint}")
            Return
        Catch e As Exception
            Console.Error.WriteLine($"Couldn't read {checkpoint}: {e.Message}")
            Return
        End Try

        ' right now we cannot run for more than config.seq_len steps
        If steps <= 0 OrElse steps > config.seq_len Then
            steps = config.seq_len
        End If

        ' read in the tokenizer.bin file
        Dim vocab = New String(config.vocab_size - 1) {}
        Dim vocabScores = New Single(config.vocab_size - 1) {}
        Dim maxTokenLength As Integer

        Using fs As FileStream = New FileStream(tokenizer, FileMode.Open, FileAccess.Read)
            Using reader As BinaryReader = New BinaryReader(fs)
                Try
                    maxTokenLength = reader.ReadInt32()

                    For i = 0 To config.vocab_size - 1
                        vocabScores(i) = reader.ReadSingle()

                        Dim len As Integer = reader.ReadInt32()
                        Dim buffer = New Byte(len - 1) {} ' stack allocate buffer, assumes len is small

                        reader.Read(buffer)
                        vocab(i) = Encoding.UTF8.GetString(buffer)
                    Next
                Catch __unusedEndOfStreamException1__ As EndOfStreamException
                    Console.Error.WriteLine("failed read")
                    Return
                End Try
            End Using
        End Using


        ' create and init the application RunState
        Dim state = InitializeRunState(config)

        ' process the prompt, if any
        Dim promptTokens As Integer() = Nothing
        Dim numPromptTokens = 0
        If Not String.IsNullOrEmpty(prompt) Then
            promptTokens = New Integer(prompt.Length - 1) {}
            Llama2.BpeEncode(prompt, vocab, vocabScores, config.vocab_size, maxTokenLength, promptTokens, numPromptTokens)
        End If


        ' start the main loop
        Dim token = 1 ' init with token 1 (=BOS), as done in Llama-2 sentencepiece tokenizer
        Dim pos = 0 ' position in the sequence
        Dim timer As Stopwatch = New Stopwatch()
        timer.Start()

        While pos < steps
            ' forward the transformer to get logits for the next token
            Transformer(token, pos, config, state, weights)

            ' advance the state state machine
            Dim [next] As Integer ' will store the next token in the sequence
            If pos < numPromptTokens Then
                ' if we are still processing the input prompt, force the next prompt token
                [next] = promptTokens(pos)
            Else
                ' sample the next token
                If temperature = 0.0F Then
                    ' greedy argmax sampling: take the token with the highest probability
                    [next] = Argmax(state.logits, config.vocab_size)
                Else
                    ' apply the temperature to the logits
                    For q = 0 To config.vocab_size - 1
                        state.logits(q) /= temperature
                    Next
                    ' apply softmax to the logits to get the probabilities for next token
                    Softmax(state.logits, 0, config.vocab_size)
                    ' we sample from this distribution to get the next token
                    If topp <= 0 Then
                        ' simply sample from the predicted probability distribution
                        [next] = Sample(state.logits, config.vocab_size)
                    Else
                        ' top-p (nucleus) sampling, clamping the least likely tokens to zero
                        [next] = SampleTopp(state.logits, config.vocab_size, topp, state.probindex)
                    End If
                End If
            End If

            pos += 1

            ' data-dependent terminating condition: the BOS (1) token delimits sequences
            If [next] = 1 Then
                Exit While
            End If

            ' following BOS (1) token, sentencepiece decoder strips any leading whitespace (see PR #89)
            Dim tokenStr As String = If(token = 1 AndAlso vocab([next])(0) = " "c, vocab([next]).TrimStart(), vocab([next]))
            Console.Write(tokenStr)
            token = [next]
        End While

        timer.Start()
        Console.WriteLine()

        ' report achieved tok/s (pos-1 because the timer starts after first iteration)
        If pos > 1 Then
            Console.WriteLine($"achieved tok/s: {(pos - 1) / timer.Elapsed.Seconds}, tokens: {pos - 1} time: {timer.Elapsed}")
        End If
    End Sub

    Private Function StrLookup(str As String, vocab As String(), vocabSize As Integer) As Integer
        For i = 0 To vocabSize - 1
            If Equals(str, vocab(i)) Then Return i
        Next
        Return -1
    End Function

    Private Sub BpeEncode(text As String, vocab As String(), vocabScores As Single(), vocabSize As Integer, maxTokenLength As Integer, ByRef tokens As Integer(), ByRef nTokens As Integer)

        Dim strBuffer As New StringBuilder(maxTokenLength * 2 + 1) ' *2 for concat, +1 for null terminator

        ' first encode every individual byte in the input string
        nTokens = 0 ' the number of tokens
        For Each c In text
            strBuffer.Clear()
            strBuffer.Append(c)

            Dim id As Integer = StrLookup(strBuffer.ToString(), vocab, vocabSize)
            If id = -1 Then
                Console.Error.WriteLine("not good")
                Throw New Exception("Encoding error")
            End If

            tokens(nTokens) = id
            nTokens += 1
        Next

        ' merge the best consecutive pair each iteration, according to the scores in vocab_scores
        While True
            Dim bestScore = Single.MinValue
            Dim bestId = -1
            Dim bestIdx = -1

            For i = 0 To nTokens - 1 - 1
                ' check if we can merge the pair (tokens[i], tokens[i+1])
                strBuffer.Clear()
                strBuffer.Append(vocab(tokens(i)))
                strBuffer.Append(vocab(tokens(i + 1)))

                Dim id As Integer = StrLookup(strBuffer.ToString(), vocab, vocabSize)
                If id <> -1 AndAlso vocabScores(id) > bestScore Then
                    ' this merge pair exists in vocab! record its score and position
                    bestScore = vocabScores(id)
                    bestId = id
                    bestIdx = i
                End If
            Next

            If bestIdx = -1 Then Exit While ' we couldn't find any more pairs to merge, so we're done

            ' merge the consecutive pair (bestIdx, bestIdx+1) into new token bestId
            tokens(bestIdx) = bestId
            ' delete token at position bestIdx+1, shift the entire sequence back 1
            For i = bestIdx + 1 To nTokens - 1 - 1
                tokens(i) = tokens(i + 1)
            Next
            nTokens -= 1 ' token length decreased
        End While
    End Sub


    ' This method sets the seed for the RNG
    Private Sub SetSeed(seed As Long)
        _rngSeed = seed
    End Sub

    Private Function RandomU32() As Integer
        ' xorshift rng: https://en.wikipedia.org/wiki/Xorshift#xorshift.2A
        _rngSeed = _rngSeed Xor _rngSeed >> 12
        _rngSeed = _rngSeed Xor _rngSeed << 25
        _rngSeed = _rngSeed Xor _rngSeed >> 27
        Return _rngSeed * &H2545F4914F6CDD1DL >> 32
    End Function

    Private Function RandomF32() As Single
        ' random float32 in [0,1)
        Return (RandomU32() >> 8) / 16777216.0F
    End Function

    Private Function Argmax(probabilities As Single(), configVocabSize As Integer) As Integer
        Dim maxI = 0
        Dim maxP = probabilities(0)
        For i = 1 To configVocabSize - 1
            If probabilities(i) > maxP Then
                maxI = i
                maxP = probabilities(i)
            End If
        Next

        Return maxI
    End Function


    Private Function Sample(probabilities As Single(), configVocabSize As Integer) As Integer
        Dim r As Single = RandomF32()
        Dim cdf = 0.0F
        For i = 0 To configVocabSize - 1
            cdf += probabilities(i)
            If r < cdf Then Return i
        Next

        Return configVocabSize - 1
    End Function

    Private Function Compare(a As ProbIndex, b As ProbIndex) As Integer
        If a.Prob > b.Prob Then Return -1
        If a.Prob < b.Prob Then Return 1
        Return 0
    End Function

    Private Function SampleTopp(probabilities As Single(), configVocabSize As Integer, topp As Single, probindex As ProbIndex()) As Integer
        For i = 0 To configVocabSize - 1
            probindex(i).Index = i
            probindex(i).Prob = probabilities(i)
        Next

        Array.Sort(probindex, New Comparison(Of ProbIndex)(AddressOf Compare))

        Dim cumulativeProb = 0.0F
        Dim lastIdx = 0
        For i = 0 To configVocabSize - 1
            cumulativeProb += probindex(i).Prob
            If cumulativeProb > topp Then
                lastIdx = i
                Exit For
            End If
        Next

        Dim r As Single = RandomF32() * cumulativeProb
        Dim cdf = 0.0F
        For i = 0 To lastIdx
            cdf += probindex(i).Prob
            If r < cdf Then Return probindex(i).Index
        Next

        Return probindex(lastIdx).Index
    End Function


    Private Sub Accum(a As Single(), b As Single(), size As Integer)
        For i = 0 To size - 1
            a(i) += b(i)
        Next
    End Sub

    Private Sub Rmsnorm(o As Single(), x As Single(), weight As ArraySegment(Of Single), size As Integer)
        ' calculate sum of squares
        Dim ss = 0.0F
        For j = 0 To size - 1
            ss += x(j) * x(j)
        Next
        ss /= size
        ss += 0.00001F
        ss = 1.0F / MathF.Sqrt(ss)

        ' normalize and scale
        For j = 0 To size - 1
            o(j) = weight(j) * (ss * x(j))
        Next
    End Sub

    Private Sub Softmax(x As Single(), xOffset As Integer, size As Integer)
        ' find max value (for numerical stability)
        Dim maxVal = x(0 + xOffset)
        For i = 1 To size - 1
            If x(i + xOffset) > maxVal Then maxVal = x(i + xOffset)
        Next
        ' exp and sum
        Dim sum = 0.0F
        For i = 0 To size - 1
            x(i + xOffset) = CSng(Math.Exp(x(i + xOffset) - maxVal))
            sum += x(i + xOffset)
        Next

        ' normalize
        For i = 0 To size - 1
            x(i + xOffset) /= sum
        Next
    End Sub

    Private Sub Matmul(xout As Single(), x As Single(), w As ArraySegment(Of Single), n As Integer, d As Integer)
        ' W (d,n) @ x (n,) . xout (d,)
        Parallel.For(0, d, Sub(i)
                               Dim val = 0.0F
                               For j = 0 To n - 1
                                   val += w(i * n + j) * x(j)
                               Next
                               xout(i) = val
                           End Sub)
    End Sub


    Private Sub Transformer(token As Integer, pos As Integer, config As Config, state As RunState, w As TransformerWeights)
        ' a few convenience variables
        Dim [dim] = config.dim
        Dim hiddenDim = config.hidden_dim
        Dim headSize As Integer = [dim] / config.n_heads

        ' copy the token embedding into x
        Array.Copy(w.token_embedding_table, token * [dim], state.x, 0, [dim])


        ' forward all the layers
        For l = 0 To config.n_layers - 1
            ' attention rmsnorm
            Call Rmsnorm(state.xb, state.x, w.rms_att_weight.Skip(l * [dim]).ToArray(), [dim])

            ' qkv matmuls for this position
            Call Matmul(state.q, state.xb, w.wq.Skip(l * [dim] * [dim]).ToArray(), [dim], [dim])
            Call Matmul(state.k, state.xb, w.wk.Skip(l * [dim] * [dim]).ToArray(), [dim], [dim])
            Call Matmul(state.v, state.xb, w.wv.Skip(l * [dim] * [dim]).ToArray(), [dim], [dim])

            ' RoPE relative positional encoding: complex-valued rotate q and k by freq_cis in each head
            For i = 0 To [dim] - 1 Step 2
                Dim q0 = state.q(i)
                Dim q1 = state.q(i + 1)
                Dim k0 = state.k(i)
                Dim k1 = state.k(i + 1)
                Dim fcr = w.freq_cis_real(pos * headSize / 2 + i Mod headSize / 2)
                Dim fci = w.freq_cis_imag(pos * headSize / 2 + i Mod headSize / 2)
                state.q(i) = q0 * fcr - q1 * fci
                state.q(i + 1) = q0 * fci + q1 * fcr
                state.k(i) = k0 * fcr - k1 * fci
                state.k(i + 1) = k0 * fci + k1 * fcr
            Next

            ' save key,value at this time step (pos) to our kv cache
            Dim loff = l * config.seq_len * [dim] ' kv cache layer offset for convenience
            Array.Copy(state.k, 0, state.key_cache, loff + pos * [dim], [dim])
            Array.Copy(state.v, 0, state.value_cache, loff + pos * [dim], [dim])

            ' multihead attention. iterate over all heads
            Parallel.For(0, config.n_heads, Sub(h)
                                                ' get the query vector for this head
                                                Dim qOffset = h * headSize

                                                ' attention scores for this head
                                                Dim attOffset = h * config.seq_len

                                                ' iterate over all timesteps, including the current one
                                                For t = 0 To pos
                                                    ' get the key vector for this head and at this timestep
                                                    Dim keyCacheOffset = loff + t * [dim] + h * headSize

                                                    ' calculate the attention score as the dot product of q and k
                                                    Dim score = 0.0F
                                                    For i = 0 To headSize - 1
                                                        score += state.q(i + qOffset) * state.key_cache(i + keyCacheOffset)
                                                    Next

                                                    score /= CSng(Math.Sqrt(headSize))

                                                    ' save the score to the attention buffer
                                                    state.att(t + attOffset) = score
                                                Next


                                                ' softmax the scores to get attention weights, from 0..pos inclusively
                                                Softmax(state.att, attOffset, pos + 1)

                                                ' weighted sum of the values, store back into xb
                                                Dim xbOffset = h * headSize
                                                For i = xbOffset To xbOffset + headSize - 1
                                                    state.xb(i) = 0F
                                                Next

                                                For t = 0 To pos
                                                    ' get the value vector for this head and at this timestep
                                                    Dim vOffset = loff + t * [dim] + h * headSize

                                                    ' get the attention weight for this timestep
                                                    Dim a = state.att(t + attOffset)

                                                    ' accumulate the weighted value into xb
                                                    For i = 0 To headSize - 1
                                                        state.xb(i + xbOffset) += a * state.value_cache(i + vOffset)
                                                    Next
                                                Next
                                            End Sub)

            ' final matmul to get the output of the attention
            Call Matmul(state.xb2, state.xb, w.wo.Skip(l * [dim] * [dim]).ToArray(), [dim], [dim])

            ' residual connection back into x
            Accum(state.x, state.xb2, [dim])

            ' ffn rmsnorm
            Call Rmsnorm(state.xb, state.x, w.rms_ffn_weight.Skip(l * [dim]).ToArray(), [dim])

            ' Now for FFN in PyTorch we have: self.w2(F.silu(self.w1(x)) * self.w3(x))
            ' first calculate self.w1(x) and self.w3(x)
            Call Matmul(state.hb, state.xb, w.w1.Skip(l * [dim] * hiddenDim).ToArray(), [dim], hiddenDim)
            Call Matmul(state.hb2, state.xb, w.w3.Skip(l * [dim] * hiddenDim).ToArray(), [dim], hiddenDim)

            ' F.silu; silu(x)=x*σ(x),where σ(x) is the logistic sigmoid
            For i = 0 To hiddenDim - 1
                state.hb(i) *= 1.0F / (1.0F + CSng(Math.Exp(-state.hb(i))))
            Next

            ' elementwise multiply with w3(x)
            For i = 0 To hiddenDim - 1
                state.hb(i) *= state.hb2(i)
            Next

            ' final matmul to get the output of the ffn
            Call Matmul(state.xb, state.hb, w.w2.Skip(l * [dim] * hiddenDim).ToArray(), hiddenDim, [dim])

            ' residual connection
            Accum(state.x, state.xb, [dim])
        Next

        ' final rmsnorm
        Rmsnorm(state.x, state.x, w.rms_final_weight, [dim])

        ' classifier into logits
        Matmul(state.logits, state.x, w.wcls, config.dim, config.vocab_size)
    End Sub

    Private Sub CheckpointInitWeights(ByRef w As TransformerWeights, ByRef p As Config, accessor As MemoryMappedViewAccessor, sharedWeights As Boolean)
        Dim offset As Long = 0

        w.token_embedding_table = ReadFloatArray(accessor, offset, p.vocab_size * p.dim)
        w.rms_att_weight = ReadFloatArray(accessor, offset, p.n_layers * p.dim)
        w.wq = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.wk = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.wv = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.wo = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.rms_ffn_weight = ReadFloatArray(accessor, offset, p.n_layers * p.dim)
        w.w1 = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.hidden_dim)
        w.w2 = ReadFloatArray(accessor, offset, p.n_layers * p.hidden_dim * p.dim)
        w.w3 = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.hidden_dim)
        w.rms_final_weight = ReadFloatArray(accessor, offset, p.dim)
        Dim headSize As Integer = p.dim / p.n_heads
        w.freq_cis_real = ReadFloatArray(accessor, offset, p.seq_len * headSize / 2)
        w.freq_cis_imag = ReadFloatArray(accessor, offset, p.seq_len * headSize / 2)

        If sharedWeights Then
            w.wcls = w.token_embedding_table
        End If
    End Sub

    Private Function ReadFloatArray(accessor As MemoryMappedViewAccessor, ByRef offset As Long, size As Integer) As Single()
        Dim array = New Single(size - 1) {}
        accessor.ReadArray(offset, array, 0, size)
        offset += Marshal.SizeOf(Of Single)() * CLng(size)
        Return array
    End Function

    Private Function InitializeRunState(cfg As Config) As RunState
        Return New RunState With {
            .x = New Single(cfg.dim - 1) {},
            .xb = New Single(cfg.dim - 1) {},
            .xb2 = New Single(cfg.dim - 1) {},
            .hb = New Single(cfg.hidden_dim - 1) {},
            .hb2 = New Single(cfg.hidden_dim - 1) {},
            .q = New Single(cfg.dim - 1) {},
            .k = New Single(cfg.dim - 1) {},
            .v = New Single(cfg.dim - 1) {},
            .att = New Single(cfg.n_heads * cfg.seq_len - 1) {},
            .logits = New Single(cfg.vocab_size - 1) {},
            .probindex = New ProbIndex(cfg.vocab_size - 1) {},
            .key_cache = New Single(cfg.n_layers * cfg.seq_len * cfg.dim - 1) {},
            .value_cache = New Single(cfg.n_layers * cfg.seq_len * cfg.dim - 1) {}
        }
    End Function
End Module
