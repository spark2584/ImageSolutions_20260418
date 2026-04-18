using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmailMarketing
{
    class FedExSample
    {
        private const string ClientId = "l73f3eb9a189d340898fb8d81292241dae";
        private const string ClientSecret = "113cceb0bb49407fa71eaaa702d52a4d";
        private const string AccountNumber = "740561073";
        private const string BaseUrl = "https://apis-sandbox.fedex.com";

        private const string AccountState = "OH";
        private const string AccountCity = "Columbus";
        private const string AccountZip = "43215";
        private const string AccountAddress = "123 Main St";

        private const string ExampleTrackingNumber = "453779581060";

        //static void Main() => Execute().GetAwaiter().GetResult();

        // .NET Framework safety for HTTPS
        public static async Task Execute()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                Console.WriteLine("Getting OAuth token...");
                var token = await GetAccessTokenAsync(ClientId, ClientSecret);
                Console.WriteLine("Token acquired.\n");

                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                using (var http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) })
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    http.DefaultRequestHeaders.Accept.Clear();
                    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    http.DefaultRequestHeaders.Add("X-locale", "en_US");

                    // 1) Rate shopping
                    Console.WriteLine("Requesting rate quotes...");
                    await GetRatesAsync(http, AccountNumber);

                    // 2) Create label (download base64 to PDF if present)
                    Console.WriteLine("\nCreating shipment + label...");
                    await CreateLabelAsync(http, AccountNumber);

                    // 3) Tracking status
                    Console.WriteLine("\nTracking status...");
                    await TrackAsync(http, ExampleTrackingNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERR: " + ex);
            }

            Console.WriteLine("\nDone. Press any key to exit.");
            Console.ReadKey();
        }

        // === OAuth2 client_credentials ===
        static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret)
        {
            using (var http = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type", "client_credentials"),
                    new KeyValuePair<string,string>("client_id", clientId),
                    new KeyValuePair<string,string>("client_secret", clientSecret),
                });

                using (var resp = await http.PostAsync("/oauth/token", form))
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception($"OAuth failed {resp.StatusCode}: {body}");

                    var json = JObject.Parse(body);
                    return json.Value<string>("access_token")
                           ?? throw new Exception("No access_token in OAuth response.");
                }
            }
        }

        // === Rate shopping ===
        static async Task GetRatesAsync(HttpClient http, string accountNumber)
        {
            var payload = new
            {
                accountNumber = new { value = accountNumber },
                requestedShipment = new
                {
                    serviceType = "FEDEX_GROUND",
                    shipper = new
                    {
                        contact = new { personName = "Your Company Name", phoneNumber = "5555555555" },
                        address = new
                        {
                            streetLines = new[] { AccountAddress },
                            city = AccountCity,
                            stateOrProvinceCode = AccountState, 
                            postalCode = AccountZip,
                            countryCode = "US"
                        }
                    },
                    recipient = new
                    {
                        contact = new { personName = "Recipient Name", phoneNumber = "5555550000" },
                        address = new
                        {
                            streetLines = new[] { "30 FedEx Pkwy" },
                            city = "Memphis",
                            stateOrProvinceCode = "TN",
                            postalCode = "38116",
                            countryCode = "US",
                            residential = false
                        }
                    },
                    pickupType = "DROPOFF_AT_FEDEX_LOCATION",
                    packagingType = "YOUR_PACKAGING",
                    preferredCurrency = "USD",
                    rateRequestType = new[] { "ACCOUNT", "LIST" },
                    requestedPackageLineItems = new[]
                    {
                new {
                    weight = new { units = "LB", value = 2.0 },
                    dimensions = new { length = 10, width = 6, height = 4, units = "IN" }
                }
            }
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            
            using (var resp = await http.PostAsync("/rate/v1/rates/quotes", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                var body = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"Rate HTTP {(int)resp.StatusCode}");

                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine(body);
                    return;
                }
               
                try
                {
                    var doc = JObject.Parse(body);

                    var rateDetails = doc.SelectToken("output.rateReplyDetails") ?? doc.SelectToken("rateReplyDetails") ?? doc.SelectToken("output");

                    if (rateDetails is JArray arr && arr.Count > 0)
                    {
                        foreach (var rd in arr.Take(5))
                        {
                            try
                            {
                                var svc = rd.Value<string>("serviceType") ?? "(service?)";

                                var priceDetails = rd.SelectToken("ratedShipmentDetails");
                                if (priceDetails is JArray priceArr && priceArr.Count > 0)
                                {
                                    var firstPrice = priceArr[0];
                                    var chargeTokens = new[]
                                    {
                                firstPrice.SelectToken("shipmentRateDetail.totalNetCharge"),
                                firstPrice.SelectToken("totalNetCharge"),
                                firstPrice.SelectToken("rateDetail.totalNetCharge"),
                                firstPrice.SelectToken("shipmentRateDetail.totalBaseCharge"),
                                firstPrice.SelectToken("totalBaseCharge")
                            };

                                    JToken netCharge = null;
                                    foreach (var token in chargeTokens)
                                    {
                                        if (token != null)
                                        {
                                            netCharge = token;
                                            break;
                                        }
                                    }

                                    if (netCharge != null)
                                    {
                                        string amount = null;
                                        string currency = null;

                                        if (netCharge is JObject chargeObj)
                                        {
                                            amount = chargeObj.Value<string>("amount") ?? chargeObj.Value<decimal?>("amount")?.ToString();
                                            currency = chargeObj.Value<string>("currency");
                                        }
                                        else if (netCharge is JValue chargeVal)
                                        {
                                            amount = chargeVal.ToString();
                                            currency = "USD";
                                        }

                                        Console.WriteLine($"Price: {amount} {currency}");

                                        Console.WriteLine($"Debug - Charge structure: {netCharge}");
                                    }
                                    else
                                    {
                                        if (firstPrice is JObject fpObj)
                                        {
                                            foreach (var prop in fpObj.Properties().Take(5))
                                            {
                                                Console.WriteLine($"{prop.Name}: {prop.Value.Type}");
                                            }
                                        }
                                    }
                                }
                                Console.WriteLine();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"  Error parsing rate detail: {ex.Message}");
                            }
                        }
                    }
                    else if (rateDetails != null)
                    {
                        Console.WriteLine("Rate details found but not in expected array format:");
                        Console.WriteLine(rateDetails.ToString());
                    }
                    else
                    {
                        Console.WriteLine("No rate details found. Full response:");
                        Console.WriteLine(body);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON response: {ex.Message}");
                    Console.WriteLine("Raw response:");
                    Console.WriteLine(body);
                }
            }
        }
        static async Task CreateLabelAsync(HttpClient http, string accountNumber)
        {
            // Minimal domestic example; edit addresses/contact as needed.
            // To get a downloadable link instead of base64, set labelResponseOptions = "URL_ONLY".
            var payload = new
            {
                labelResponseOptions = "LABEL", // or "URL_ONLY"
                accountNumber = new { value = accountNumber },
                requestedShipment = new
                {
                    shipDatestamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    serviceType = "FEDEX_GROUND",
                    packagingType = "YOUR_PACKAGING",
                    pickupType = "DROPOFF_AT_FEDEX_LOCATION",
                    shipper = new
                    {
                        contact = new { personName = "Your Company Name", phoneNumber = "5555555555" },
                        address = new
                        {
                            streetLines = new[] { AccountAddress },
                            city = AccountCity,
                            stateOrProvinceCode = AccountState,  // Now using OH (or CA)
                            postalCode = AccountZip,
                            countryCode = "US"
                        }
                    },
                    recipients = new[]
                    {
                        new {
                            contact = new { personName = "Recipient Name", phoneNumber = "5555550000" },
                            address = new {
                                streetLines = new[] { "30 FedEx Pkwy" },
                                city = "Memphis",
                                stateOrProvinceCode = "TN",
                                postalCode = "38116",
                                countryCode = "US",
                                residential = false
                            }
                        }
                    },
                    shippingChargesPayment = new { paymentType = "SENDER" },
                    requestedPackageLineItems = new[]
                    {
                        new {
                            weight = new { units = "LB", value = 2.0 },
                            dimensions = new { length = 10, width = 6, height = 4, units = "IN" }
                        }
                    },
                    labelSpecification = new
                    {
                        imageType = "PDF",              // ZPLII, PNG, PDF, etc.
                        labelStockType = "PAPER_4X6"    // or PAPER_85X11_TOP_HALF_LABEL, etc.
                    }
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            using (var resp = await http.PostAsync("/ship/v1/shipments", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                var body = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"Ship HTTP {(int)resp.StatusCode}");
                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine(body);
                    return;
                }

                var doc = JObject.Parse(body);

                // Print tracking numbers found
                var tracking = doc.SelectTokens("$..trackingNumber").Select(t => t.ToString()).Distinct().ToList();
                if (tracking.Any())
                    Console.WriteLine("Tracking number(s): " + string.Join(", ", tracking));

                // Try to save a base64 label if present
                var labelBase64 = doc.SelectTokens("$..encodedLabel").Select(t => t.ToString()).FirstOrDefault();
                if (!string.IsNullOrEmpty(labelBase64))
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "fedex_label.pdf");
                    File.WriteAllBytes(path, Convert.FromBase64String(labelBase64));
                    Console.WriteLine("Saved label to: " + path);
                }
                else
                {
                    // Or pick up a short-lived label URL if using URL_ONLY
                    var url = doc.SelectTokens("$..url").Select(t => t.ToString())
                                 .Concat(doc.SelectTokens("$..documentUrl").Select(t => t.ToString()))
                                 .FirstOrDefault();
                    if (!string.IsNullOrEmpty(url))
                        Console.WriteLine("Label URL: " + url + "  (save immediately; sandbox URLs can expire quickly)");
                }
            }
        }

        // === Tracking ===
        static async Task TrackAsync(HttpClient http, string trackingNumber)
        {
            var payload = new
            {
                trackingInfo = new[]
                {
                    new { trackingNumberInfo = new { trackingNumber = trackingNumber } }
                },
                includeDetailedScans = true
            };

            var json = JsonConvert.SerializeObject(payload);
            using (var resp = await http.PostAsync("/track/v1/trackingnumbers", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                var body = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"Track HTTP {(int)resp.StatusCode}");
                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine(body);
                    return;
                }

                var doc = JObject.Parse(body);
                var status = doc.SelectToken("$..latestStatusDetail.description")
                           ?? doc.SelectToken("$..statusDetail.description");
                if (status != null)
                    Console.WriteLine($"Status: {status}");
                else
                    Console.WriteLine(body);
            }
        }
    }
}