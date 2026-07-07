using System.Net;
using System.Net.Http.Json;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.DTOs.Shipping;

namespace Marketplace.Application.UseCases.Shipping
{
    /// <summary>
    /// Proxy do ViaCEP encapsulado como endpoint da própria API para o frontend
    /// não precisar sair do domínio "/api" nem manter fallback direto para
    /// serviços externos.
    /// </summary>
    public class LookupAddressByCepUseCase(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<AddressLookupResponse> Execute(string rawCep)
        {
            var digits = new string((rawCep ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length != 8)
                throw new ValidationException("CEP inválido");

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            ViaCepResponse? data;
            try
            {
                using var response = await client.GetAsync($"https://viacep.com.br/ws/{digits}/json/");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException("CEP não encontrado");
                if (!response.IsSuccessStatusCode)
                    throw new DependencyException("Erro ao consultar CEP");

                data = await response.Content.ReadFromJsonAsync<ViaCepResponse>();
            }
            catch (HttpRequestException)
            {
                throw new DependencyException("Erro ao consultar CEP");
            }
            catch (TaskCanceledException)
            {
                throw new DependencyException("Erro ao consultar CEP");
            }

            if (data is null || data.Erro == true)
                throw new NotFoundException("CEP não encontrado");

            return new AddressLookupResponse
            {
                Cep = digits,
                Street = data.Logradouro ?? string.Empty,
                Neighborhood = data.Bairro ?? string.Empty,
                City = data.Localidade ?? string.Empty,
                State = data.Uf ?? string.Empty,
                Complement = string.IsNullOrWhiteSpace(data.Complemento) ? null : data.Complemento,
            };
        }

        private sealed class ViaCepResponse
        {
            public string? Cep { get; set; }
            public string? Logradouro { get; set; }
            public string? Complemento { get; set; }
            public string? Bairro { get; set; }
            public string? Localidade { get; set; }
            public string? Uf { get; set; }
            public bool? Erro { get; set; }
        }
    }
}
