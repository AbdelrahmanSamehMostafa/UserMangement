using System.Text;
using System.Text.Json;
using com.gbg.modules.utility.Helpers.DTo.UsersDTO;

namespace UserManagmentRazor.Extentions
{
    public interface IGenericApiService
    {
        Task<TResponse> SearchAsync<T, TInput, TResponse>(string baseUrl, TInput queryParams, TResponse response,
            string token, bool isLogs, Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null);
        Task<T?> GetAsync<T>(string url, string token, Func<HttpResponseMessage, Task<T?>> handleErrorResponse = null);

        Task<T> GetListAsync<T>(string url, string token, Func<HttpResponseMessage, Task<T>> handleErrorResponse = null);
        Task<TResponse> LoginAsync<TRequest, TResponse>(
           string url,
           TRequest requestData, string token,
           Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null);

        Task<TResponse> DeleteAsync<TResponse>(string url, string token);

        Task<bool> LogoutAsync(string logoutUrl, string token);

        Task<byte[]> ExportExcelAsync(string url, string token, object listingInput = null,
             Func<HttpResponseMessage, Task<byte[]>> handleErrorResponse = null);

        Task<TResponse> UploadFileAsync<TRequest, TResponse>(
            string url,
            TRequest requestData, string token,
            Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null);

        Task<TResponse> PutAsync<TRequest, TResponse>(
            string url,
            TRequest requestData,
            string token,
            Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null);

        Task<TResponse> PostAsync<TRequest, TResponse>(
            string url,
            TRequest requestData, string token,
            Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null);
    }

    public class GenericApiService : IGenericApiService
    {
        private readonly HttpClient _httpClient;
        public GenericApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private string ConvertToQueryString(object queryParams)
        {
            if (queryParams == null)
                return string.Empty;

            // Use reflection to get the properties of the object
            var properties = queryParams.GetType().GetProperties();
            var queryParameters = properties
                .Where(p => p.GetValue(queryParams) != null)
                .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(queryParams)?.ToString())}");

