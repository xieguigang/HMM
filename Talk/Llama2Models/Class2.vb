Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Structure TransformerWeights
    ' token embedding table
    Public token_embedding_table As Single() ' (vocab_size, dim)

    ' weights for rmsnorms
    Public rms_att_weight As Single() ' (layer, dim) rmsnorm weights

    Public rms_ffn_weight As Single() ' (layer, dim)

    ' weights for matmuls
    Public wq As Single() ' (layer, dim, dim)
    Public wk As Single() ' (layer, dim, dim)
    Public wv As Single() ' (layer, dim, dim)

    Public wo As Single() ' (layer, dim, dim)

    ' weights for ffn
    Public w1 As Single() ' (layer, hidden_dim, dim)
    Public w2 As Single() ' (layer, dim, hidden_dim)

    Public w3 As Single() ' (layer, hidden_dim, dim)

    ' final rmsnorm
    Public rms_final_weight As Single() ' (dim,)

    ' freq_cis for RoPE relatively positional embeddings
    Public freq_cis_real As Single() ' (seq_len, head_size/2)

    Public freq_cis_imag As Single() ' (seq_len, head_size/2)

    ' (optional) classifier weights for the logits, on the last layer
    Public wcls As Single()
End Structure