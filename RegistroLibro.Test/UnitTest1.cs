namespace RegistroLibro.Test;

public class UnitTest1
{
    [Fact]
    public void Sumar_DosNumero_DebeRetornarResultadoCorrecto()
    {
        var a = 10; var b = 20;
        var resultado = a + b;
        Assert.Equal(30, resultado);
    }
    
}
