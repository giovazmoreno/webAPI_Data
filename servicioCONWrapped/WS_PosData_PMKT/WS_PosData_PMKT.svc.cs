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
            Crypto crypto = new Crypto();
            UserDataResponse userresponse = new UserDataResponse();
            crypto.Cypher(ref request, 2);
            DataTable dt = new DataTable();
            string email = request.Email;
            string psw = request.Password;

            try
            {

                dt = ValidateUser(email, psw);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["nPER_estatus"].ToString() == "True")
                    {
                        if (dt.Rows[0]["Licencia"].ToString() == "1")
                        {
                            DataTable dt2 = GetGeneralDataUser(dt.Rows[0]["nPER_clave"].ToString());
                            userresponse.DataUser.IdUser = Convert.ToInt32(dt2.Rows[0]["nPER_clave"].ToString());
                            userresponse.DataUser.NameEmployee = dt2.Rows[0]["cPER_nombres"].ToString();
                            userresponse.DataUser.LastName = dt2.Rows[0]["cPER_apellidopaterno"].ToString();
                            userresponse.DataUser.MothersLastName = dt2.Rows[0]["cPER_apellidomaterno"].ToString();
                            userresponse.DataUser.IdStaffType = dt2.Rows[0]["nTIP_clave"].ToString();
                            userresponse.DataUser.NameStaffType = dt2.Rows[0]["cTIP_nombre"].ToString();
                            userresponse.DataUser.Email = dt2.Rows[0]["cPER_cuentacorreo"].ToString();

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
                                ValidateCheckIn = Convert.ToBoolean(p["bPRO_validarcheckin"].ToString())
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
            NoVisitMotiveWrapped objNovisit = new NoVisitMotiveWrapped();
            SurveyWrapped objSurvey = new SurveyWrapped();
            PhotographicEvidenceWrapped objEvidence = new PhotographicEvidenceWrapped();
            ProductsWrapped objProduct = new ProductsWrapped();
            TypeMaterialWrapped objMaterial = new TypeMaterialWrapped();
            UbicationProductWrapped objUbication = new UbicationProductWrapped();
            KPIWrapped objKPI = new KPIWrapped();

            int countEmptyTables = 0;
            try
            {
                DataSet ds = GetDataDetailPromo(IdPromo);
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objNovisit.Code = "MS001";
                                objNovisit.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objNovisit.ListNoVisitMotive = FillListNoVisit(ds.Tables[i]);
                                objNovisit.IsEmpty = false;
                                detailevent.ListNoVisitMotive = objNovisit;
                            }
                            break;
                        case 1:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objSurvey.Code = "MS001";
                                objSurvey.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objSurvey.ListSurvey = FillListSurvey(ds.Tables[i]);
                                objSurvey.IsEmpty = false;
                                detailevent.ListSurvey = objSurvey;
                            }
                            break;
                        case 2:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objEvidence.Code = "MS001";
                                objEvidence.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objEvidence.ListTypeEvidence = FillTypeEvidence(ds.Tables[i]);
                                objEvidence.IsEmpty = false;
                                detailevent.ListTypeEvidence = objEvidence;
                            }
                            break;
                        case 3:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objProduct.Code = "MS001";
                                objProduct.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objProduct.ListProductsByCategory = FillListProducts(ds.Tables[i]);
                                objProduct.IsEmpty = false;
                                detailevent.ListProductsByCategory = objProduct;
                            }
                            break;
                        case 4:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objMaterial.Code = "MS001";
                                objMaterial.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objMaterial.ListTypeMaterial = FillListMaterial(ds.Tables[i]);
                                objMaterial.IsEmpty = false;
                                detailevent.ListTypeMaterial = objMaterial;
                            }
                            break;
                        case 5:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objUbication.Code = "MS001";
                                objUbication.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objUbication.ListUbication = FillListUbication(ds.Tables[i]);
                                objUbication.IsEmpty = false;
                                detailevent.ListUbication = objUbication;
                            }
                            break;
                        case 6:
                            if (ds.Tables[i].Rows.Count == 0)
                            {
                                objKPI.Code = "MS001";
                                objKPI.IsEmpty = true;

                                countEmptyTables += countEmptyTables;
                            }
                            else
                            {
                                objKPI.ListKPI = FillListKPI(ds.Tables[i]);
                                objKPI.IsEmpty = false;
                                detailevent.ListKPI = objKPI;
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
                    detaileventsresponse.Code = "P002";
                    detaileventsresponse.Message = "Error general de Promoción: Promoción mal diseñada.";
                    detaileventsresponse.Success = false;
                }
                if (ds.Tables.Count == 0)
                {
                    detaileventsresponse.Code = "MS001";
                    detaileventsresponse.Message = "Estructura de datos vacía";
                    detaileventsresponse.Success = false;
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
                    lstProducts.Add(new Products { IdCategory = r["cCAP_clavecategoria"].ToString(), NameCategory = r["cCAP_nombre"].ToString(),
                    IdProduct = r["cPRD_claveproducto"].ToString(), NameProduct = r["cPRD_nombre"].ToString()
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
                        Question5 = r["cENC_pregunta5"].ToString()
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
        #endregion
    }
}
