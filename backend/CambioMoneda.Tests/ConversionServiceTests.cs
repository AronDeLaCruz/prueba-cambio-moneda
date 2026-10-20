using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using CambioMoneda.Application.Services;
using CambioMoneda.Domain.Entities;
using CambioMoneda.Domain.Exceptions;
using NSubstitute;

namespace CambioMoneda.Tests;

public class ConversionServiceTests
{
    private readonly ITipoCambioRepository _repo = Substitute.For<ITipoCambioRepository>();
    private readonly ConversionService _sut;

    public ConversionServiceTests() => _sut = new ConversionService(_repo);

    private void ConfigurarTasa(string origen, string destino, decimal tasa) =>
        _repo.GetByParAsync(origen, destino)
             .Returns(new TipoCambio { MonedaOrigen = origen, MonedaDestino = destino, Tasa = tasa });

    private void ConfigurarSinTasa(string origen, string destino) =>
        _repo.GetByParAsync(origen, destino).Returns((TipoCambio?)null);

    [Fact]
    public async Task Convertir_ConTasaDirecta_MultiplicaMontoPorTasa()
    {
        ConfigurarTasa("USD", "PEN", 3.75m);

        var result = await _sut.ConvertirAsync(new ConversionRequest(100m, "USD", "PEN"));

        Assert.Equal(100m, result.Monto);
        Assert.Equal(375.00m, result.MontoConTipoCambio);
        Assert.Equal(3.75m, result.TipoCambio);
        Assert.Equal("USD", result.MonedaOrigen);
        Assert.Equal("PEN", result.MonedaDestino);
        await _repo.DidNotReceive().GetByParAsync("PEN", "USD");
    }

    [Fact]
    public async Task Convertir_SinParDirecto_UsaTasaInversa()
    {
        ConfigurarSinTasa("PEN", "USD");
        ConfigurarTasa("USD", "PEN", 4m);

        var result = await _sut.ConvertirAsync(new ConversionRequest(100m, "PEN", "USD"));

        Assert.Equal(0.25m, result.TipoCambio);
        Assert.Equal(25.00m, result.MontoConTipoCambio);
    }

    [Fact]
    public async Task Convertir_TasaInversa_RedondeaTasaA6YMontoA2()
    {
        ConfigurarSinTasa("PEN", "USD");
        ConfigurarTasa("USD", "PEN", 3.75m);

        var result = await _sut.ConvertirAsync(new ConversionRequest(100m, "PEN", "USD"));

        Assert.Equal(0.266667m, result.TipoCambio);
        Assert.Equal(26.67m, result.MontoConTipoCambio);
    }

    [Fact]
    public async Task Convertir_NormalizaMonedasAMayusculasYSinEspacios()
    {
        ConfigurarTasa("USD", "PEN", 3.75m);

        var result = await _sut.ConvertirAsync(new ConversionRequest(10m, " usd ", "pen"));

        Assert.Equal("USD", result.MonedaOrigen);
        Assert.Equal("PEN", result.MonedaDestino);
        Assert.Equal(37.50m, result.MontoConTipoCambio);
    }

    [Fact]
    public async Task Convertir_ParInexistente_LanzaNotFound()
    {
        ConfigurarSinTasa("USD", "JPY");
        ConfigurarSinTasa("JPY", "USD");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.ConvertirAsync(new ConversionRequest(100m, "USD", "JPY")));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Convertir_MontoNoPositivo_LanzaBadRequest(decimal monto)
    {
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _sut.ConvertirAsync(new ConversionRequest(monto, "USD", "PEN")));
    }

    [Fact]
    public async Task Convertir_MonedasIguales_LanzaBadRequest()
    {
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _sut.ConvertirAsync(new ConversionRequest(100m, "USD", "usd")));
    }

    [Theory]
    [InlineData("US", "PEN")]
    [InlineData("USD", "")]
    public async Task Convertir_MonedaMalFormada_LanzaBadRequest(string origen, string destino)
    {
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _sut.ConvertirAsync(new ConversionRequest(100m, origen, destino)));
    }
}