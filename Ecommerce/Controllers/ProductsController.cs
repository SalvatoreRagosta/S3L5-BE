using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;
using System.Linq.Dynamic;
using System.Web.Script.Serialization;
using System.IO;
using Ecommerce.Helpers;
using System.Diagnostics;
using Microsoft.AspNet.Identity;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private ProductDBContext db = new ProductDBContext();


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public string Index(FormCollection collection)
        {
            GlobalMethods.MaybeInitializeSession();

        
            if (collection["action"] == "get-all-products")
            {

                
                var pag_content = "";
                var pag_navigation = "";

                
                int page = Convert.ToInt32(collection["data[page]"]); 
                string sort = collection["data[sort]"] == "ASC" ? "asc" : "desc"; 
                string name = collection["data[name]"]; 
                int max = Convert.ToInt32(collection["data[max]"]); 
                string search = collection["data[search]"]; 

                int cur_page = page;
                page -= 1;
                int per_page = max > 1 ? max : 16;
                bool previous_btn = true;
                bool next_btn = true;
                bool first_btn = true;
                bool last_btn = true;
                int start = page * per_page;

                
                var all_items_query = db.Products
                    .Where(x => x.id != 0)
                    .OrderBy(name + " " + sort)
                    .Skip(start)
                    .Take(per_page); 

              
                var count_query = db.Products
                    .Where(x => x.id != 0); 

            
                if (search != "")
                {
                  
                    all_items_query = all_items_query.Where(x =>
                        x.name.Contains(search) ||
                        x.content.Contains(search) ||
                        x.excerpt.Contains(search)
                    );
                    count_query = count_query.Where(x =>
                        x.name.Contains(search) ||
                        x.content.Contains(search) ||
                        x.excerpt.Contains(search)
                    );
                }

              
                var all_items = all_items_query.ToList();
                int count = count_query.Count();

                if (count > 0)
                {
                   
                    int wrap_i = 0;
                    foreach (var item in all_items)
                    {
                        string featured_image = !String.IsNullOrEmpty(item.featured_image) ? "/Content/Images/Products/" + item.featured_image : "/Content/Images/dummy-shirt.png";
                        bool in_cart = SessionSingleton.Current.Cart.ContainsKey(item.id);

                        wrap_i++;
                        if (wrap_i % 4 == 1)
                        {
                            pag_content += "<div class='clearfix'>";
                        }

                        pag_content += 
                        "<div class='col-sm-3 item-" + item.id + "'>" +
                            "<div class='panel panel-default'>" +
                                "<div class='panel-heading item-name'>" +
                                    item.name +
                                "</div>" +
                                "<div class='panel-body p-0 m-0'>" +
                                    "<a href='/Products/Details/" + item.id + "'><img src='" + featured_image + "' width='100%' class='img-responsive item-featured' /></a>" +
                                    "<div class='list-group m-0'>" +
                                        "<div class='list-group-item b-0 b-t'>" +
                                            "<i class='fa fa-calendar-o fa-2x pull-left ml-r'></i>" +
                                            "<p class='list-group-item-text'>Price</p>" +
                                            "<h4 class='list-group-item-heading'>$<span class='item-price'>" + item.price + "</span></h4>" +
                                        "</div>" +
                                        "<div class='list-group-item b-0 b-t'>" +
                                                   "<i class='fa fa-calendar fa-2x pull-left ml-r'></i>" +
                                            "<p class='list-group-item-text'>On Stock</p>" +
                                            "<h4 class='list-group-item-heading item-stock'>" + item.quantity + "</h4>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                                "<div class='panel-footer'>" +
                                    "<form method='post' action='/Cart/Actions'>";
				                        if (!in_cart)
                                        {
                                            pag_content +=
                                            "<div class='input-group'>" +
                                                "<span class='input-group-addon'>Qty</span>" + 
                                                "<input type='number' id='quantity' min='1' max='" + item.quantity + "' class='form-control' name='quantity' value='1' />" +
                                                "<div class='input-group-btn'>" +
                                                    "<button type='submit' class='btn btn-success'>Add To Cart</button>" +
                                                "</div>" + 
                                            "</div>";
                                        }
                                        else
                                        {
                                            pag_content += "<button type='submit' class='btn btn-block btn-danger'>Remove from Cart</button>";
                                        }

                                        string action = in_cart ? "remove-item-from-cart" : "add-item-to-cart";
                                        pag_content += 
			                            "<input type='hidden' name='action' value='" + action + "' />" +
                                        "<input type='hidden' name='item_id' value='" + item.id + "' />" +
                                    "</form>" +
                                 "</div>" +
                            "</div>" +
                        "</div>";

                        if (wrap_i % 4 == 0)
                        {
                            pag_content += "</div>";
                        }
                    }

                    if (wrap_i % 4 != 0)
                    {
                        pag_content += "</div>";
                    }
                }
                else
                {
             '
                    pag_content += "<p class='p-d bg-danger'>No items found</p>";
                }

                pag_content = pag_content + "<br class = 'clear' />";
                pag_navigation = GlobalMethods.Pagination(count, per_page, cur_page, first_btn, previous_btn, next_btn, last_btn);

              '
                var response = new Dictionary<string, string> {
                    { "content", pag_content },
                    { "navigation", pag_navigation }
                };

           '
                string json = new JavaScriptSerializer().Serialize(response);
                return json;
            }

            return null;
        }
        
