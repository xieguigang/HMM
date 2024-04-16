Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Structure TransformerWeights

    ''' <summary>
    ''' token embedding table
    ''' </summary>
    Public token_embedding_table As Single() ' (vocab_size, dim)

    ''' <summary>
    ''' weights for rmsnorms
    ''' </summary>
    Public rms_att_weight As Single() ' (layer, dim) rmsnorm weights

    Public rms_ffn_weight As Single() ' (layer, dim)

    ''' <summary>
    ''' weights for matmuls
    ''' </summary>
    Public wq As Single() ' (layer, dim, dim)
    Public wk As Single() ' (layer, dim, dim)
    Public wv As Single() ' (layer, dim, dim)

    Public wo As Single() ' (layer, dim, dim)

    ''' <summary>
    ''' weights for ffn
    ''' </summary>
    Public w1 As Single() ' (layer, hidden_dim, dim)
    Public w2 As Single() ' (layer, dim, hidden_dim)

    Public w3 As Single() ' (layer, hidden_dim, dim)

    ''' <summary>
    ''' final rmsnorm
    ''' </summary>
    Public rms_final_weight As Single() ' (dim,)

    ''' <summary>
    ''' freq_cis for RoPE relatively positional embeddings
    ''' </summary>
    Public freq_cis_real As Single() ' (seq_len, head_size/2)

    Public freq_cis_imag As Single() ' (seq_len, head_size/2)

    ''' <summary>
    ''' (optional) classifier weights for the logits, on the last layer
    ''' </summary>
    Public wcls As Single()

End Structure