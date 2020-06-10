using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using WS_PosData_PMKT.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using WS_PosData_PMKT.Models.Object;
using WS_PosData_PMKT.Models.Response;
using WS_PosData_PMKT.Models.Request;
using WS_PosData_PMKT.Helpers;
using WS_PosData_PMKT.Models.Base;
using System.IO;
using System.Globalization;

namespace WS_PosData_PMKT
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "WS_PosData_PMKT" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione WS_PosData_PMKT.svc o WS_PosData_PMKT.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class WS_PosData_PMKT : IWS_PosData_PMKT
    {
        private string dbConnection = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString.ToString();

        #region WSMETODS


        public UserDataResponse AutenticateUser(LoginUserRequest request)
        {
            UserDataResponse userresponse = new UserDataResponse();
            GeneralDataUser datauser = new GeneralDataUser();

            Crypto crypto = new Crypto();

            DataTable dt = new DataTable();
            try
            {
                crypto.Cypher(ref request, 2);

                string email = request.Email;
                string psw = request.Password;



                dt = ValidateUser(email, psw);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["nPER_estatus"].ToString() == "True")
                    {
                        if (dt.Rows[0]["Licencia"].ToString() == "1")
                        {
                            DataTable dt2 = GetGeneralDataUser(dt.Rows[0]["nPER_clave"].ToString());
                            datauser.IdUser = Convert.ToInt32(dt2.Rows[0]["nPER_clave"].ToString());
                            datauser.NameEmployee = dt2.Rows[0]["cPER_nombres"].ToString();
                            datauser.LastName = dt2.Rows[0]["cPER_apellidopaterno"].ToString();
                            datauser.MothersLastName = dt2.Rows[0]["cPER_apellidomaterno"].ToString();
                            datauser.IdStaffType = dt2.Rows[0]["nTIP_clave"].ToString();
                            datauser.NameStaffType = dt2.Rows[0]["cTIP_nombre"].ToString();
                            datauser.Email = dt2.Rows[0]["cPER_cuentacorreo"].ToString();

                            userresponse.DataUser = datauser;
                            userresponse.Message = "Proceso exitoso";
                            userresponse.Success = true;
                        }
                        else
                        {
                            userresponse.Code = "A003";
                            userresponse.Message = "Cuenta sin licencia.";
                            userresponse.Success = false;
                        }
                    }
                    else
                    {
                        userresponse.Code = "A002";
                        userresponse.Message = "Cuenta no activa.";
                        userresponse.Success = false;
                    }
                }
                else
                {
                    userresponse.Code = "A001";
                    userresponse.Message = "Error de credenciales: la cuenta de usuario y/o la contraseña son incorrectos.";
                    userresponse.Success = false;
                }
            }
            catch (Exception ex)
            {
                userresponse.Code = "A000";
                userresponse.Success = false;
                userresponse.Message = ex.StackTrace + ". " + ex.Message;
            }

            return userresponse;
        }

        public ListPromosResponse AvailablePromosUser(string user)
        {
            ListPromosResponse listPromos = new ListPromosResponse();
            List<InfoPromosUser> lstipu = new List<InfoPromosUser>();
            List<Events> lstep = new List<Events>();
            DataTable dt = new DataTable();
            bool baddesign = false;
            try
            {
                dt = GetListPromosUser(user);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow p in dt.Rows)
                    {
                        DataTable dtep = GetEventsForPromo(p["cPRO_clave"].ToString());

                        if (dtep != null && dtep.Rows.Count > 0) // verificar si la promo tiene eventos asociados
                        {
                            lstep.Clear();
                            foreach (DataRow e in dtep.Rows)
                            {
                                lstep.Add(new Events
                                {
                                    IdTypeEvent = e["nTIM_clave"].ToString(),
                                    NameTypeEvent = e["cTIM_nombre"].ToString()
                                });
                            }
                            lstipu.Add(new InfoPromosUser
                            {
                                IdClient = p["cCLI_clave"].ToString(),
                                NameClient = p["cCLI_nombre"].ToString(),
                                IdPromo = p["cPRO_clave"].ToString(),
                                NamePromo = p["cPRO_nombre"].ToString(),
                                AvailableEvents = lstep,
                                ValidateCheckIn = Convert.ToBoolean(p["bPRO_validarcheckin"].ToString()),
                                URLReceptionData = p["cPRO_evidenciafotografica"].ToString()
                            });
                        }

                        else
                        {
                            baddesign = true;
                        }

                    }
                    if (baddesign)
                    {
                        listPromos.Code = "P002";
                        listPromos.Message = "Error general de Promoción: Promoción mal diseñada.";
                        listPromos.Success = false;
                    }
                    else
                    {
                        listPromos.AvailablePromosUser = lstipu;
                        listPromos.Message = "Proceso exitoso";
                        listPromos.Success = true;
                    }
                }
                else
                {
                    listPromos.Code = "P001";
                    listPromos.Message = "Cuenta sin Promociones.";
                    listPromos.Success = false;
                }
            }
            catch (Exception ex)
            {
                listPromos.Code = "A000";
                listPromos.Success = false;
                listPromos.Message = ex.StackTrace + ". " + ex.Message;
            }

            return listPromos;
        }

        public DetailEventsPromoResponse DetailEventsPromo(string IdPromo)
        {
            DetailEventsPromoResponse detaileventsresponse = new DetailEventsPromoResponse();

            DetailEventsPromo detailevent = new Models.Object.DetailEventsPromo();
            detailevent.ListNoVisitMotive = new List<NoVisitMotive>();
            detailevent.ListSurvey = new List<Survey>();
            detailevent.ListTypeEvidence = new List<PhotographicEvidence>();
            detailevent.ListProductsByCategory = new List<Products>();
            detailevent.ListKPI = new List<KPI>();
            detailevent.ListUbication = new List<UbicationProduct>();
            detailevent.ListTypeMaterial = new List<TypeMaterial>();

            int countEmptyTables = 0;
            try
            {
                DataTable dt = GetModulesPromo(IdPromo);
                DataSet ds = GetDataDetailPromo(IdPromo);

                foreach (DataRow te in dt.Rows)
                {

                    int i = int.Parse(te["nTIM_clave"].ToString());
                    switch (i)
                    {
                        case 1:
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                
                                countEmptyTables += 1;
                            }
                            else
                            {
                                detailevent.ListNoVisitMotive = FillListNoVisit(ds.Tables[0]);
                            }
                            break;
                        case 3:
                            if (ds.Tables[1].Rows.Count == 0)
                            {
                               
                                countEmptyTables += 1;
                            }
                            else
                            {
                                detailevent.ListSurvey = FillListSurvey(ds.Tables[1]);
                            }
                            break;
                        case 4:
                            if (ds.Tables[2].Rows.Count == 0)
                            {
                                
                                countEmptyTables += 1;
                            }
                            else
                            {
                                detailevent.ListTypeEvidence = FillTypeEvidence(ds.Tables[2]);

                            }
                            break;
                        case 5:
                            if (ds.Tables[3].Rows.Count == 0)
                            {
                                
                                countEmptyTables += 1;
                            }
                            if (ds.Tables[6].Rows.Count == 0)
                            {
                                
                                countEmptyTables += 1;
                            }
                            else
                            {
                                detailevent.ListProductsByCategory = FillListProducts(ds.Tables[3]);
                                detailevent.ListKPI = FillListKPI(ds.Tables[6]);
                            }
                            break;
                        case 6:
                            for (int n = 3; n < ds.Tables.Count; n++)
                            {
                                if (ds.Tables[i].Rows.Count == 0)
                                {
                                    
                                    countEmptyTables += 1;
                                }

                            }
                            detailevent.ListProductsByCategory = new List<Products>();
                            detailevent.ListProductsByCategory = FillListProducts(ds.Tables[3]);
                            detailevent.ListUbication = FillListUbication(ds.Tables[5]);

                            break;
                        case 7:
                            if (ds.Tables[4].Rows.Count == 0)
                            {
                               
                                countEmptyTables += 1;
                            }
                            else
                            {
                                detailevent.ListTypeMaterial = FillListMaterial(ds.Tables[4]);
                            }
                            break;
                        default:
                            break;
                    }

                }


                detaileventsresponse.DetailEvents = detailevent;
                detaileventsresponse.Message = "Proceso Exitoso.";
                detaileventsresponse.Success = true;


                if (ds.Tables.Count == countEmptyTables)
                {
                    detaileventsresponse.Code = "MS001";
                    detaileventsresponse.Message = "Estructura de datos vacía";
                    detaileventsresponse.Success = true;
                }


            }
            catch (Exception ex)
            {
                detaileventsresponse.Code = "P002";
                detaileventsresponse.Message = "Error general de Promoción: Promoción mal diseñada. " + ex.StackTrace;
                detaileventsresponse.Success = false;
            }

            return detaileventsresponse;
        }

        public DataRoutePromoResponse DataRoutePromo(string Email, string IdPromo, string IdDay)
        {
            DataRoutePromoResponse routeResponse = new DataRoutePromoResponse();
            GeneralDataRoute dataroute = new GeneralDataRoute();
            List<InfoRoutesPromo> lstruta = new List<InfoRoutesPromo>();
            List<InfoRoutesPromo> lstOutRoute = new List<InfoRoutesPromo>();
            List<InfoRoutesPromo> lstVisited = new List<InfoRoutesPromo>();
            try
            {
                DataSet ds = GetDataRoute(Email, IdPromo, Convert.ToInt32(IdDay));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dataroute.IdRoute = ds.Tables[0].Rows[0]["cRUT_clave"].ToString();
                    dataroute.NameRoute = ds.Tables[0].Rows[0]["cRUT_nombre"].ToString();
                    dataroute.IdRegion = ds.Tables[0].Rows[0]["cREG_clave"].ToString();
                    dataroute.NameRegion = ds.Tables[0].Rows[0]["cREG_nombre"].ToString();
                    dataroute.IdCity = ds.Tables[0].Rows[0]["cCIU_clave"].ToString();
                    dataroute.NameCity = ds.Tables[0].Rows[0]["cCIU_nombre"].ToString();
                    dataroute.NameSupervisor = ds.Tables[0].Rows[0]["Supervisor"].ToString();

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[1].Rows)
                        {
                            lstruta.Add(new InfoRoutesPromo
                            {
                                IdCity = r["cCIU_clave"].ToString(),
                                IdRegion = r["cREG_clave"].ToString(),
                                IdStore = r["cTIE_clave"].ToString(),
                                NameCity = r["cCIU_nombre"].ToString(),
                                NameRegion = r["cREG_nombre"].ToString(),
                                NameStore = r["cTIE_nombre"].ToString(),
                                Latitude = r["cTIE_latitud"].ToString(),
                                Longitude = r["cTIE_longitud"].ToString(),
                                CheckInRange = r["nTIE_rangocheckin"].ToString()
                            });
                        }
                        dataroute.ListScheduledRoute = lstruta;
                        routeResponse.Success = true;
                        routeResponse.Message = "Proceso exitoso.";

                    }
                    else
                    {
                        /* routeResponse.Code = "MS001";
                         routeResponse.Success = true;
                         routeResponse.Message = "Estructura de datos vacía.";*/
                        DataTable dt = GetMotiveNoStores(Email, IdPromo, Convert.ToInt32(IdDay));
                        routeResponse.Code = dt.Rows[0]["cVAR_clave"].ToString() == "WSTextoRutaConcluida" ? "MS002" : "MS001";
                        routeResponse.Success = true;
                        routeResponse.Message = dt.Rows[0]["cVAR_valor"].ToString();

                    }
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[2].Rows)
                        {
                            lstOutRoute.Add(new InfoRoutesPromo
                            {
                                IdCity = r["cCIU_clave"].ToString(),
                                IdRegion = r["cREG_clave"].ToString(),
                                IdStore = r["cTIE_clave"].ToString(),
                                NameCity = r["cCIU_nombre"].ToString(),
                                NameRegion = r["cREG_nombre"].ToString(),
                                NameStore = r["cTIE_nombre"].ToString(),
                                Latitude = r["cTIE_latitud"].ToString(),
                                Longitude = r["cTIE_longitud"].ToString(),
                                CheckInRange = r["nTIE_rangocheckin"].ToString()
                            });
                        }
                        dataroute.ListScopePromoRoute = lstOutRoute;
                    }
                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[3].Rows)
                        {
                            lstVisited.Add(new InfoRoutesPromo
                            {
                                IdCity = r["cCIU_clave"].ToString(),
                                IdRegion = r["cREG_clave"].ToString(),
                                IdStore = r["cTIE_clave"].ToString(),
                                NameCity = r["cCIU_nombre"].ToString(),
                                NameRegion = r["cREG_nombre"].ToString(),
                                NameStore = r["cTIE_nombre"].ToString(),
                                Latitude = r["cTIE_latitud"].ToString(),
                                Longitude = r["cTIE_longitud"].ToString(),
                                CheckInRange = r["nTIE_rangocheckin"].ToString()
                            });
                        }
                        dataroute.ListVisitedStores = lstVisited;
                    }
                    routeResponse.DataRoute = dataroute;
                }
                else
                {
                    routeResponse.Code = "R001";
                    routeResponse.Success = false;
                    routeResponse.Message = "Sin asignación de Ruta: la cuenta de usuario no tiene asociada una ruta.";
                }

            }
            catch (Exception ex)
            {
                routeResponse.Code = "P002";
                routeResponse.Success = false;
                routeResponse.Message = "Error general de Promoción: Promoción mal diseñada. " + ex.StackTrace + ex.Message;
            }
            return routeResponse;
        }


        public LoginUserRequest ObtieneDatosEmcriptados()
        {
            LoginUserRequest response = new LoginUserRequest() { Email = "aperez@miportal-promarket.mx", Password = "26202", getfechaactual = DateTime.Now };
            Crypto c = new Crypto();
            c.Cypher(response, 1);

            return response;
        }

        public ResponseBase RegisterCheckin(RegisterCheckinRequest request)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                if (request.IdMotive != null)
                {
                    SaveMotiveNoVisit(request);
                }
                else
                {
                    SaveCheckinData(request);
                }
                response.Code = "S001";
                response.Message = "Sincronización exitosa.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Code = "S002";
                response.Message = "Error de sincronización: No se pudo realizar el proceso de sincronización. " + ex.StackTrace + ex.Message;
                response.Success = false;
            }

            return response;
        }

        public ResponseBase RegisterCheckout(RegisterCheckoutRequest request)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                SaveCheckoutData(request);

                response.Code = "S001";
                response.Message = "Sincronización exitosa.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Code = "S002";
                response.Message = "Error de sincronización: No se pudo realizar el proceso de sincronización. " + ex.StackTrace + ex.Message;
                response.Success = false;
            }

            return response;
        }

        public ResponseBase RegisterSurvey(RegisterSurveyRequest request)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                SaveSurveyData(request);

                response.Code = "S001";
                response.Message = "Sincronización exitosa." + request.DateTimeStartEvent.ToString();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Code = "S002";
                response.Message = "Error de sincronización: No se pudo realizar el proceso de sincronización. " + ex.StackTrace + ex.Message;
                response.Success = false;
            }

            return response;
        }

        public ResponseBase ResetPassword(string Email)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                if (ValidateEmail(Email) == true)
                {
                    GenerateSendPassword(Email);
                    response.Message = "Proceso Exitoso.";
                    response.Success = true;
                }
                else
                {
                    response.Code = "A001";
                    response.Message = "Error de credenciales: la cuenta de usuario y/o la contraseña son incorrectos.";
                    response.Success = false;
                }

            }
            catch (Exception ex)
            {

                response.Code = "A000";
                response.Success = false;
                response.Message = ex.StackTrace + ". " + ex.Message;

            }

            return response;
        }

        public ResponseBase RegisterSale(RegisterSalesRequest request)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                string idsync = request.IdSync;
                string idstore = request.IdStore;
                DateTime startdate = request.StartEventDate;
                DateTime enddate = request.StartEventDate;

                foreach (DataProductKPI pkpi in request.ListProductsKPI)
                {
                    SaveSaleData(idsync, idstore, pkpi, startdate, enddate);
                }
                response.Code = "S001";
                response.Message = "Sincronización exitosa.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Code = "S002";
                response.Message = "Error de sincronización: No se pudo realizar el proceso de sincronización. " + ex.StackTrace + ex.Message;
                response.Success = false;
            }

            return response;
        }

        public ResponseBase RegisterAudit(RegisterAuditRequest request)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                string idsync = request.IdSync;
                string idstore = request.IdStore;
                DateTime startdate = request.StartEventDate;
                DateTime enddate = request.StartEventDate;
                foreach (DataProductAudit pa in request.ListProductsAudit)
                {
                    SaveAuditData(idsync, idstore, pa, startdate, enddate);

                }
                response.Code = "S001";
                response.Message = "Sincronización exitosa. ";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Code = "S002";
                response.Message = "Error de sincronización: No se pudo realizar el proceso de sincronización. " + ex.StackTrace + ex.Message;
                response.Success = false;
            }

            return response;
        }


        public ResponseBase RegisterPlacement(RegisterPlacementRequest request)
        {
            ResponseBase response = new ResponseBase();
            try
            {
                string idsync = request.IdSync;
                string idstore = request.IdStore;
                DateTime startdate = request.StartEventDate;
                DateTime enddate = request.StartEventDate;

                foreach (DataMaterialPlacement mp in request.ListMaterialPlacement)
                {
                    SavePlacementData(idsync, idstore, mp, startdate, enddate);
                }
                response.Code = "S001";
                response.Message = "Sincronización exitosa.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Code = "S002";
                response.Message = "Error de sincronización: No se pudo realizar el proceso de sincronización. " + ex.StackTrace + ex.Message;
                response.Success = false;
            }

            return response;
        }



        #endregion

        #region HELPERMETODS
        private DataTable ValidateUser(string email, string psw)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "ValidateUser"));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", email));
                    command.Parameters.Add(new SqlParameter("cPER_contrasena", psw));

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private DataTable GetGeneralDataUser(string user)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "GetGeneralDataUser"));
                    command.Parameters.Add(new SqlParameter("nPER_clave", user));

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private DataTable GetListPromosUser(string user)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "GetListPromosUser"));
                    command.Parameters.Add(new SqlParameter("nPER_clave", user));

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private DataTable GetEventsForPromo(string p)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "GetEventsForPromo"));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", p));

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private DataTable GetModulesPromo(string p)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "ActiveModulesPromo"));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", p));

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private DataSet GetDataDetailPromo(string idPromo)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "DetailEvents"));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", idPromo));

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        private List<KPI> FillListKPI(DataTable dataTable)
        {
            List<KPI> lstKPI = new List<KPI>();

            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstKPI.Add(new KPI { IdKPI = r["cKPI_clave"].ToString(), NameKPI = r["cKPI_nombre"].ToString() });
                }
            }
            catch (Exception ex) { throw ex; }
            return lstKPI;
        }

        private List<UbicationProduct> FillListUbication(DataTable dataTable)
        {
            List<UbicationProduct> lstUbication = new List<UbicationProduct>();
            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstUbication.Add(new UbicationProduct { IdUbication = r["cUPR_clave"].ToString(), NameUbication = r["cUPR_nombre"].ToString() });
                }
            }
            catch (Exception ex) { throw ex; }
            return lstUbication;
        }

        private List<TypeMaterial> FillListMaterial(DataTable dataTable)
        {

            List<TypeMaterial> lstMaterial = new List<TypeMaterial>();
            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstMaterial.Add(new TypeMaterial { IdTypeMaterial = r["cTIM_clave"].ToString(), NameTypeMaterial = r["cTIM_nombre"].ToString() });
                }
            }
            catch (Exception ex) { throw ex; }
            return lstMaterial;
        }

        private List<Products> FillListProducts(DataTable dataTable)
        {
            List<Products> lstProducts = new List<Products>();
            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstProducts.Add(new Products
                    {
                        IdCategory = r["cCAP_clavecategoria"].ToString(),
                        NameCategory = r["cCAP_nombre"].ToString(),
                        IdProduct = r["cPRD_claveproducto"].ToString(),
                        NameProduct = r["cPRD_nombre"].ToString()
                    });
                }
            }
            catch (Exception ex) { throw ex; }
            return lstProducts;
        }

        private List<PhotographicEvidence> FillTypeEvidence(DataTable dataTable)
        {
            List<PhotographicEvidence> lstEvidence = new List<PhotographicEvidence>();
            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstEvidence.Add(new PhotographicEvidence { IdTypeEvidence = r["nTEF_clave"].ToString(), NameTypeEvidence = r["cTEF_nombre"].ToString() });
                }
            }
            catch (Exception ex) { throw ex; }
            return lstEvidence;
        }

        private List<Survey> FillListSurvey(DataTable dataTable)
        {
            List<Survey> lstSurvey = new List<Survey>();
            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstSurvey.Add(new Survey
                    {
                        IdSurvey = r["cENC_clave"].ToString(),
                        NameSurvey = r["cENC_nombre"].ToString(),
                        Question1 = r["cENC_pregunta1"].ToString(),
                        Question2 = r["cENC_pregunta2"].ToString(),
                        Question3 = r["cENC_pregunta3"].ToString(),
                        Question4 = r["cENC_pregunta4"].ToString(),
                        Question5 = r["cENC_pregunta5"].ToString(),
                        Question6 = r["cENC_pregunta6"].ToString(),
                        Question7 = r["cENC_pregunta7"].ToString(),
                        Question8 = r["cENC_pregunta8"].ToString(),
                        Question9 = r["cENC_pregunta9"].ToString(),
                        Question10 = r["cENC_pregunta10"].ToString(),
                        Question11 = r["cENC_pregunta11"].ToString(),
                        Question12 = r["cENC_pregunta12"].ToString(),
                        Question13 = r["cENC_pregunta13"].ToString(),
                        Question14 = r["cENC_pregunta14"].ToString(),
                        Question15 = r["cENC_pregunta15"].ToString()
                    });
                }

            }
            catch (Exception ex) { throw ex; }
            return lstSurvey;

        }

        private List<NoVisitMotive> FillListNoVisit(DataTable dataTable)
        {
            List<NoVisitMotive> lstNoVisit = new List<NoVisitMotive>();
            try
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    lstNoVisit.Add(new NoVisitMotive { IdMotive = r["cMNV_clave"].ToString(), NameMotive = r["cMNV_nombre"].ToString() });
                }
            }
            catch (Exception ex) { throw ex; }
            return lstNoVisit;
        }

        private DataSet GetDataRoute(string email, string idPromo, int idDay)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "DataRoute"));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", email));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", idPromo));
                    command.Parameters.Add(new SqlParameter("nDIP_clave", idDay));

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        private void SaveCheckinData(RegisterCheckinRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SaveCheckinData"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", request.IdSync));
                    command.Parameters.Add(new SqlParameter("cRUT_clave", request.IdRoute));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", request.IdPromo));
                    command.Parameters.Add(new SqlParameter("nDIP_clave", request.IdDay));
                    command.Parameters.Add(new SqlParameter("cTIE_clave", request.IdStore));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", request.Email));
                    command.Parameters.Add(new SqlParameter("cMNV_clave", request.IdMotive));
                    command.Parameters.Add(new SqlParameter("bSIN_checkin", request.ValidCheckin));
                    command.Parameters.Add(new SqlParameter("cSIN_checkinlatitud", request.Latitude));
                    command.Parameters.Add(new SqlParameter("cSIN_checkinlongitud", request.Longitude));
                    command.Parameters.Add(new SqlParameter("dSIN_checkinfecha", request.DateCheckin.ToString("yyyy-MM-dd")));
                    command.Parameters.Add(new SqlParameter("dSIN_checkinhora", request.TimeCheckin.ToString("HH:mm:s")));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveMotiveNoVisit(RegisterCheckinRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SaveMotiveNoVisit"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", request.IdSync));
                    command.Parameters.Add(new SqlParameter("cRUT_clave", request.IdRoute));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", request.IdPromo));
                    command.Parameters.Add(new SqlParameter("nDIP_clave", request.IdDay));
                    command.Parameters.Add(new SqlParameter("cTIE_clave", request.IdStore));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", request.Email));
                    command.Parameters.Add(new SqlParameter("cMNV_clave", request.IdMotive));


                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void SaveCheckoutData(RegisterCheckoutRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SaveCheckoutData"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", request.IdSync));
                    command.Parameters.Add(new SqlParameter("cSIN_checkoutobservaciones", request.CommentsCheckout));
                    command.Parameters.Add(new SqlParameter("bSIN_checkout", request.ValidCheckout));
                    command.Parameters.Add(new SqlParameter("cSIN_checkoutlatitud", request.Latitude));
                    command.Parameters.Add(new SqlParameter("cSIN_checkoutlongitud", request.Longitude));
                    command.Parameters.Add(new SqlParameter("dSIN_checkoutfecha", request.DateCheckout.ToString("yyyy-MM-dd")));
                    command.Parameters.Add(new SqlParameter("dSIN_checkouthora", request.TimeCheckout.ToString("HH:mm:s")));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveSurveyData(RegisterSurveyRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SaveSurveyData"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", request.IdSync));
                    command.Parameters.Add(new SqlParameter("cTIE_clave", request.IdStore));
                    command.Parameters.Add(new SqlParameter("cENC_clave", request.IdSurvey));
                    command.Parameters.Add(new SqlParameter("cENC_comentarios", request.CommentsCheckout));
                    command.Parameters.Add(new SqlParameter("dENC_horainicio", request.DateTimeStartEvent.ToString("MM/dd/yyyy hh:mm:ss")));
                    command.Parameters.Add(new SqlParameter("dENC_horatermino", request.DateTimeEndEvent.ToString("MM/dd/yyyy hh:mm:ss")));
                    //command.Parameters.Add(new SqlParameter("dENC_horainicio", request.DateTimeStartEvent));
                    //command.Parameters.Add(new SqlParameter("dENC_horatermino", request.DateTimeEndEvent));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta1", request.Answer1));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta2", request.Answer2));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta3", request.Answer3));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta4", request.Answer4));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta5", request.Answer5));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta6", request.Answer6));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta7", request.Answer7));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta8", request.Answer8));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta9", request.Answer9));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta10", request.Answer10));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta11", request.Answer11));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta12", request.Answer12));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta13", request.Answer13));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta14", request.Answer14));
                    command.Parameters.Add(new SqlParameter("bENC_respuesta15", request.Answer15));


                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateEmail(string email)
        {
            bool valid = false;
            object user = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "ValidateEmail"));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", email));

                    user = command.ExecuteScalar();
                };

                if (!string.IsNullOrEmpty(user.ToString()))
                {
                    valid = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return valid;

        }

        private void GenerateSendPassword(string email)
        {
            int length = 5;
            var sb = new StringBuilder(length);

            while (sb.Length < length)
            {
                var tmp = System.Web.Security.Membership.GeneratePassword(length, 0);

                foreach (var c in tmp)
                {
                    if (char.IsDigit(c))
                    {
                        sb.Append(c);

                        if (sb.Length == length)
                        {
                            break;
                        }
                    }
                }
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "UpdateSendPassword"));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", email));
                    command.Parameters.Add(new SqlParameter("cPER_contrasena", sb.ToString()));

                    command.ExecuteNonQuery();
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable GetMotiveNoStores(string email, string idPromo, int idDay)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "MensajeRutaDelDia"));
                    command.Parameters.Add(new SqlParameter("cPER_cuentacorreo", email));
                    command.Parameters.Add(new SqlParameter("cPRO_clave", idPromo));
                    command.Parameters.Add(new SqlParameter("nDIP_clave", idDay));

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex) { throw ex; }
            return dt;
        }

        private void SaveSaleData(string idsync, string idstore, DataProductKPI pkpi, DateTime startdate, DateTime enddate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SaveSale"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", idsync));
                    command.Parameters.Add(new SqlParameter("cTIE_clave", idstore));
                    command.Parameters.Add(new SqlParameter("dVEN_horainicio", startdate));
                    command.Parameters.Add(new SqlParameter("dVEN_horatermino", enddate));
                    command.Parameters.Add(new SqlParameter("cCAP_clavecategoria", pkpi.IdCategory));
                    command.Parameters.Add(new SqlParameter("cKPI_clave", pkpi.IdKPI));
                    command.Parameters.Add(new SqlParameter("cPRD_claveproducto", pkpi.IdProduct));
                    command.Parameters.Add(new SqlParameter("cVEN_valor", pkpi.KPIValue));
                    command.Parameters.Add(new SqlParameter("cVEN_observaciones", pkpi.ObservationsSale));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void SaveAuditData(string idsync, string idstore, DataProductAudit pa, DateTime startdate, DateTime enddate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SaveAudit"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", idsync));
                    command.Parameters.Add(new SqlParameter("cTIE_clave", idstore));
                    command.Parameters.Add(new SqlParameter("dAUD_horainicio", startdate));
                    command.Parameters.Add(new SqlParameter("dAUD_horatermino", enddate));
                    command.Parameters.Add(new SqlParameter("cCAP_clavecategoria", pa.IdCategory));
                    command.Parameters.Add(new SqlParameter("cPRD_claveproducto", pa.IdProduct));
                    command.Parameters.Add(new SqlParameter("nAUD_inventario", pa.FinalInventory));
                    command.Parameters.Add(new SqlParameter("nAUD_frente", pa.Front));
                    command.Parameters.Add(new SqlParameter("cUPR_clave", pa.IdUbication));
                    command.Parameters.Add(new SqlParameter("nAUD_posicion", pa.Position));
                    command.Parameters.Add(new SqlParameter("nAUD_precio", pa.Price));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SavePlacementData(string idsync, string idstore, DataMaterialPlacement mp, DateTime startdate, DateTime enddate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("spWS_PosData_PMKTData", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("Action", "SavePlacement"));
                    command.Parameters.Add(new SqlParameter("nSIN_clave", idsync));
                    command.Parameters.Add(new SqlParameter("cTIE_clave", idstore));
                    command.Parameters.Add(new SqlParameter("dCOL_horainicio", startdate));
                    command.Parameters.Add(new SqlParameter("dCOL_horatermino", enddate));
                    command.Parameters.Add(new SqlParameter("cTIM_clave", mp.IdTypeMaterial));
                    command.Parameters.Add(new SqlParameter("nCOL_nuevo", string.IsNullOrEmpty(mp.New)?"0": mp.New));
                    command.Parameters.Add(new SqlParameter("nCOL_cantidad", string.IsNullOrEmpty(mp.Amount) ? "0" : mp.Amount));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
