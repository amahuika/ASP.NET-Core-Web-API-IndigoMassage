using Indigo.BuisnessLogic.Services;
using Indigo.BuisnessLogic.Utilities;
using Indigo.DataAccess.Data;
using Indigo.Models.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IndigoMassage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobServices _BlobServices;
        private readonly IDtoMappingToModel _dtoMapToModel;
        private ApiResponseDTO _response;

        public ServicesController(ApplicationDbContext context, IBlobServices blobServices, IDtoMappingToModel dtoMapToModel)
        {
            _context = context;
            _response = new ApiResponseDTO();
            _BlobServices = blobServices;
            _dtoMapToModel = dtoMapToModel;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO>> GetServices()
        {
            try
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = await _context.Services.Include(u => u.Prices).ToListAsync();
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add($"Something went wrong. Message: {ex.Message}");
                return BadRequest(_response);
            }

        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO>> GetService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);

                if (service == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add($"Service with id {id} not found");
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = service;

                return Ok(_response);


            }
            catch (Exception ex)
            {
                _response.ErrorMessages.Add($"Something went wrong. Message: {ex.Message}");

                return BadRequest(_response);
            }


        }

        // PUT: api/Services/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDTO>> PutService(int id, [FromForm] ServiceUpdateDTO serviceUpdateDto)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;

            if (!ModelState.IsValid)
            {
                return BadRequest(_response);
            }

            try
            {


                var serviceFromDb = await _context.Services.FindAsync(id);

                if (serviceFromDb == null)
                {

                    return BadRequest(_response);
                }

                serviceFromDb.Name = serviceUpdateDto.Name;
                serviceFromDb.Description = serviceUpdateDto.Description;

                // Check if image has been uploaded
                if (serviceUpdateDto.Image != null && serviceUpdateDto.Image.Length > 0)
                {
                    if (serviceFromDb.ImageUrl != null)
                    {
                        // delete current image 
                        var blobToDelete = serviceFromDb.ImageUrl.Split("/").Last();
                        await _BlobServices.DeleteBlob(blobToDelete, StaticDetails.SD_Blob_Container_Name);
                    }
                    // create new guid name and add the extension
                    var newImageName = $"{Guid.NewGuid()}{Path.GetExtension(serviceUpdateDto.Image.FileName)}";

                    // Upload new image and add to serviceFromDb
                    string newImageUrl = await _BlobServices.UploadBlob(newImageName, StaticDetails.SD_Blob_Container_Name, serviceUpdateDto.Image);
                    serviceFromDb.ImageUrl = newImageUrl;

                }

                await _context.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);


            }
            catch (DbUpdateException ex)
            {
                _response.ErrorMessages.Add($"Something went wrong. Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                _response.ErrorMessages.Add($"Something went wrong. Message: {ex.Message}");

            }

            return BadRequest(_response);


        }

        // POST: api/Services
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO>> PostService([FromForm] ServiceDTO serviceDto)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;


            if (!ModelState.IsValid)
            {
                _response.ErrorMessages.Add("Invalid model state");
                return BadRequest(_response);
            }

            try
            {


                if (serviceDto.Image == null || serviceDto.Image.Length == 0)
                {

                    _response.ErrorMessages.Add("Image is required");
                    return BadRequest(_response);
                }

                // get the image extension create new file name
                var fileName = $"${Guid.NewGuid()}${Path.GetExtension(serviceDto.Image.FileName)}";

                // Upload image to Azure Blob Storage
                string imageUrl;

                try
                {
                    imageUrl = await _BlobServices.UploadBlob(fileName, StaticDetails.SD_Blob_Container_Name, serviceDto.Image);
                }
                catch (Exception ex)
                {


                    _response.ErrorMessages.Add($"Something went wrong uploading the image: {ex.Message}");

                    return BadRequest(_response);
                }



                // add new service from serviceDTO 
                var newService = _dtoMapToModel.MapToServiceModel(serviceDto, imageUrl);


                // add to database
                _context.Services.Add(newService);
                _context.SaveChanges();

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = newService;
                return CreatedAtAction("GetService", new { id = newService.Id }, _response);

            }
            catch (Exception ex)
            {


                _response.ErrorMessages.Add($"Something went wrong. Error message: {ex.Message}");
                return BadRequest(_response);
            }



        }

        // DELETE: api/Services/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;

            if (id == 0)
            {

                return BadRequest(_response);
            }

            try
            {

                var serviceFromDb = await _context.Services.FindAsync(id);
                if (serviceFromDb == null)
                {
                    _response.ErrorMessages.Add($"Service not found");
                    return BadRequest(_response);

                }

                // delete blob
                if (serviceFromDb.ImageUrl != null)
                {

                    var blobName = serviceFromDb.ImageUrl.Split("/").Last();
                    var isDeleted = await _BlobServices.DeleteBlob(blobName, StaticDetails.SD_Blob_Container_Name);
                    if (isDeleted)
                    {
                        _response.IsSuccess = true;
                    }
                    // wait 2 seconds to make sure blob is deleted
                    int milliseconds = 2000;
                    Thread.Sleep(milliseconds);
                }
                _context.Services.Remove(serviceFromDb);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {

                _response.ErrorMessages.Add($"Something went wrong. Error message: {ex.Message}");
                _response.ErrorMessages.Add($"Inner exception: {ex.InnerException}");

                return BadRequest(_response);
            }
        }




    }
}
