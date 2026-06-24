using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.DTOs.Requests;
using Microsoft.Extensions.Hosting;
using IntegrationTests.Etiquetas.Fixtures;

namespace IntegrationTests.Etiquetas
{
    public class TestsMastersGestionEtiquetas : IClassFixture<AuthenticatedClientFixture>
    {
        private readonly HttpClient _client;

        public TestsMastersGestionEtiquetas(AuthenticatedClientFixture fixture)
        {
            _client = fixture.ClientNegocio;
        }
        //TODO: replantear tests
        [Fact]
        public async Task CRUD_Maestro_Secciones()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterSecciones/getListSecciones?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de secciones porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevaSeccion = new MasterDataDTO { Descripcion = "Sección de test" };
            var postResponse = await _client.PostAsJsonAsync("/api/MasterSecciones/postSeccion", nuevaSeccion);
            if (!postResponse.IsSuccessStatusCode)
                await MostrarError(postResponse, "Error al hacer POST de seccion.");

            // 3. GET para obtener ID del que acabamos de añadir
            var listaActualizada = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterSecciones/getListSecciones?pageNumber=1&pageSize=1000");
            var creada = listaActualizada.Items.FirstOrDefault(x => x.Descripcion == "Sección de test");
            Assert.True(creada != null, "No se ha podido obtener la seccion creada.");

            // 4. PUT
            creada.Descripcion = "Sección modificada";
            var putResponse = await _client.PutAsJsonAsync("/api/MasterSecciones/putSeccion", creada);
            if (!putResponse.IsSuccessStatusCode)
                await MostrarError(putResponse, "Error al hacer PUT de seccion.");

            // 5. DELETE
            var deleteResponse = await _client.DeleteAsync($"/api/MasterSecciones/deleteSeccion?id={creada.Id}");
            if (!deleteResponse.IsSuccessStatusCode)
                await MostrarError(deleteResponse, "Error al hacer DELETE de seccion.");
        }

        [Fact]
        public async Task CRUD_Maestro_Clientes()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterClientes/getListClientes?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los clientes porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "Cliente de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterClientes/postCliente", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del cliente.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterClientes/getListClientes?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el cliente creado.");