'
        [HttpGet]
        public ActionResult UserItems()
        {
        '
            return View();
        }

        [HttpPost]
        public string UserItems(FormCollection collection)
        {
         '
            var pag_content = "";
            var pag_navigation = "";

    '
            int page = Convert.ToInt32(collection["data[page]"]); '
            string sort = collection["data[th_sort]"] == "ASC" ? "asc" : "desc"; '
            string name = collection["data[th_name]"]; 
            int max = Convert.ToInt32(collection["data[max]"]); 
            string search = collection["data[search]"]; 
            string current_user_id = User.Identity.GetUserId(); 

            int cur_page = page;
            page -= 1;
            int per_page = max > 1 ? max : 16;
            bool previous_btn = true;
            bool next_btn = true;
            bool first_btn = true;
            bool last_btn = true;
            int start = page * per_page;

          
            var all_items_query = db.Products
                .Where(x => x.id != 0 && x.author.ToString() == current_user_id)
                .OrderBy(name + " " + sort)
                .Skip(start)
                .Take(per_page); 

            /* Get total items in our database */
            var count_query = db.Products
                .Where(x => x.id != 0 && x.author.ToString() == current_user_id); /* Get total user products count. */

            /* If there is a search keyword, we search through the database for possible matches*/
            if (search != "")
            {
                /* The "Contains" method matches records using the LIKE %keyword% format */
                all_items_query = all_items_query.Where(x =>
                    x.name.Contains(search) ||
                    x.content.Contains(search) ||
                    x.excerpt.Contains(search)
                );
                count_query = count_query.Where(x =>
                    x.name.Contains(search) ||
                    x.content.Contains(search) ||
                    x.excerpt.Contains(search)
                );
            }

            /* We now fetch the data from our database */
            var all_items = all_items_query.ToList();
            int count = count_query.Count();

            if (count > 0)
            {
                /* Loop through each item to create views */
                foreach (var item in all_items)
                {
                    string featured_image = !String.IsNullOrEmpty(item.featured_image) ? "/Content/Images/Products/" + item.featured_image : "/Content/Images/dummy-shirt.png";
			        string status = item.status == 1 ? "Active" : "Inactive";
			
			        pag_content += 
                    "<tr>" +
                        "<td><a href= '/Products/Edit/" + item.id + "'><img src= '" + featured_image + "' width ='100' /></a></td>" + 
				        "<td>" + item.name + "</td>" +
                        "<td>$" + item.price + "</td>" +
                        "<td>" + item.status + "</td>" +
                        "<td>" + item.date + "</td>" +
                        "<td>" + item.quantity + "</td>" + 
				        "<td>" + 
					        "<a href= '/Products/Edit/" + item.id + "' class='text-success'><span class='glyphicon glyphicon-pencil' title='Edit'></span></a> &nbsp; &nbsp;" +
                            "<a href='#_' class='text-danger delete-product' item_id='" + item.id + "'><span class='glyphicon glyphicon-remove' title='Delete'></span></a>" + 
                        "</td>" + 
                    "</tr>";
                }
            }
            else
            {
                /* Show a message if no items were found */
                pag_content += "<tr><td class='p-d bg-danger' colspan='7'>No items found</td></tr>";
            }

            pag_content = pag_content + "<br class = 'clear' />";
            pag_navigation = GlobalMethods.Pagination(count, per_page, cur_page, first_btn, previous_btn, next_btn, last_btn);

            /* Let's put our variables in a dictionary */
            var response = new Dictionary<string, string> {
                { "content", pag_content },
                { "navigation", pag_navigation }
            };

            /* Then we return the Dictionary in json format to our front-end */
            string json = new JavaScriptSerializer().Serialize(response);
            return json;
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            GlobalMethods.MaybeInitializeSession();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Session = SessionSingleton.Current.Cart;

            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public int Create([Bind(Include = "id,name,content,excerpt,date,price,quantity,status,author,images,featured_image")] Product product)
        {
            if (ModelState.IsValid)
            {  
                string current_user_id = User.Identity.GetUserId(); /* Set logged-in user id as the author of the product. */
                product.author = current_user_id;

                db.Products.Add(product);
                db.SaveChanges();

                return product.id;
            }

            return 0;
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            string current_user_id = User.Identity.GetUserId(); /* Get ID of currently logged-in user.*/

            /* Check if product exists and if current user owns the product */
            if (product == null || product.author.ToString() != current_user_id)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Edit([Bind(Include = "id,name,content,excerpt,date,price,quantity,status,author,images")] Product product)
        {
            /* Query for the current data */
            var original_data_detached = db.Products.AsNoTracking().Where(P => P.id == product.id).FirstOrDefault();

            /* Let's put our variables in a dictionary */
            var response = new Dictionary<string, string> {
                { "status", "0" },
                { "message", "" },
                { "errors", "" },
                { "images", "" }
            };

            if (ModelState.IsValid)
            {
                /* Image uploade handler */
                try
                {
                    HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files; /* Retrieve files from form request */
                    string path = "/Content/Images/Products/"; /* Location to store images */
                    List<string> images = new List<string>(); /* Initialize an empty list for our images */

                    /* Check if Products Directory exists */
                    if (!Directory.Exists(path))
                    {
                        /* Create Products directory if not exists */
                        Directory.CreateDirectory(Server.MapPath("~") + path);
                    }

                    /* Process each image */
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        HttpPostedFile hpf = hfc[i];
                        if (hpf.ContentLength > 0)
                        {
                            string file_name_original = Path.GetFileNameWithoutExtension(hpf.FileName); /* Original file name with extension */
                            string file_name = GlobalMethods.GenerateRandomString(20); /* Generate a random string for our image name */
                            string file_extension = Path.GetExtension(hpf.FileName); /* Get file extension */
                            string file_path_with_file_name = path + file_name + file_extension; /* Final image path */

                            /* Append single image to our images list */
                            images.Add(file_name + file_extension);

                            /* Move files to the specified directory on the "path" variable */
                            hpf.SaveAs(Server.MapPath(file_path_with_file_name));
                        }
                    }

                    /* If there is an image uploaded, let's include them to the response dictionary */
                    if (images.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(original_data_detached.images))
                        {
                            /* Get existing images from database then convert it to an array */
                            var old_images_array = new JavaScriptSerializer().Deserialize<dynamic>(original_data_detached.images);

                            /* Add old images to the new images list variable*/
                            foreach (var image in old_images_array)
                            {
                                images.Add(image);
                            }
                        }

                        /* Convert images list to json */
                        response["images"] = new JavaScriptSerializer().Serialize(images);

                        product.images = response["images"];
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                string current_user_id = User.Identity.GetUserId();
                product.author = current_user_id;

                db.Entry(product).State = EntityState.Modified;

                /* Do not update images field if there are no new posted images */
                if (String.IsNullOrEmpty(product.images))
                {
                    db.DontUpdateProperty<Product>("images");
                }

                /* Do not update featured image, we do it in a separate method called "SetFeaturedImage()" */
                db.DontUpdateProperty<Product>("featured_image");

                db.SaveChanges();
                response["status"] = "1";
                response["message"] = "Item successfully updated";
            }

            /* Then we return the Dictionary in json format to our front-end */
            string json = new JavaScriptSerializer().Serialize(response);
            return json;
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(FormCollection collection)
        {
            var item_id = Convert.ToInt32(collection["item_id"]);

            /* Let's create dictionary with default values */
            var response = new Dictionary<string, string> {
                { "status", "0" },
                { "msg", "Delete failed, please try again" }
            };

            Product product = db.Products.Find(item_id);
            string current_user_id = User.Identity.GetUserId(); /* Get ID of currently logged-in user.*/

            /* Check if product exists and if current user owns the product */
            if (product != null || product.author.ToString() == current_user_id)
            {
                /* Parse the JSON string then loop through it */
                dynamic images = new JavaScriptSerializer().Deserialize<dynamic>(product.images);
                foreach (var image in images)
                {
                    string image_path = Request.MapPath("/Content/Images/Products/" + image); /* Path of the image in the server */
                    /* Check if the image exists in the server. */
                    if (System.IO.File.Exists(image_path))
                    {
                        System.IO.File.Delete(image_path); /* Delete the image from the server */
                    }
                }

                db.Products.Remove(product);
                db.SaveChanges();

                response["status"] = "1";
                response["msg"] = "Deleted Successfully";

            }
            else
            {
                response["msg"] = "You are not allowed to delete this product";
            }

            /* Then we return the Dictionary in json format to our front-end */
            string json = new JavaScriptSerializer().Serialize(response);
            return json;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        [HttpPost]
        public string DeleteImage(FormCollection collection)
        {
            /* Let's create dictionary with default values */
            var response = new Dictionary<string, string> {
                { "status", "0" },
                { "message", "An error occurred, please try again" }
            };

            string image_name = collection["image"]; /* The image to delete */
            string image_path = Request.MapPath("/Content/Images/Products/" + image_name); /* Path of the image in the server */
            int item_id = Convert.ToInt32(collection["item_id"]); /* The ID of the product where we wanto to delete an image */

            Product product = db.Products.Find(item_id); /* Query for the product */
            IEnumerable<dynamic> images_array = new JavaScriptSerializer().Deserialize<dynamic>(product.images); /* Get existing images contained json string then convert it to an array */
            
            /* Check if the image exists in the server. */           
            if (System.IO.File.Exists(image_path))
            {
                System.IO.File.Delete(image_path); /* Delete the image from the server */
            }

            images_array = images_array.Where(val => val != image_name).ToArray(); /* Remove the image from the array of images */
            product.images = new JavaScriptSerializer().Serialize(images_array); /* Json encode the updated image array then set it as the new value for our product.images field */

            /* Let's return a successfull message and status */
            response["status"] = "1";
            response["message"] = "Image Successfully Deleted";

            /* Proceed with the update */
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            /* Then we return the Dictionary in json format to our front-end */
            string json = new JavaScriptSerializer().Serialize(response);
            return json;
        }

        [HttpPost]
        public string SetFeaturedImage(FormCollection collection)
        {
            /* Let's create dictionary with default values */
            var response = new Dictionary<string, string> {
                { "status", "0" },
                { "message", "An error occurred, please try again" }
            };

            string image_name = collection["image"]; /* The image to delete */
            string image_path = Request.MapPath("/Content/Images/Products/" + image_name); /* Path of the image in the server */
            int item_id = Convert.ToInt32(collection["item_id"]); /* The ID of the product where we wanto to delete an image */

            Product product = db.Products.Find(item_id); /* Query for the product */

            /* Check if the image exists in the server. */
            if (System.IO.File.Exists(image_path))
            {
                product.featured_image = image_name; /* Set image as the new value for our product.featured_image field */

                /* Let's return a successfull message and status */
                response["status"] = "1";
                response["message"] = "Image Successfully Set as Featured";
            }
            else {
                response["message"] = "The image no longer exists from the server";
            }

            /* Proceed with the update */
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            /* Then we return the Dictionary in json format to our front-end */
            string json = new JavaScriptSerializer().Serialize(response);
            return json;
        }

    }
}
