Imports System.Runtime.InteropServices

''' <summary>
''' current wave of activations
''' </summary>
<StructLayout(LayoutKind.Sequential)>
Public Structure RunState

    ''' <summary>
    ''' activation at current time stamp (dim,)
    ''' </summary>
    Public x As Single()
    ''' <summary>
    ''' same, but inside a residual branch (dim,)
    ''' </summary>
    Public xb As Single()
    ''' <summary>
    ''' an additional buffer just for convenience (dim,)
    ''' </summary>
    Public xb2 As Single()
    ''' <summary>
    ''' buffer for hidden dimension in the ffn (hidden_dim,)
    ''' </summary>
    Public hb As Single()
    ''' <summary>
    ''' buffer for hidden dimension in the ffn (hidden_dim,)
    ''' </summary>
    Public hb2 As Single()
    ''' <summary>
    ''' query (dim,)
    ''' </summary>
    Public q As Single()
    ''' <summary>
    ''' key (dim,)
    ''' </summary>
    Public k As Single()
    ''' <summary>
    ''' value (dim,)
    ''' </summary>
    Public v As Single()
    ''' <summary>
    ''' buffer for scores/attention values (n_heads, seq_len)
    ''' </summary>
    Public att As Single()
    ''' <summary>
    ''' output logits
    ''' </summary>
    Public logits As Single()

    ''' <summary>
    ''' buffer used in top-p sampling
    ''' </summary>
    Public probindex As ProbIndex()

    ' kv cache
    Public key_cache As Single() ' (layer, seq_len, dim)
    Public value_cache As Single() ' (layer, seq_len, dim)

End Structure