            return string.Join("&", queryParameters);
        }

        public async Task<T?> GetAsync<T>(string url, string token, Func<HttpResponseMessage, Task<T?>> handleErrorResponse = null)
        {
            // Set the authorization header
            SetAuthorizationHeader(token);

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            // Prepare JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            };

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                // You can log the error here if needed
                Console.WriteLine($"Error: {errorResponseJson}");

                return JsonSerializer.Deserialize<T>(errorResponseJson, options);
            }
        }

        public async Task<T> GetListAsync<T>(string url, string token, Func<HttpResponseMessage, Task<T>> handleErrorResponse = null)
        {
            // Set the authorization header
            SetAuthorizationHeader(token);

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            // Prepare JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            };

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                // You can log the error here if needed
                Console.WriteLine($"Error: {errorResponseJson}");

                return JsonSerializer.Deserialize<T>(errorResponseJson, options);
            }
        }

        public async Task<TResponse> SearchAsync<T, TInput, TResponse>(string baseUrl, TInput queryParams, TResponse Tresponse,
            string token, bool isLogs, Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null)
        {
            SetAuthorizationHeader(token);
            string url = baseUrl;

            if (!isLogs)
            {
                // Construct the query parameters from the object
                var queryString = ConvertToQueryString(queryParams);

                // Combine the base URL and the query string
                url = $"{baseUrl}?{queryString}";
            }

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            // Prepare JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            };

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                // You can log the error here if needed
                Console.WriteLine($"Error: {errorResponseJson}");

                return JsonSerializer.Deserialize<TResponse>(errorResponseJson, options);
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string url,
            TRequest requestData,
            string token,
            Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null)
        {
            // Set the authorization header
            SetAuthorizationHeader(token);

            // Serialize the request data to JSON
            var requestJson = JsonSerializer.Serialize(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Make the POST request
            var response = await _httpClient.PostAsync(url, content);

            // Prepare JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            };

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(errorResponseJson, options);
            }
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(
            string url,
            TRequest requestData,
            string token,
            Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null)
        {
            // Set the authorization header
            SetAuthorizationHeader(token);

            // Serialize the request data to JSON
            var requestJson = JsonSerializer.Serialize(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Make the PUT request
            var response = await _httpClient.PutAsync(url, content);

            // Prepare JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            };

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(errorResponseJson, options);
            }
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string url, string token)
        {
            SetAuthorizationHeader(token);

            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            var content = await response.Content.ReadAsStringAsync();

            // Deserialize the response into Tresposne
            var result = JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // Optionally add more options, e.g., handling null values
            });

            return result;
        }

        public async Task<TResponse> LoginAsync<TRequest, TResponse>(
                   string url,
                   TRequest requestData, string token,
                   Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null)
        {
            var requestJson = JsonSerializer.Serialize(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(errorResponseJson, options);
            }
        }

        public async Task<bool> LogoutAsync(string logoutUrl, string token)
        {
            SetAuthorizationHeader(token);

            var response = await _httpClient.PostAsync(logoutUrl, null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Logout successful.");

                return true;
            }
            else
            {
                Console.WriteLine($"Logout failed with status code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {responseContent}");

                return false;
            }
        }

        public async Task<byte[]> ExportExcelAsync(string url, string token, object listingInput = null,
             Func<HttpResponseMessage, Task<byte[]>> handleErrorResponse = null)
        {
            SetAuthorizationHeader(token);
            string finalUrl;

            if (listingInput != null)
            {
                // Construct the query parameters from the object
                var queryString = ConvertToQueryString(listingInput);

                // Combine the base URL and the query string
                finalUrl = $"{url}?{queryString}";
            }
            else
            {
                finalUrl = url;
            }

            // Make the GET request to the specified URL
            var response = await _httpClient.GetAsync(finalUrl);

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a byte array
                return await response.Content.ReadAsByteArrayAsync(); // Return the byte array representing the Excel file
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read the error response content
                var errorResponseContent = await response.Content.ReadAsStringAsync();
                // Log the error if needed
                Console.WriteLine($"Error: {errorResponseContent}");

                // You can choose to return null or throw an exception here
                throw new Exception($"Error exporting Excel: {errorResponseContent}");
            }
        }


        public async Task<TResponse> UploadFileAsync<TRequest, TResponse>(
            string url,
            TRequest requestData, string token,
            Func<HttpResponseMessage, Task<TResponse>> handleErrorResponse = null)
        {
            SetAuthorizationHeader(token);

            var content = new MultipartFormDataContent();

            if (requestData is UserMultipleInsertDto dto)
            {
                if (dto.File != null)
                {
                    var fileContent = new StreamContent(dto.File.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.File.ContentType);
                    content.Add(fileContent, "file", dto.File.FileName);
                }
            }
            // Check if requestData has a file property using reflection
            var fileProperty = typeof(TRequest).GetProperty("file");
            if (fileProperty != null && fileProperty.GetValue(requestData) is IFormFile file)
            {
                if (file != null)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "file", file.FileName);
                }
            }

            // Serialize the requestData and add non-file properties as form fields
            foreach (var prop in typeof(TRequest).GetProperties())
            {
                if (prop.Name != "file")
                {
                    var value = prop.GetValue(requestData);
                    if (value != null)
                    {
                        content.Add(new StringContent(value.ToString()), prop.Name);
                    }
                }
            }

            var response = await _httpClient.PostAsync(url, content);
            // Prepare JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            };

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(jsonResponse, options);
            }
            else
            {
                // Handle error response if a handler is provided
                if (handleErrorResponse != null)
                {
                    return await handleErrorResponse(response);
                }

                // Attempt to read and deserialize the error response content
                var errorResponseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(errorResponseJson, options);
            }
        }
    }
}