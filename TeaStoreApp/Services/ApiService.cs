using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TeaStoreApp.Models;

namespace TeaStoreApp.Services
{
    public static class ApiService
    {
        // ora lista di metodi per ciascuna api
        // con static posso chiamare funzione solo con nome di classe senza dover istanziare oggetto

        public static async Task<bool> RegisterUser(string name, string email, string phone, string password) 
        {
            // sotto definisco quella che era payload in app car sharing
            // metodo per registrare utente
            var register = new Register() 
            {
                Name = name,
                Email = email,
                Phone = phone,
                Password = password
            };
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(register);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // sotto concatenazione con endpoint da classe appsettings, per non ripeterlo x ogni metodo in questo file
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/users/register", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
    
        // metodo per il login
        public static async Task<bool> Login(string email, string password) 
        {
            // bisogna mandare username e password
            var login = new Login() 
            { 
                Email = email, Password = password 
            };
            // qui sotto simile a prima, cambiare "register" in "login"
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/users/login", content);
            if (!response.IsSuccessStatusCode) return false;

            // fare login con api restituisce access token con json, va deserializzato
            // seguendo proprietà della classe token
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Token>(jsonResult);
            // salvare token, chiave + valore da salvare. Lo stesso per user id e username
            Preferences.Set("accesstoken", result.AccessToken);
            Preferences.Set("userid", result.UserId);
            Preferences.Set("username", result.UserName);

            return true;
        }

        public static async Task<ProfileImage> GetUserProfileImage()
        {
            // metodo ritorna profileimage (metodo per upload è dopo)
            // serve mandare anche access token         
            var httpClient = new HttpClient();
            // aggiungo namespace system.net.http.headers in alto. Bearer = chi possiede il token
            // Con preferences.get prendo valori settati con login. String.empty valore di default vuoto
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            // è una get request -> get async
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/users/profileimage");
            return JsonConvert.DeserializeObject<ProfileImage>(response);
        }

        public static async Task<bool> UploadUserImage(byte[] imageArray)
        {
            // simile a login, formato byte array, non prende json ma serve access token
            // in generale per metodi, copia i simili (get/post, serve accesstoken?, se return oggetto o lista...) e fai sostituzioni
            // la post usa form data
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));

            var content = new MultipartFormDataContent();
            //aggiungo content, key name e file name
            content.Add(new ByteArrayContent(imageArray),"Image","image.jpg");

            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/users/uploadphoto", content);
            return true;
        }

        public static async Task<List<Category>> GetCategories() 
        {       
            // a seconda di quello che restituisce, cambiare tipo accanto a task (e in return)
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/categories");
            // restituisce lista di prodotti
            return JsonConvert.DeserializeObject<List<Category>>(response);
        }

        public static async Task<List<Product>> GetProducts(string productType, string categoryId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            // copia da postman per l'url preciso. Poi concatenazione con quello che serve passare all'api
            // bisogna sostituire category e 1 nella stringa con variabili
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + 
                "api/products?productType=" + productType + "&category&categoryId=" + categoryId);
            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        public static async Task<ProductDetail> GetProductDetail(int productId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/products/" + productId);
            return JsonConvert.DeserializeObject<ProductDetail>(response);
        }

        public static async Task<bool> AddItemsInCart(ShoppingCart shoppingCart)
        {  
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(shoppingCart);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));

            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/shoppingcartitems", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<List<ShoppingCartItem>> GetShoppingCartItems(int userId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/shoppingcartitems/" + userId);
            return JsonConvert.DeserializeObject<List<ShoppingCartItem>>(response);
        }

        public static async Task<bool> UpdateCartQuantity(int productId, string action)
        {
            var httpClient = new HttpClient();
            var content = new StringContent(string.Empty);   // niente data json
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            var response = await httpClient.PutAsync(AppSettings.ApiUrl + 
                "api/shoppingcartitems?productId=" + productId + "&action=" + action, content);   // put
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<bool> PlaceOrder(Order order)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));

            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/orders", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<List<OrderByUser>> GetOrdersByUser(int userId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/orders/ordersbyuser/" + userId);
            return JsonConvert.DeserializeObject<List<OrderByUser>>(response);
        }

        public static async Task<List<OrderDetail>> GetOrderDetails(int orderId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Preferences.Get("accesstoken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/orders/orderdetails/" + orderId);
            return JsonConvert.DeserializeObject<List<OrderDetail>>(response);
        } 
    }
} 