            // 4. PUT
            creado.Descripcion = "Cliente modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterClientes/putCliente", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del cliente.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterClientes/deleteCliente?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del cliente.");
        }

        [Fact]
        public async Task CRUD_Maestro_IdiomasLabel()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterIdiomasLabels/getListIdiomasLabel?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los idiomas de label porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "IdiomaLabel de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterIdiomasLabels/postIdiomaLabel", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del idioma label.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterIdiomasLabels/getListIdiomasLabel?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el idioma label creado.");

            // 4. PUT
            creado.Descripcion = "IdiomaLabel modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterIdiomasLabels/putIdiomaLabel", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del idioma label.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterIdiomasLabels/deleteIdiomaLabel?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del idioma label.");
        }

        [Fact]
        public async Task CRUD_Maestro_TiposCampo()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposCampos/getListTiposCampo?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de campos porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "TipoCampo de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterTiposCampos/postTipoCampo", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del tipo de campo.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposCampos/getListTiposCampo?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el tipo campo creado.");

            // 4. PUT
            creado.Descripcion = "TipoCampo modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterTiposCampos/putTipoCampo", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del tipo de campo.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterTiposCampos/deleteTipoCampo?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del tipo de campo.");
        }

        [Fact]
        public async Task CRUD_Maestro_TiposCodigoBarra()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposCodigoBarra/getListTiposCodigosBarra?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de codigos de barras porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "TipoCodigoBarra de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterTiposCodigoBarra/postTipoCodigoBarra", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del tipo de codigo de barras.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposCodigoBarra/getListTiposCodigosBarra?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el tipo código de barra creado.");

            // 4. PUT
            creado.Descripcion = "TipoCodigoBarra modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterTiposCodigoBarra/putTipoCodigoBarra", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del tipo de codigo de barras.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterTiposCodigoBarra/deleteTipoCodigoBarra?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del tipo de codigo de barras.");
        }

        [Fact]
        public async Task CRUD_Maestro_TiposConservacion()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposConservacion/getListTiposConservacion?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de conservacion porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "TipoConservacion de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterTiposConservacion/postTipoConservacion", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del tipo de conservacion.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposConservacion/getListTiposConservacion?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el tipo conservación creado.");

            // 4. PUT
            creado.Descripcion = "TipoConservacion modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterTiposConservacion/putTipoConservacion", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del tipo de conservacion.");
            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterTiposConservacion/deleteTipoConservacion?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del tipo de conservacion.");
        }

        [Fact]
        public async Task CRUD_Maestro_TiposEnvasado()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposEnvasado/getListTiposEnvasado?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de envasado porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "TipoEnvasado de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterTiposEnvasado/postTipoEnvasado", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del tipo de envasado.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposEnvasado/getListTiposEnvasado?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el tipo envasado creado.");

            // 4. PUT
            creado.Descripcion = "TipoEnvasado modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterTiposEnvasado/putTipoEnvasado", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del tipo de envasado.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterTiposEnvasado/deleteTipoEnvasado?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del tipo de envasado.");
        }

        [Fact]
        public async Task CRUD_Maestro_TiposEtiqueta()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposEtiqueta/getListTiposEtiquetas?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de etiqueta porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "TipoEtiqueta de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterTiposEtiqueta/postTipoEtiqueta", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del tipo de etiqueta.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposEtiqueta/getListTiposEtiquetas?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == "TipoEtiqueta de test");
            Assert.True(creado != null, "No se ha podido obtener el tipo etiqueta creado.");

            // 4. PUT
            creado.Descripcion = "TipoEtiqueta modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterTiposEtiqueta/putTipoEtiqueta", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del tipo de etiqueta.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterTiposEtiqueta/deleteTipoEtiqueta?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE del tipo de etiqueta.");
        }

        [Fact]
        public async Task CRUD_Maestro_TiposLabels()
        {
            // 1. GET list inicial
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposLabels/getListTiposLabel?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se han podido obtener los tipos de label porque el GET ha fallado o no trae datos.");

            // 2. POST
            var nuevo = new MasterDataDTO { Descripcion = "TipoLabel de test" };
            var post = await _client.PostAsJsonAsync("/api/MasterTiposLabels/postTipoLabel", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del tipo de label.");

            // 3. GET para obtener ID del que acabamos de añadir
            var lista = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposLabels/getListTiposLabel?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.Descripcion == nuevo.Descripcion);
            Assert.True(creado != null, "No se ha podido obtener el tipo label creado.");

            // 4. PUT
            creado.Descripcion = "TipoLabel modificado";
            var put = await _client.PutAsJsonAsync("/api/MasterTiposLabels/putTipoLabel", creado);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del tipo de label.");

            // 5. DELETE
            var delete = await _client.DeleteAsync($"/api/MasterTiposLabels/deleteTipoLabel?id={creado.Id}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Error al hacer DELETE de tipo de label.");
        }

        [Fact]
        public async Task CRUD_Maestro_Campos()
        {
            var listaInicial = await _client.GetFromJsonAsync<PaginatedResult<CampoDTO>>("/api/MasterCampos/getListCampos?pageNumber=1&pageSize=1000");
            Assert.True(listaInicial != null && listaInicial.Items != null && listaInicial.Items.Any(), "No se pudieron obtener los campos GET falló.");

            var nuevo = new CampoDTORequest
            {
                NombreCampo = "CampoTest",
                IdTipoCampo = 1,
                IsNavision = false
            };

            var post = await _client.PostAsJsonAsync("/api/MasterCampos/postCampo", nuevo);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Error al hacer POST del campo.");

            var lista = await _client.GetFromJsonAsync<PaginatedResult<CampoDTO>>("/api/MasterCampos/getListCampos?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.NombreCampo == nuevo.NombreCampo);
            Assert.True(creado != null, "El campo no se ha creado por algun motivo.");

            var nuevoCampo = new CampoDTORequest
            {
                PkIdCampo = creado.PkIdCampo,
                IdTipoCampo = creado.IdTipoCampo,
                IsNavision = true,
                NombreCampo = "CampoTestModificado"
            };
            var put = await _client.PutAsJsonAsync("/api/MasterCampos/putCampo", nuevoCampo);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer PUT del campo.");

            var listaPostPut = await _client.GetFromJsonAsync<PaginatedResult<CampoDTO>>("/api/MasterCampos/getListCampos?pageNumber=1&pageSize=1000");
            var modificado = listaPostPut.Items.FirstOrDefault(x => x.PkIdCampo == creado.PkIdCampo);
            Assert.True(nuevoCampo.NombreCampo == modificado?.NombreCampo, "El campo 'NombreCampo' no se modificó correctamente tras el PUT.");
            Assert.True(modificado?.IsNavision ?? false, "El campo 'IsNavision' no se modificó correctamente tras el PUT.");

            var delete = await _client.DeleteAsync($"/api/MasterCampos/deleteCampo?id={creado.PkIdCampo}");
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Error al hacer DELETE del campo.");

            var listaPostDelete = await _client.GetFromJsonAsync<PaginatedResult<CampoDTO>>("/api/MasterCampos/getListCampos?pageNumber=1&pageSize=1000");
            var existe = listaPostDelete.Items.Any(x => x.PkIdCampo == creado.PkIdCampo);
            Assert.False(existe, "El campo no fue eliminado correctamente.");
        }

        [Fact]
        public async Task CRUD_Maestro_Labels()
        {
            // 1. Crear TipoLabel temporal
            var tipoLabelRequest = new MasterDataDTO { Descripcion = "TipoLabelTestTemporal" };
            var postTipo = await _client.PostAsJsonAsync("/api/MasterTiposLabels/postTipoLabel", tipoLabelRequest);
            if (!postTipo.IsSuccessStatusCode)
                await MostrarError(postTipo, "Error al crear un tipo de label de prueba");

            var tipos = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterTiposLabels/getListTiposLabel?pageNumber=1&pageSize=1000");
            var tipoCreado = tipos.Items.First(x => x.Descripcion == tipoLabelRequest.Descripcion);

            // 2. Crear IdiomaLabel temporal
            var idiomaLabelRequest = new MasterDataDTO { Descripcion = "IdiomaLabelTestTemporal" };
            var postIdioma = await _client.PostAsJsonAsync("/api/MasterIdiomasLabels/postIdiomaLabel", idiomaLabelRequest);
            if (!postIdioma.IsSuccessStatusCode)
                await MostrarError(postIdioma, "Fallo al crear IdiomaLabel de prueba.");

            var idiomas = await _client.GetFromJsonAsync<PaginatedResult<MasterDataDTO>>("/api/MasterIdiomasLabels/getListIdiomasLabel?pageNumber=1&pageSize=1000");
            var idiomaCreado = idiomas.Items.First(x => x.Descripcion == idiomaLabelRequest.Descripcion);

            // 3. Crear Label usando los dos anteriores
            var nuevaLabel = new LabelDTORequest
            {
                DescripcionLabel = "LabelTest",
                FKIdIdioma = idiomaCreado.Id,
                FKIdTipoLabel = tipoCreado.Id
            };

            var post = await _client.PostAsJsonAsync("/api/MasterLabels/postLabel", nuevaLabel);
            if (!post.IsSuccessStatusCode)
                await MostrarError(post, "Ha fallado el POST del label");

            // 4. Verificar creación
            var lista = await _client.GetFromJsonAsync<PaginatedResult<LabelDTO>>("/api/MasterLabels/getListLabels?pageNumber=1&pageSize=1000");
            var creado = lista.Items.FirstOrDefault(x => x.DescripcionLabel == nuevaLabel.DescripcionLabel);
            Assert.True(creado != null, "Por algun motivo ajeno no se ha creado esa label con esa descripcion.");

            // 5. Modificar y hacer PUT
            var labelModificada = new LabelDTORequest
            {
                PKIdLabel = creado.PkIdLabel,
                DescripcionLabel = "LabelTestModificado",
                FKIdIdioma = creado.IdIdiomaLabel,
                FKIdTipoLabel = creado.IdTipoLabel
            };
            var put = await _client.PutAsJsonAsync("/api/MasterLabels/putLabel", labelModificada);
            if (!put.IsSuccessStatusCode)
                await MostrarError(put, "Ha fallado el PUT del label");

            // 6. Verificar modificación
            var listaPostPut = await _client.GetFromJsonAsync<PaginatedResult<LabelDTO>>("/api/MasterLabels/getListLabels?pageNumber=1&pageSize=1000");
            var modificado = listaPostPut.Items.FirstOrDefault(x => x.PkIdLabel == creado.PkIdLabel);
            Assert.True(labelModificada.DescripcionLabel == modificado?.DescripcionLabel, "La label no se ha modificado, mira haber que ocurre con el objeto.");

            // 7. Eliminar Label
            var delete = await _client.DeleteAsync($"/api/MasterLabels/deleteLabel?id={creado.PkIdLabel}");
            if (!delete.IsSuccessStatusCode)
                await MostrarError(delete, "Ha fallado el DELETE del label");

            var listaPostDelete = await _client.GetFromJsonAsync<PaginatedResult<LabelDTO>>("/api/MasterLabels/getListLabels?pageNumber=1&pageSize=1000");
            var existe = listaPostDelete.Items.Any(x => x.PkIdLabel == creado.PkIdLabel);
            Assert.False(existe, "El label no fue eliminado correctamente.");

            // 8. Limpiar: eliminar TipoLabel e IdiomaLabel
            var deleteTipo = await _client.DeleteAsync($"/api/MasterTiposLabels/deleteTipoLabel?id={tipoCreado.Id}");
            if (!deleteTipo.IsSuccessStatusCode)
                await MostrarError(deleteTipo, "No se pudo eliminar el TipoLabel temporal.");

            var deleteIdioma = await _client.DeleteAsync($"/api/MasterIdiomasLabels/deleteIdiomaLabel?id={idiomaCreado.Id}");
            if (!deleteIdioma.IsSuccessStatusCode)
                await MostrarError(deleteIdioma, "No se pudo eliminar el IdiomaLabel temporal.");
        }


        private async Task MostrarError(HttpResponseMessage response, string mensaje)
        {
            var detalle = await response.Content.ReadAsStringAsync();
            Assert.True(false, $"{mensaje} - StatusCode: {response.StatusCode}, Contenido: {detalle}");
        }
    }
}
