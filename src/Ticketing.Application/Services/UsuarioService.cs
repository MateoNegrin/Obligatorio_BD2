using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Usuarios;
using Ticketing.Domain;

namespace Ticketing.Application.Services;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<UsuarioResponse>> GetGeneralesAsync(CancellationToken ct = default);
    Task<UsuarioResponse?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default);
    Task CreateAsync(CrearUsuarioRequest request, CancellationToken ct = default);
    Task UpdateAsync(string numeroDocumento, ActualizarUsuarioRequest request, CancellationToken ct = default);
    Task DeleteAsync(string numeroDocumento, CancellationToken ct = default);
}

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<UsuarioResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(u => new UsuarioResponse(
            u.NumeroDocumento,
            u.Mail,
            u.Pais,
            u.Localidad,
            u.Calle,
            u.NumeroDireccion,
            u.CodigoPostal,
            u.FechaRegistro)).ToList();
    }

    public async Task<IReadOnlyList<UsuarioResponse>> GetGeneralesAsync(CancellationToken ct = default)
    {
        var usuarios = await _repository.GetGeneralesAsync(ct);
        return usuarios.Select(u => new UsuarioResponse(
            u.NumeroDocumento,
            u.Mail,
            u.Pais,
            u.Localidad,
            u.Calle,
            u.NumeroDireccion,
            u.CodigoPostal,
            u.FechaRegistro)).ToList();
    }

    public async Task<UsuarioResponse?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default)
    {
        var usuario = await _repository.GetByDocumentoAsync(numeroDocumento, ct);
        if (usuario is null) return null;
        return new UsuarioResponse(
            usuario.NumeroDocumento,
            usuario.Mail,
            usuario.Pais,
            usuario.Localidad,
            usuario.Calle,
            usuario.NumeroDireccion,
            usuario.CodigoPostal,
            usuario.FechaRegistro);
    }

    public async Task CreateAsync(CrearUsuarioRequest request, CancellationToken ct = default)
    {
        var usuario = new Usuario
        {
            NumeroDocumento = request.NumeroDocumento,
            Mail = request.Mail,
            Pais = request.Pais,
            Localidad = request.Localidad,
            Calle = request.Calle,
            NumeroDireccion = request.NumeroDireccion,
            CodigoPostal = request.CodigoPostal
        };
        await _repository.CreateAsync(usuario, ct);
    }

    public async Task UpdateAsync(string numeroDocumento, ActualizarUsuarioRequest request, CancellationToken ct = default)
    {
        var usuario = new Usuario
        {
            NumeroDocumento = numeroDocumento,
            Mail = request.Mail,
            Pais = request.Pais,
            Localidad = request.Localidad,
            Calle = request.Calle,
            NumeroDireccion = request.NumeroDireccion,
            CodigoPostal = request.CodigoPostal
        };
        await _repository.UpdateAsync(usuario, ct);
    }

    public async Task DeleteAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(numeroDocumento, ct);
    }
}
